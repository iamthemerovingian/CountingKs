using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class DiarySummaryController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiarySummaryController(ICountingKsRepository repo, ICountingKsIdentityService identity) :base(repo)
        {
            _identityService = identity;
        }

        public object Get(DateTime diaryid)
        {
            try
            {
                var diary = TheRepository.GetDiary(_identityService.CurrentUser, diaryid);

                if(diary == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                return TheModelFactory.CreateSummary(diary);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.ToString());
            }
        }
    }
}
