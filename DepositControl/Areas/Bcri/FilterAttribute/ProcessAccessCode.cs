using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bcri.Core.Bussines;
using DNF.Security.Bussines;


namespace DepositControl
{
    public class ProcessAccessCode : ActionFilterAttribute
    {
        public string Code { get; set; }

        public ProcessAccessCode() { }
        public ProcessAccessCode(string code)
        {
            Code = code;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ProcessConfig config = null;

            /*
                valido permisos para paginas que pertencen a un processconfig
                processCode esta definido en el rutter y queda dentro de la url
                si no busco un processConfigId entre los parametros del request
                si no un processId, traigo el process y despues recupero el processConfig
                finalmente co
                ncateno el sub codigo y veo si el current user tiene ese permiso 
            */
            var Params = filterContext.RequestContext.HttpContext.Request.Params;

            if (filterContext.RouteData.Values.ContainsKey("processCode"))
                config = ProcessConfig.Dao.GetByCode(filterContext.RouteData.Values["processCode"].ToString());
            else if (filterContext.RouteData.Values.ContainsKey("ProcessCode"))
                config = ProcessConfig.Dao.GetByCode(filterContext.RouteData.Values["ProcessCode"].ToString());


            else if (Params.AllKeys.Contains("processConfigId"))
                config = ProcessConfig.Dao.Get(int.Parse(Params["processConfigId"]));
            else if (Params.AllKeys.Contains("processId"))
                config = Process.Dao.Get(int.Parse(Params["processId"]))?.Config;
            else if (Params.AllKeys.Contains("ProcessConfigId"))
                config = ProcessConfig.Dao.Get(int.Parse(Params["ProcessConfigId"]));
            else if (Params.AllKeys.Contains("ProcessId"))
                config = Process.Dao.Get(int.Parse(Params["ProcessId"]))?.Config;
            if (config == null)
            {
                filterContext.Result = new RedirectToRouteResult( new RouteValueDictionary( new
                    {
                        controller = "Out", action = "AccessDenied"
                    }));
                return;
            }                
            var code = config.AccessCode + Code;
            if (!Current.User.HasAccess(code))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Out", action = "AccessDenied"
                    }));
                return;
            }

            Current.Access = Access.Dao.GetByCode(code);

            base.OnActionExecuting(filterContext);
        }
    }
}
