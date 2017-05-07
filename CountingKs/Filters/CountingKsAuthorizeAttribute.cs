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
using CountingKs.Data;
using Ninject;

namespace CountingKs.Filters
{
    public class CountingKsAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private bool _perUser;

        [Inject]
        public CountingKsRepository TheRepository { get; set; }

        public CountingKsAuthorizeAttribute(bool perUser = true)
        {
            _perUser = perUser;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            const string APIKEYNAME = "apikey";
            const string TOKENNAME = "token";

            var query = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);

            if (!string.IsNullOrWhiteSpace(query[APIKEYNAME]) && !string.IsNullOrWhiteSpace(query[TOKENNAME]))
            {
                var apiKey = query[APIKEYNAME];
                var token = query[TOKENNAME];

                var authToken = TheRepository.GetAuthToken(token);

                if (authToken != null && authToken.ApiUser.AppId == apiKey && authToken.Expiration > DateTime.UtcNow)
                {

                }

                if (_perUser)
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

                            if (WebSecurity.Login(userName, password))
                            {
                                var principle = new GenericPrincipal(new GenericIdentity(userName), null);
                                Thread.CurrentPrincipal = principle;

                                return;
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            HandleUnauthorized(actionContext);
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            if (_perUser)
            {
                actionContext.Response.Headers.Add("www-Authenticate",
                    "BASIC Scheme='CountingKs' Location='http://localhost:8901/Account/Register'");
            }
        }
    }
}