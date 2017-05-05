using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class MeasuresController : BaseApiController
    {
        public MeasuresController(ICountingKsRepository repo) : base(repo)   
        {
        }

        public IEnumerable<MeasureModel> Get(int foodId)
        {
            var results = TheRepository.GetMeasuresForFood(foodId)
                .ToList()
                .Select(m => TheModelFactory.Create(m));

            return results;
        }

        public MeasureModel get(int foodid, int id)
        {
            var result = TheRepository.GetMeasure(id);

            if (result.Food.Id == foodid)
            {
                return TheModelFactory.Create(result);
            }

            return null;
        }
    }
}
