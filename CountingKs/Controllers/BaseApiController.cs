using CountingKs.Data;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private ICountingKsRepository _repo;
        private ModelFactory _ModelFactory;

        public BaseApiController(ICountingKsRepository repo)
        {
            _repo = repo;
        }

        protected ICountingKsRepository TheRepository
        {
            get
            {
                return _repo;
            }
        }

        protected  ModelFactory TheModelFactory
        {
            get
            {
                if(_ModelFactory == null)
                {
                    _ModelFactory = new ModelFactory(this.Request, _repo);
                }
                return _ModelFactory;
            }
        }
    }
}
