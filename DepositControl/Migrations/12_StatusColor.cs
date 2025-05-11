using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace Bcri.Web.Migrations
{
    [Migration(12)]
    public class StatusColor : Migration
    {

        public override void Up()
        {
            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\StatusColor.sql"));
        }

        public override void Down()
        {
            //
        }
    }
  
}