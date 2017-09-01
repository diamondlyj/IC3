using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("Portal", typeof(Portal.Startup))]
namespace Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // --------------------------------------------------------------------
            // -- The certificate bypass must be commented out in PRODUCTION!!!!!! --
            // --------------------------------------------------------------------
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            // --------------------------------------------------------------------
            ConfigureAuth(app);
        }
    }
}
