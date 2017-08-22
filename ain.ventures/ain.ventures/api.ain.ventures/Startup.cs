using System;
using System.Configuration;
using System.Web.Http;

using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using Microsoft.Owin.Security.DataHandler.Encoder;
//using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;

using api.ain.ventures.App_Start;

[assembly: OwinStartup(typeof(api.ain.ventures.Startup))]

namespace api.ain.ventures
{
    /// <summary>
    /// Represents the entry point into an application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Specifies how the ASP.NET application will respond to individual HTTP request.
        /// </summary>
        /// <param name="app">Instance of <see cref="IAppBuilder"/>.</param>
        public void Configuration(IAppBuilder app)
        {
            // --------------------------------------------------------------------
            // -- The certificate bypass must be commented out in PRODUCTION!!!!!! --
            // --------------------------------------------------------------------
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            // --------------------------------------------------------------------

            CorsConfig.ConfigureCors(ConfigurationManager.AppSettings["cors"]);
            app.UseCors(CorsConfig.Options);

            ConfigureOAuth(app);

            var configuration = new HttpConfiguration();

            AutofacConfig.Configure(configuration);
            app.UseAutofacMiddleware(AutofacConfig.Container);

            FormatterConfig.Configure(configuration);
            RouteConfig.Configure(configuration);
            ServiceConfig.Configure(configuration);

            app.UseWebApi(configuration);
        }
        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/Token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}