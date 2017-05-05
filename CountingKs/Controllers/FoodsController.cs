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
    public class FoodsController : ApiController
    {
        private ICountingKsRepository _repo;
        private ModelFactory _ModelFactory;

        public FoodsController(ICountingKsRepository repo)
        {
            _repo = repo;
            _ModelFactory = new ModelFactory();
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
                query = repo.GetAllFoodsWithMeasures();
            }
            else
            {
                query = repo.GetAllFoods();
            }

             var results =     query.OrderBy(f => f.Description)
                                    .Take(25)
                                    .ToList()
                                    .Select(f => _ModelFactory.Create(f));

            return results;
        }

        public FoodModel Get(int foodid)
        {
            return _ModelFactory.Create(_repo.GetFood(foodid));
        }
    }
}
