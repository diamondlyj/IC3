using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;

using AIn.Ventures.BaseLibrary;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace api.ain.ventures.App_Start
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            //context.SetError("invalid_grant", "The user name or password is incorrect.");
            //return;

            
            AIn.Ventures.BaseLibrary.AInVentureEntities entities = new AIn.Ventures.BaseLibrary.AInVentureEntities();
            IEnumerator<GetUser_ByEmail_Result> users = entities.GetUser_ByEmail(context.UserName).GetEnumerator();

            if (!users.MoveNext() || users.Current.Pwd == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            
            char[] lim = { ':' };
            string[] split = users.Current.Pwd.Split(lim);

            if (split.Length < 3)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            byte[] salt = Convert.FromBase64String(split[0]);
            int iter = int.Parse(split[1]);
            byte[] hash = Convert.FromBase64String(split[2]);

            Rfc2898DeriveBytes pbdkdf2 = new Rfc2898DeriveBytes(context.Password, salt, iter);
            byte[] b = pbdkdf2.GetBytes(20);

            int n = 0;

            while (n < 20)
            {
                if (b[n] != hash[n])
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                n++;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            
            identity.AddClaim(new Claim(ClaimTypes.Email, context.UserName) );
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
            identity.AddClaim(new Claim("GUID",users.Current.GUID.ToString()));

            context.Validated(identity);

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