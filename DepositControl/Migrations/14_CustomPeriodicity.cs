using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace Bcri.Web.Migrations
{
    [Migration(14)]
    public class CustomPeriodicity : Migration
    {

        public override void Up()
        {
            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\CustomPeriodicity.sql"));
        }

        public override void Down()
        {
            //
        }
    }
  
}