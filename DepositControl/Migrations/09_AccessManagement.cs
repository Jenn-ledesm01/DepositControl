using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace DatabaseVersionControl
{

    [Migration(9)]
    public class CreateAccessManagement : Migration
    {
        public override void Up()
        {

            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\CreateAccessManagement.sql"));
        }

        public override void Down()
        {
            //
        }
    }
}