using CountingKs.Data;
using CountingKs.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class FoodsController : ApiController
    {
        private ICountingKsRepository _repo;

        public FoodsController(ICountingKsRepository repo)
        {
            _repo = repo;
        }
        public IEnumerable<Food> Get()
        {
            var repo = new CountingKsRepository(new CountingKsContext());

            //var results = repo.GetAllFoods()
            //                .OrderBy(f => f.Description)
            //                .Take(25)
            //                .ToList();

            var results = repo.GetAllFoodsWithMeasures()
                 .OrderBy(f => f.Description)
                 .Take(25)
                 .ToList();

            return results;
        }
    }
}
