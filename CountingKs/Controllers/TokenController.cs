using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CountingKs.Controllers
{
    public class TokenController : BaseApiController
    {
        public TokenController(ICountingKsRepository repo) :base(repo)
        {

        }

        public HttpResponseMessage Post([FromBody] TokenRequestModel model)
        {
            try
            {
                var user = TheRepository.GetApiUsers().Where(u => u.AppId == model.ApiKey).FirstOrDefault();

                if (user != null)
                {
                    var secret = user.Secret;

                    var key = Convert.FromBase64String(secret);
                    var provider = new HMACSHA256(key);

                    var hash = provider.ComputeHash(Encoding.UTF8.GetBytes(user.AppId));
                    var signature = Convert.ToBase64String(hash);

                    if (signature == model.Signature)
                    {
                        //create a token
                        var rawTokenInfo = string.Concat(user.AppId + DateTime.UtcNow.ToString("d"));
                        var rawTokenBytes = Encoding.UTF8.GetBytes(rawTokenInfo);

                        var token = provider.ComputeHash(rawTokenBytes);
                        var authToken = new AuthToken()
                        {
                            Token = Convert.ToBase64String(token),
                            Expiration = DateTime.UtcNow.AddDays(7),
                            ApiUser = user
                        };

                        if (TheRepository.Insert(authToken) && TheRepository.SaveAll())
                        {
                            return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(authToken));
                        }
                    }

                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

    }
}
