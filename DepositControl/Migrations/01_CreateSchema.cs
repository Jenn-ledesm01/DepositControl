using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace DatabaseVersionControl
{

    [Migration(1)]
    public class CreateSchema : Migration
    {
        public override void Up()
        {
            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\CreateSchema.sql"));
        }

        public override void Down()
        {
            //
        }
    }
}