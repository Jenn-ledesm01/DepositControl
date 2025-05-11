using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DNF.CodeGenerator;
using DNF.Entity;
using DepositControl.Areas.Bcri.Models;
using DNF.Security.Bussines;
using DNF.Structure.Bussines;
using DNF.Type.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{
	public class LogController : DynamicRenderController
	{
		public ActionResult Index(string entity)
		{
			List<SelectListItem> types = TypeConfig.Dao.GetByCode("AccionCategory").Types.ToListItemCode();

			ViewBag.Types = types;

			return View();
		}

		public ActionResult GetLogAccion(string typeCode)
		{
			DNF.Type.Bussines.Type type = DNF.Type.Bussines.Type.Dao.GetByFilter(new
			{
				Code = typeCode
			}).FirstOrDefault();

			List<Accion> acciones = Accion.Dao.GetByFilter(new
			{
				Category_Type_Id = type.Id
			});

			return Json(acciones.Select(x => new
				{
					id = x.Id,
					value = x.Name
				})
				, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetDynamicDataSet(string actionId)
		{
			//LogAccion logAccion = null;
			List<object> result = new List<object>();

			List<LogAccion> logsAccion = LogAccion.Dao.GetByFilter(new
			{
				Accion_Id = actionId
			});
			logsAccion.LoadRelation(x => x.User);
			logsAccion = logsAccion.OrderByDescending(x => x.Date).ToList();
			
			if (logsAccion.Count() > 0)
			{
				logsAccion.LoadRelation(x => x.DynamicDataSet);
				List<DynamicDataSet> dynamicsDataSet = logsAccion.Select(X => X.DynamicDataSet).ToList();
				//dynamicsDataSet.ForEach(X => X.DynamicDataSetRows = new List<DynamicDataSetRow>());
				dynamicsDataSet.LoadRelationList(y => y.DynamicDataSetRows);
				var rows = dynamicsDataSet.SelectMany(x => x.DynamicDataSetRows).ToList();
				//rows.ForEach(x => x.Datas = new List<DynamicDataSetData>());
				rows.LoadRelationList(y => y.Datas); 

				foreach (LogAccion logAccion in logsAccion)
				{
					if (logAccion.DynamicDataSet != null)
					{
						foreach (DynamicDataSetData dynamicDataSetData in logAccion.DynamicDataSet.DynamicDataSetRows.SelectMany(u => u.Datas))
						{
							result.Add(new
							{
								id = logAccion.Id,
								dataId = dynamicDataSetData.DynamicDataSetRow.Id,
								date = logAccion.Date,
								user = new
								{
									id = logAccion.User.Id,
									name = logAccion.User.Name
								},
								actionTo = logAccion.AccionTo,
								data = logAccion.Data,
								oldValue = dynamicDataSetData.OldValue,
								value = dynamicDataSetData.Value
							});
						}
					}
					else
					{
						result.Add(new
						{
							id = logAccion.Id,
							dataId = 0,
							date = logAccion.Date,
							user = new
							{
								id = logAccion.User.Id,
								name = logAccion.User.Name
							},
							actionTo = logAccion.AccionTo,
							data = logAccion.Data,
							oldValue = string.Empty,
							value = string.Empty
						});
					}
				}
				/*
				

				

				
				*/
			}

			return Json(result.ToArray(), JsonRequestBehavior.AllowGet);
		}
	}
}
