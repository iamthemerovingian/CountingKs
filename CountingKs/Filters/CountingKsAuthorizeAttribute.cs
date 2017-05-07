using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using System.Text;
using WebMatrix.WebData;
using System.Security.Principal;
using System.Threading;

namespace CountingKs.Filters
{
    public class CountingKsAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return;
            }

            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    var rawCredentials = authHeader.Parameter;
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
                    var split = credentials.Split(':');
                    var userName = split[0];
                    var password = split[1];

                    if (!WebSecurity.Initialized)
                    {
                        WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                    }

                    if (WebSecurity.Login(userName,password))
                    {
                        var principle = new GenericPrincipal(new GenericIdentity(userName), null);
                        Thread.CurrentPrincipal = principle;

                        return;
                    }


                }
            }
            HandleUnauthorized(actionContext);
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("www-Authenticate",
                "BASIC Scheme='CountingKs' Location='http://localhost:8901/Account/Register'");
        }
    }
}