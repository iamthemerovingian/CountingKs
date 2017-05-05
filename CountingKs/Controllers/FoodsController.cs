using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class FoodsController : BaseApiController
    {
        public FoodsController(ICountingKsRepository repo) : base(repo)
        {           
        }

        public IEnumerable<FoodModel> Get(bool includeMeasures = true)
        {
            IQueryable<Food> query;
            var repo = new CountingKsRepository(new CountingKsContext());

            //var results = repo.GetAllFoods()
            //                .OrderBy(f => f.Description)
            //                .Take(25)
            //                .ToList();

            if (includeMeasures)
            {
                query = TheRepository.GetAllFoodsWithMeasures();
            }
            else
            {
                query = TheRepository.GetAllFoods();
            }

             var results =     query.OrderBy(f => f.Description)
                                    .Take(25)
                                    .ToList()
                                    .Select(f => TheModelFactory.Create(f));

            return results;
        }

        public FoodModel Get(int foodid)
        {
            return TheModelFactory.Create(TheRepository.GetFood(foodid));
        }
    }
}
