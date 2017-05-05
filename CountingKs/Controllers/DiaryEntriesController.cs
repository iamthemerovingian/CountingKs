﻿using CountingKs.Data;
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

        public HttpResponseMessage Post(DateTime diaryid, [FromBody] DiaryEntryModel model)
        {
            try
            {
                var entity = TheModelFactory.Parse(model);

                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read diary entry in body");
                }

                var diary = TheRepository.GetDiary(_identityService.CurrentUser, diaryid);

                if (diary == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                //make sure it is not a duplicate.
                //checks if the diary entry that is going to be added to the repository already exists by comparing the measureId of the entity to the existing measureIds in the diary.
                if (diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate measures not allowed");
                }


                //saves the entry.
                diary.Entries.Add(entity);
                if(TheRepository.SaveAll())
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

        
    }
}
