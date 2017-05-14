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
    public class MeasuresV2Controller : BaseApiController
    {
        public MeasuresV2Controller(ICountingKsRepository repo) : base(repo)   
        {
        }

        public IEnumerable<MeasureV2Model> Get(int foodId)
        {
            var results = TheRepository.GetMeasuresForFood(foodId)
                .ToList()
                .Select(m => TheModelFactory.Create2(m));

            return results;
        }

        public MeasureV2Model get(int foodid, int id)
        {
            var result = TheRepository.GetMeasure(id);

            if (result.Food.Id == foodid)
            {
                return TheModelFactory.Create2(result);
            }

            return null;
        }
    }
}
