using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : PROTR.Web.Controllers.AuthenticationController
    {
        public AuthenticationController(IMemoryCache memoryCache) : base(memoryCache)
        {

        }
    }
}
