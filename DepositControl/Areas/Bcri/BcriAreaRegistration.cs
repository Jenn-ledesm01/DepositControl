using System.Web.Http;
using System.Web.Mvc;

namespace DepositControl.Areas.Bcri
{
    public class BcriAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Bcri";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "Process", // Route name
                "Process/{processCode}/{action}/{Id}", // URL with parameters
                new { controller = "Process", action = "Processes", id = UrlParameter.Optional, area = "Bcri" },
                new { controller = "Process" },
                new[] { "DepositControl.Areas.Bcri.Controllers" } // Parameter defaults
            );

            context.MapRoute(
               "ConfigCreate", // Route name
               "Config/CreateConfig", // URL with parameters
               new { controller = "Config", action = "CreateConfig", id = UrlParameter.Optional, area = "Bcri" },
               new { controller = "Config",  action = "CreateConfig" },
               new[] { "DepositControl.Areas.Bcri.Controllers" } // Parameter defaults
           );


            context.MapRoute(
                "Config", // Route name
                "Config/{processCode}/{action}/{Id}", // URL with parameters
                new { controller = "Config", action = "Index", id = UrlParameter.Optional, area = "Bcri" },
                new { controller = "Config" },
                new[] { "DepositControl.Areas.Bcri.Controllers" } // Parameter defaults
            );

            context.MapRoute(
                "Diagram", // Route name
                "Diagram/{action}/{entity}", // URL with parameters
                new { controller = "Diagram", action = "Entity", entity = UrlParameter.Optional },
                new { controller = "Diagram" },
                new[] { "DepositControl.Areas.Bcri.Controllers", "DepositControl.Controllers" } // Parameter defaults
            );

            context.MapRoute(
               "Grid", // Route name
               "Grid/{action}/{entityName}", // URL with parameters
               new { controller = "Grid", action = "Index", id = UrlParameter.Optional, area = "Bcri" },
               new { controller = "Grid" },
               new[] { "DepositControl.Areas.Bcri.Controllers" } // Parameter defaults
            );


            context.MapRoute(
               "RepositoryLoadModelData", // Route name
               "Repository/LoadColModelAndData/{Id}", // URL with parameters
               new { controller = "Repository", action = "LoadColModelAndData", id = UrlParameter.Optional, area = "Bcri" },
               new { controller = "Repository" },
               new[] { "DepositControl.Areas.Bcri.Controllers" } // Parameter defaults
            );

            context.MapRoute(
               "Repository", // Route name
               "Repository/{repositoryCode}/{action}/{Id}", // URL with parameters
               new { controller = "Repository", action = "Index", id = UrlParameter.Optional, area = "Bcri" },
               new { controller = "Repository" },
               new[] { "DepositControl.Areas.Bcri.Controllers" } // Parameter defaults
            );


            context.MapRoute(
                "Wizard_default",
                "Wizard/{action}/{entity}",
                new { controller = "Wizard", action = "Entity", entity = UrlParameter.Optional },
                new { controller = "Wizard" },
                new[] { "DepositControl.Areas.Bcri.Controllers", "DepositControl.Controllers" }
            );

			context.MapRoute(
				"RESTLogin",
				"api/login",
				new { controller = "Login", action = "LoginREST" },
				new { controller = "Login" },
				new[] { "Pmp.Web.Areas.Bcri.Controllers", "Pmp.Web.Controllers" }
			);

			context.MapRoute(
				"RESTReport",
				"api/report",
				new { controller = "Report", action = "DataREST" },
				new { controller = "Report" },
				new[] { "Pmp.Web.Areas.Bcri.Controllers", "Pmp.Web.Controllers" }
			);

			context.MapRoute(
              "Bcri_default",
              "{controller}/{action}/{id}",
              new { action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "DepositControl.Areas.Bcri.Controllers", "DepositControl.Controllers" }
            );


            BcriConfig.Register();

        }
    }
}
