using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core.REST
{
    public class ListModelFromClient
    {
        public string objectName { get; set; } = "";
        public string filterName { get; set; } = "";
        public int sortIndex { get; set; } = 0;
        public string sortDir { get; set; } = "";
        public bool dofastsearch { get; set; } = false;
        public string fastsearch { get; set; } = "";
        public Dictionary<string, string> filters { get; set; } = new Dictionary<string, string>();
        public int pageNumber { get; set; } = 1;
        public int rowsPerPage { get; set; } = 100;
        public int topRecords { get; set; } = 100;
        public bool first = true;
    }
}
