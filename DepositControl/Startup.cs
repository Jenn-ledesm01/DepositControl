using System.Configuration;
using System.Reflection;
using DepositControl;
using DNF.Release.Bussines;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace DepositControl
{
    public partial class Startup
    {


        public void Configuration(IAppBuilder app)
        {

        }
    }
}

