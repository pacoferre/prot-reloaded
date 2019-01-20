using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROTR.Core.REST
{
    public class ModelFromClient
    {
        public string objectName { get; set; } = "";
        public string formToken { get; set; } = "";
        public int sequence { get; set; } = 0;
        public string action { get; set; } = "";

        public List<string> dataNames { get; set; }
        public ModelFromClientData root { get; set; }

        public void Sanitize()
        {
            if (dataNames != null)
            {
                for (var index = 0; index < dataNames.Count; ++index)
                {
                    dataNames[index] = dataNames[index].ToTitleCase();
                }
            }

            if (root != null)
            {
                root.Sanitize();
            }
        }
    }

    public class ModelFromClientData
    {
        public string key { get; set; } = "";
        public Dictionary<string, string> data { get; set; }
        public List<ModelFromClientCollection> children { get; set; }

        public void Sanitize()
        {
            if (data != null)
            {
                data = data.ToDictionary(_ => _.Key.ToTitleCase(), _ => _.Value);
            }

            if (children != null)
            {
                children.ForEach(_ => _.Sanitize());
            }
        }
    }

    public class ModelFromClientCollection
    {
        public string path { get; set; } = "";
        public List<string> dataNames { get; set; }
        public List<ModelFromClientData> elements { get; set; }

        public void Sanitize()
        {
            for (var index = 0; index < dataNames.Count; ++index)
            {
                dataNames[index] = dataNames[index].ToTitleCase();
            }

            elements.ForEach(_ => _.Sanitize());
        }
    }
}

