using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Filters;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Routing;

namespace CountingKs.Controllers
{
    //[RequireHttps]
    [Authorize]
    public class FoodsController : BaseApiController
    {
        const int PAGE_SIZE = 50;
        public FoodsController(ICountingKsRepository repo) : base(repo)
        {           
        }

        public object Get(bool includeMeasures = true, int page = 0)
        {
            IQueryable<Food> query;
            var repo = new CountingKsRepository(new CountingKsContext());
            
            if (includeMeasures)
            {
                query = TheRepository.GetAllFoodsWithMeasures();
            }
            else
            {
                query = TheRepository.GetAllFoods();
            }

            var baseQuery = query.OrderBy(f => f.Description);

            var totalCount = baseQuery.Count();
            var totalPages = Math.Ceiling((double)totalCount / PAGE_SIZE);

            var helper = new UrlHelper(Request);
            var prevUrl = page > 0 ? helper.Link("Food", new { page = page - 1}) : "" ;
            var nextUrl = page < totalPages - 1 ? helper.Link("Food", new { page = page + 1}) : "";

            var results  = baseQuery.Skip(PAGE_SIZE*page)
                                    .ToList()
                                    .Select(f => TheModelFactory.Create(f));



            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PrevUrl = prevUrl,
                NextUrl = nextUrl,
                Results = results
            };
        }

        public FoodModel Get(int foodid)
        {
            return TheModelFactory.Create(TheRepository.GetFood(foodid));
        }
    }
}
