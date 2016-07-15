using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CBLSummerPortfolio07132016.Startup))]
namespace CBLSummerPortfolio07132016
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
