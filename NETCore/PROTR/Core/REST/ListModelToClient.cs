using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core.REST
{
    public class ListModelToClient
    {
        public string plural { get; set; }
        public BusinessObjectPermission permission = new BusinessObjectPermission();
        public List<object[]> result { get; set; }
        public string fastsearch { get; set; } = "";
        public int sortIndex { get; set; } = 1;
        public string sortDir { get; set; } = "a";
        public Dictionary<string, string> filters { get; set; } = null;
        public int topRecords { get; set; } = 100;
        public int pageNumber { get; set; } = 1;
        public int rowsPerPage { get; set; } = 100;
        public int rowCount { get; set; } = 0;
    }
}
