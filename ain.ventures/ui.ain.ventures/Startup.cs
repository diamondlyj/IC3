using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AIn.Ventures.UI.Startup))]
namespace AIn.Ventures.UI
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
