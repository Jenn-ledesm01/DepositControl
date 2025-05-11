using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace DatabaseVersionControl
{

    [Migration(11)]
    public class ProcessAccessDefaultWithRes : Migration
    {
        public override void Up()
        {
            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\ProcessAccessDefaultWithRes.sql"));
        }

        public override void Down()
        {
            //
        }
    }
}