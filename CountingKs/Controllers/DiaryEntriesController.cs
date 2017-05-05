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
    public class DiaryEntriesController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repo, ICountingKsIdentityService identityService) : base(repo)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryEntryModel> Get(DateTime diaryId)
        {
            var result = TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryId)
                                .ToList()
                                .Select(d => TheModelFactory.Create(d));

            return result;
        }

        public HttpResponseMessage Get(DateTime diaryId, int id)
        {
            var result = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryId, id);

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
                                
        }
    }
}
