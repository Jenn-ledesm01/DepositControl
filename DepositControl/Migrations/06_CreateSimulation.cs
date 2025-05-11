using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace DatabaseVersionControl
{

    [Migration(6)]
    public class CreateSimulation : Migration
    {
        public override void Up()
        {

            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\CreateSimulation.sql"));
        }

        public override void Down()
        {
            //
        }
    }
}