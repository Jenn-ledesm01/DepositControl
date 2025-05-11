using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DNF.Entity;
using Bcri.Core.Bussines;
using System.Web.Mvc;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class DocumentationController : Controller
    {
        // GET: Bcri/Documentation
        private ProcessConfig fullyLoadProcessConfig(string processConfigCode)
        {
            var processConfig = ProcessConfig.Dao.GetByCode(processConfigCode);

            //processConfig.InputConfigs.LoadRelation(x => x.RepositoryConfig);
            //processConfig.OutputConfigs.LoadRelation(x => x.RepositoryConfig);

            var allRepos = processConfig.InputConfigs.Select(x => x.RepositoryConfig).ToList();
            allRepos.AddRange(processConfig.OutputConfigs.Select(x => x.RepositoryConfig));

            allRepos.LoadRelation(x => x.Entity);
            allRepos.Select(x => x.Entity).ToList().LoadRelationList(x => x.Structs);
            return processConfig;
        }
        public ActionResult Manual(string processConfigCode)
        {
            return View(fullyLoadProcessConfig(processConfigCode));
        }
    }
}
