using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using DNF.Security.Bussines;
using System.Configuration;
using DepositControl.Areas.Bcri.Models;
using System.Security.Principal;
using DepositControl.Areas.Bcri.Utility;

namespace DepositControl
{
    public class Authenticated : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (!isLogin(filterContext))
                return;

            if (isDuplicateSession(filterContext))
                return;

            if (isSessionExpired(filterContext))
                return;
          
          
            base.OnActionExecuting(filterContext);
        }
        private bool isLogin(ActionExecutingContext filterContext)
        {
            if (Current.User != null)
                return true;

            if (SecurityUtility.LoginUserFromToken(filterContext.RequestContext.HttpContext.Request.Headers))
                return true;

            //FormsAuthentication.SignOut();
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseActiveDirectory"]))
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Login",
                    ReturnUrl = filterContext.RequestContext.HttpContext.Request.Url
                }));
            else
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Out",
                }));


            return false;
        }

        private bool isDuplicateSession(ActionExecutingContext filterContext)
        {

            if (SettingsSiteUtility.AllowDoubleSession)
                return false;

            //refresco el usuario que esta en la session desde la base de datos por si cambio el SessionId
            Current.User = User.Dao.Get(Current.User.Id);
            if (string.IsNullOrEmpty(Current.User.SessionId))
            {
                Current.User.SessionId = filterContext.HttpContext.Session.SessionID;
                Current.User.Save();
            }
            else if (Current.User.SessionId != filterContext.HttpContext.Session.SessionID) // que no sean la Actual
            {
               if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.HttpContext.Response.Write("SessionDuplicate");
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Out",
                        action = "SessionDuplicate"
                    }));
                }
                return true;
            }

            Current.User.HasActivity();
            return false;

        }

        private bool isSessionExpired(ActionExecutingContext filterContext)
        {


            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseActiveDirectory"]))
                return false;

            HttpContext ctx = HttpContext.Current;
            if (ctx.Session?.IsNewSession != true)
                return false;

            if (ctx.Request.Headers["Cookie"]?.Contains("ASP.NET_SessionId") != true)
                return false;

            LogAccion.Dao.AddLog("Timeout", Current.User.Name);

            ctx.Session.Clear();
            ctx.Session.Abandon();
            ctx.Request.Cookies["ASP.NET_SessionId"].Value = string.Empty;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.HttpContext.Response.Write("SessionTimeout");
                filterContext.HttpContext.Response.End();
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Out",
                    action = "SessionExpired"
                }));
            }
            return true;          
        }

    }

}
