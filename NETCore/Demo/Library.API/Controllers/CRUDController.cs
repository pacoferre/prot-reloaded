using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CRUDController : PROTR.Web.Controllers.CRUDController
    {
        public CRUDController(IMemoryCache memoryCache) : base(memoryCache)
        {

        }
    }
}
