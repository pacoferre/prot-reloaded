using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core.REST
{
    public class SimpleListRequest
    {
        public string objectName { get; set; }
        public string listName { get; set; }
        public string parameter { get; set; }
    }
}
