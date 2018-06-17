using System;
using System.Collections.Generic;
using System.Text;

namespace PROTR.Core.REST
{
    public class ListModelDefinitionRequest
    {
        public string objectName { get; set; } = "";
        public string filterName { get; set; } = "";
    }
}
