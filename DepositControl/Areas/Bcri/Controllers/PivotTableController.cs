using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DepositControl.Areas.Bcri.Models;
using DNF.Security.Bussines;
using DNF.Structure.Bussines;
using Newtonsoft.Json;

namespace DepositControl.Areas.Bcri.Controllers
{
    [Authenticated]
    public class PivotTableController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.tmpDataSource = GetPermissionsPivotCube();
            ViewBag.Title = "Pivot Table";

            return View();
        }

        public JsonResult FilterInfo(string dataSource, string dateRange, string dateStart, string dateEnd)
        {
            return Json(GetData(dataSource, dateRange, dateStart, dateEnd), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadQueryInfo(int idDataSource)
        {
            var query = GetSavedQueries(idDataSource);
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckQuery(string nameQuery)
        {
            string menssageError;

            try
            {
                if (string.IsNullOrEmpty(nameQuery))
                    return Json(true, JsonRequestBehavior.AllowGet);

                var s = PivotQuery.Dao.GetAll()
                    .Where(z => string.Equals(z.Name.Trim(), nameQuery, StringComparison.CurrentCultureIgnoreCase));
                if (!s.Any())
                    return Json(true, JsonRequestBehavior.AllowGet);

                menssageError = string.Format(CultureInfo.InvariantCulture,
                    "The report {0} already exists in the database.", nameQuery);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(menssageError, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadQuery(int idSelectedQuery)
        {
            object result = null;

            try
            {
                if (idSelectedQuery <= 0) return Json(null, JsonRequestBehavior.AllowGet);

                var query = PivotQuery.Dao.Get(idSelectedQuery);
                if (query != null)
                {
                    result = query.PivotQueryStructs.Select(s => new
                    {
                        hiddenAttributes = string.IsNullOrEmpty(s.HiddenAttributes) ? new string[] { } : s.HiddenAttributes.Trim().Replace(" ", "").Split(','),
                        menuLimit = Convert.ToInt32(s.MenuLimit),
                        cols = string.IsNullOrEmpty(s.Cols) ? new string[] { } : s.Cols.Trim().Replace(" ", "").Split(','),
                        rows = string.IsNullOrEmpty(s.Rows) ? new string[] { } : s.Rows.Trim().Replace(" ", "").Split(','),
                        vals = string.IsNullOrEmpty(s.Vals) ? new string[] { } : s.Vals.Trim().Replace(" ", "").Split(','),
                        exclusions = s.Exclusions,
                        inclusions = s.Inclusions,
                        unusedAttrsVertical = Convert.ToInt32(s.UnusedAttrsVertical),
                        autoSortUnusedAttrs = Convert.ToBoolean(s.AutoSortUnusedAttrs),
                        inclusionsInfo = s.InclusionsInfo,
                        aggregatorName = s.AggregatorName,
                        rendererName = s.RendererName
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveQuery(string dataJson)
        {
            object result = null;

            try
            {
                var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataJson);
                if (obj != null)
                {
                    var objData = JsonConvert.DeserializeObject<JsonQueryPivot>(obj["dataNewQuery"]);
                    if (objData != null)
                    {
                        var query = Convert.ToInt32(obj["idSelectedQuery"]) == 0
                            ? new PivotQuery()
                            : PivotQuery.Dao.Get(Convert.ToInt32(obj["idSelectedQuery"]));

                        var isNew = query.IsNew();
                        if (isNew)
                        {
                            query.Name = obj["nameNewQuery"];
                            query.Code = obj["nameNewQuery"];
                            query.User = Current.User;
                            query.RepositoryConfig = new RepositoryConfig { Id = Convert.ToInt32(obj["idDataSource"]) };
                            query.Save();
                        }

                        var structQuery = isNew
                            ? new PivotQueryStruct()
                            : query.PivotQueryStructs?.FirstOrDefault();

                        structQuery.HiddenAttributes = objData.HiddenAttributes.Count == 0 ? string.Empty : string.Join(", ", objData.HiddenAttributes);
                        structQuery.MenuLimit = objData.MenuLimit.ToString();
                        structQuery.Cols = objData.Cols.Count == 0 ? string.Empty : string.Join(", ", objData.Cols);
                        structQuery.Rows = objData.Rows.Count == 0 ? string.Empty : string.Join(", ", objData.Rows);
                        structQuery.Vals = objData.Vals.Count == 0 ? string.Empty : string.Join(", ", objData.Vals);
                        structQuery.Exclusions = objData.Exclusions.Count == 0 ? string.Empty : FromDictionaryToJson(objData.Exclusions);
                        structQuery.Inclusions = objData.Inclusions.Count == 0 ? string.Empty : FromDictionaryToJson(objData.Inclusions);
                        structQuery.UnusedAttrsVertical = objData.UnusedAttrsVertical.ToString();
                        structQuery.AutoSortUnusedAttrs = objData.AutoSortUnusedAttrs.ToString();
                        structQuery.InclusionsInfo = objData.InclusionsInfo.Count == 0 ? string.Empty : FromDictionaryToJson(objData.InclusionsInfo);
                        structQuery.AggregatorName = objData.AggregatorName;
                        structQuery.RendererName = objData.RendererName;
                        structQuery.PivotQuery = query;
                        structQuery.Save();

                        result = query.Id;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteQuery(int idSelectedQuery)
        {
            try
            {
                if (idSelectedQuery <= 0) return Json(null, JsonRequestBehavior.AllowGet);

                var query = PivotQuery.Dao.Get(idSelectedQuery);
                PivotQueryStruct.Dao.Delete(query.PivotQueryStructs);
                query.Delete();
            }
            catch (Exception)
            {

                throw;
            }

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        #region private -> methods
        private static List<SelectListItem> GetPermissionsPivotCube()
        {
            var cubos = RepositoryConfig.Dao.GetAll()
                .Where(x => Current.User.HasAccess("CUBE_PIVOT_" + x.Code.Trim()));

            return cubos.ToListItem();
        }
        private static List<dynamic> GetData(string repositoryId, string dateRange, string dateStart, string dateEnd)
        {
            var data = new List<dynamic>();

            try
            {
                if (!string.IsNullOrEmpty(repositoryId))
                {
                    var parameters = GetParametersArray(dateRange, dateStart, dateEnd);
                    

                    var repositoryConfig = RepositoryConfig.Dao.Get(repositoryId); ;

                    if (repositoryConfig == null) return null;

                    var repositories = Repository.Dao.GetByPeriod(repositoryConfig, parameters[0], parameters[1]);
                    if (repositories == null) return null;

                    //repositoryConfig.Entity = Entity.Dao.Get(repositoryConfig.Entity.Id);
                    //var type = Type.GetType(repositoryConfig.Entity.Class + "," + repositoryConfig.Entity.Assembly);

                    var entity = Entity.Dao.Get(repositoryConfig.Entity.Id);
                    var type = Type.GetType(entity.Class + "," + entity.Assembly);

                    var dao = type?.GetProperty("Dao").GetValue(null, null);

                    var methodInfo = dao?.GetType().GetMethod("GetBy", new[] { typeof(string), typeof(IEnumerable<long>) });
                    if (methodInfo == null) return null;

                    var parametersInfo = methodInfo.GetParameters();
                    var result = (methodInfo.Invoke(dao, parametersInfo.Length == 0 ? null : new object[] { "Repository", repositories.Select(x => x.Id) }) as IEnumerable<IToPivot>);
                    if (result != null)
                        data = result.Select(x => x.ToPivot()).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return data;
        }
        private static List<SelectListItem> GetSavedQueries(int idDataSource)
        {
            var queriesByPivotCube = PivotQuery.Dao.GetByFilter(new { RepositoryConfig_Id = idDataSource })
                .OrderBy(o => o.Name);

            return queriesByPivotCube.ToListItem(" ");
        }
        private static string FromDictionaryToJson(Dictionary<string, List<object>> dictionary)
        {
            var dictmp = new Dictionary<string, List<string>>();
            foreach (var keyDic in dictionary)
            {
                var list = keyDic.Value.Select(valueDic => $"\"{valueDic}\"").ToList();
                dictmp.Add(keyDic.Key, list);
            }

            var kvs = dictmp.Select(kvp => $"\"{kvp.Key}\":[{string.Join(",", kvp.Value)}]");
            return string.Concat("{", string.Join(",", kvs), "}");
        }
        private static string FromDictionaryToJson(Dictionary<string, string> dictionary)
        {
            var kvs = dictionary.Select(kvp => $"\"{kvp.Key}\":\"{string.Join(",", kvp.Value)}\"");
            return string.Concat("{", string.Join(",", kvs), "}");
        }
        private static Dictionary<string, string> FromJsonToDictionary(string json)
        {
            var keyValueArray = json.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');
            return keyValueArray.ToDictionary(item => item.Split(':')[0], item => item.Split(':')[1]);
        }
        #endregion

        private static object[] GetParametersArray(string dateRange, string dateStart, string dateEnd)
        {
            if (string.IsNullOrEmpty(dateRange))
            {
                if (string.IsNullOrEmpty(dateStart))
                    dateStart = DateTime.Today.ToString(CultureInfo.CreateSpecificCulture("en-US"));

                if (string.IsNullOrEmpty(dateEnd))
                    dateEnd = DateTime.Today.ToString(CultureInfo.CreateSpecificCulture("en-US"));

                DateTime fecDesde;
                if (!DateTime.TryParseExact(dateStart, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecDesde))
                    return null;

                DateTime fecHasta;
                if (!DateTime.TryParseExact(dateEnd, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecHasta))
                    return null;

                return new object[] { fecDesde, fecHasta };
            }

            switch (dateRange)
            {
                case "todayDate":
                    return new object[] { DateTime.Today, DateTime.Today };
                case "lastWeek":
                    var lastWeek = DateTime.Today.AddDays(-7);
                    return new object[] { lastWeek, DateTime.Today };
                case "lastMonth":
                    var lastMonth = DateTime.Today.AddDays(-30);
                    return new object[] { lastMonth, DateTime.Today };
                case "lastSixMonths":
                    var lastSixMonths = DateTime.Today.AddMonths(-6);
                    return new object[] { lastSixMonths, DateTime.Today };
                case "lastYear":
                    var lastYear = DateTime.Today.AddDays(-365);
                    return new object[] { lastYear, DateTime.Today };
            }

            return null;
        }
    }
}
