using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace DatabaseVersionControl
{

    [Migration(4)]
    public class CreateData : Migration
    {
        public override void Up()
        {
            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\CreateData.sql"));
        }

        public override void Down()
        {
            //
        }
    }
}