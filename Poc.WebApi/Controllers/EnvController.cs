using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Poc.WebApi.Models;

namespace Poc.WebApi.Controllers
{
    public class EnvController : ApiController
    {
        Env[] env = new Env[] 
        { 
            new Env { Name = ConfigurationManager.AppSettings["Environment"] }
        };

        public IEnumerable<Env> GetEnv()
        {
            return env;
        }
    }
}
