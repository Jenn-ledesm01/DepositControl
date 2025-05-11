using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DNF.Entity;
using DNF.Security.Bussines;
using DNF.Structure.Bussines;
using java.sql;
using Type = DNF.Type.Bussines.Type;
using Profile = DNF.Security.Bussines.Profile;
using Struct = DNF.Structure.Bussines.Struct;
using System.Globalization;
using DepositControl.Areas.Bcri.Models;
using DNF.Release.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class HolidayController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult HolidayData()
        {
            var HolidayData = FrequencyHolyDay.Dao
                .GetAll().Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Date
                });
            
            return Json(HolidayData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HolidayEdit(string oper, DateTime? date, string Name, long? id)
        {
            FrequencyHolyDay holiday = new FrequencyHolyDay();
            if (oper == "edit" || oper == "del")
            {
                holiday = FrequencyHolyDay.Dao.Get(id);
            }

            if (oper == "del")
            {
                holiday.Delete();
            }
            else
            {
                holiday.Name = Name;
                holiday.Date = Convert.ToDateTime(date);
                holiday.Deleted = false;
                try
                {
                    holiday.Save();
                }
                catch (Exception e)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content(e.Message);
                }
            }
            return PartialView("_GridHoliday");
        }

        public ActionResult HolidayDel(string oper, long? id)
        {
            if (oper == "del")
            {
                FrequencyHolyDay holiday = new FrequencyHolyDay();
                holiday = FrequencyHolyDay.Dao.Get(id);
                holiday.Delete();
            }
            return PartialView("_GridHoliday");
        }
    }
}
