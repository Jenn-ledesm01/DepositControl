using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace DatabaseVersionControl
{

    [Migration(3)]
    public class CreateProcedures : Migration
    {
        public override void Up()
        {

            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\CreateProcedures.sql"));
        }

        public override void Down()
        {
            //
        }
    }
}