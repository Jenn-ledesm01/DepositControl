using System;
using System.Linq;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using Newtonsoft.Json;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class ReportController : Controller
    {
		[Authenticated]
		public ActionResult Index(int  id  = 0, string reportCode = null)
        {
            Report Reporte =  !string.IsNullOrEmpty(reportCode) 
                                ? Report.Dao.GetByCode(reportCode)
                                : Report.Dao.Get(id);
            if (Reporte == null && !string.IsNullOrEmpty(reportCode) )
            { 
                new Wizard().CreateReport(reportCode);
                Reporte = Report.Dao.GetByCode(reportCode);
            }

            if (Reporte == null)
                return Json($"El report {reportCode} no existe en la base de datos");


            try
            {
                return new JsonReaderResult(Reporte.GetData(Request.Params));
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }

        }

		[Authenticated]
		public ActionResult Data(int id = 0, string reportCode = null)
        {
			return this.data(id, reportCode);
		}


		public ActionResult DataREST(int id = 0, string reportCode = null)
		{
            return this.data(id, reportCode);
		}

		private ActionResult data(int id = 0, string reportCode = null)
		{
			Report Reporte = !string.IsNullOrEmpty(reportCode)
								? Report.Dao.GetByCode(reportCode)
								: Report.Dao.Get(id);
			if (Reporte == null && !string.IsNullOrEmpty(reportCode))
			{
				new Wizard().CreateReport(reportCode);
				Reporte = Report.Dao.GetByCode(reportCode);
			}
			if (Reporte == null)
				return null;// aca error 


			//return new JsonReaderResult(Reporte.GetData(Request.Params));
			try
			{
				return new JsonReaderResult(Reporte.GetData(Request.Params),
					new JsonSerializer() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 500;
				HttpContext.Response.SubStatusCode = 100;
				return Content(ex.Message);
			}
		}

        public JsonResult RepositoryDepency(string reportCode)
        {
            var report = Report.Dao.GetByCode(reportCode);

            var ret = from rc in report.GetRepositoryConfigsDepency()
                      select new
                      {
                          Code = rc.Code,
                          Name = rc.Name,
                          Type = rc.Type.Name,
                          ProcessName = rc.ParentProcessConfig?.Name ?? "",
                          ProcessCode = rc.ParentProcessConfig?.Code ?? ""
                      };

            return Json(ret.ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}
