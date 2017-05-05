using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class DiariesController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiariesController(ICountingKsRepository repo, ICountingKsIdentityService identityService) : base(repo)
        {
            _identityService = identityService;
        }

        public IEnumerable<DiaryModel> Get()
        {
            var userName = _identityService.CurrentUser;

            var results = TheRepository.GetDiaries(userName)
                                .OrderByDescending(d => d.CurrentDate)
                                .Take(10)
                                .ToList()
                                .Select(d => TheModelFactory.Create(d));

            return results;
        }

        public HttpResponseMessage Get(DateTime diaryid)
        {
            var result = TheRepository.GetDiary(_identityService.CurrentUser, diaryid);

            if(result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse( HttpStatusCode.OK ,TheModelFactory.Create(result));
        }

        public HttpResponseMessage Post([FromBody] DiaryModel model)
        {
            try
            {
                Diary entity = TheModelFactory.Parse(model);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read diary body");
                }

                //Check if diary exists for the current user.
                if(TheRepository.GetDiaries(_identityService.CurrentUser).Count(d => d.CurrentDate == model.CurrentDate.Date) > 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate diaries not allowed");
                }

                entity.UserName = _identityService.CurrentUser;
                if (TheRepository.Insert(entity) && TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(entity));
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could Not Save to the Database");
                }

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.ToString());
            }
        }

        public HttpResponseMessage Delete(DateTime diaryid)
        {
            try
            {
                var diaryTodelete = TheRepository.GetDiary(_identityService.CurrentUser, diaryid);
                if (diaryTodelete == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (TheRepository.DeleteDiary(diaryTodelete.Id) && TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.ToString());
            }
        }
    }
}
