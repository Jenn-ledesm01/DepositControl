using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace DatabaseVersionControl
{

    [Migration(8)]
    public class AddTokenColums : Migration
    {
        public override void Up()
        {

            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\AddTokenColums.sql"));
        }

        public override void Down()
        {
            //
        }
    }
}