using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using DNF.Security.Bussines;
using Current = DNF.Security.Bussines.Current;
using DepositControl.Areas.Bcri.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using DepositControl.Areas.Bcri.Controllers;
using System.Globalization;
using System.Threading;
using DepositControl.Areas.Bcri.Utility;

using DNF.Release.Bussines;
using log4net;

namespace DepositControl
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));

        private Migrator _migrator;
        Migrator Migrator
        {
            get
            {
                return _migrator ?? (_migrator = new Migrator());
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();

            SecurityUtility.WebConfigEncrypt();


            //Migracion
            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
            string runMigrationValue = ConfigurationManager.AppSettings.Get("RunMigration");
            bool runMigration = runMigrationValue != null && runMigrationValue == "1";
            if (connectionString != null && runMigration)
            {
               this.Migrator.Run(connectionString, System.Reflection.Assembly.GetExecutingAssembly());
            }


            DNF.Security.Bussines.User.Dao.ClearAllSessions();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseActiveDirectory"]))
            {
                User user = DNF.Security.Bussines.User.Dao.GetFromIdentity(Request.LogonUserIdentity);
                if (user == null)
                {
                    Current.User = DNF.Security.Bussines.User.Dao.Get(1);
                    LogAccion.Dao.AddLog("UserDoesNotExist", Request.LogonUserIdentity?.Name ?? "");
                    Current.User = null;
                }
                if (user?.Login() != true)
                {
                    Response.RedirectToRoute("Bcri_default", new { controller = "Out", action = "AccessDenied" });
                    Response.End();
                    return;
                }

            }
            if (Current.User != null)
                return;

            if (SecurityUtility.LoginUserFromCookie(Request.Cookies))
                return;

            if (SecurityUtility.LoginUserFromToken(Request.Headers))
                return;
           
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            //languaje
            if (SettingsSiteUtility.UserChooseCulture && Current.User?.Language?.Code != null)
            {
                var currentCulture = CultureInfo.GetCultureInfo(Current.User.Language.Code);

                Thread.CurrentThread.CurrentUICulture = currentCulture;
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }                
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is ThreadAbortException)
                return;

            log.Error("App_Error", ex);

        }
        protected void Session_End(object sender, EventArgs e)
        {
            User user = ((User)Session["CurrentUser"]);
            if (user != null)
            {
                LogAccion.Dao.AddLog(user, "Timeout", user.Name);
                user.LoginOut(false);
            }

        }


    }
}

