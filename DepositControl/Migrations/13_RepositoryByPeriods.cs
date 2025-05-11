using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentMigrator;
using System.Web.Hosting;

namespace Bcri.Web.Migrations
{
    [Migration(13)]
    public class RepositoryByPeriods : Migration
    {

        public override void Up()
        {
            Execute.Script(HostingEnvironment.MapPath(@"~\Migrations\RepositoryByPeriods.sql"));
        }

        public override void Down()
        {
            //
        }
    }
  
}