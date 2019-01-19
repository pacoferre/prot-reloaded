using Newtonsoft.Json;

namespace PROTR.Core.DataViews
{
    public class DataViewColumn
    {
        public BasicType BasicType { get; set; } = BasicType.Text;
        public bool IsID { get; set; } = false;
        public HorizontalAlign Align { get; set; } = HorizontalAlign.Left;
        public string Label { get; set; } = "";
        public string As { get; set; } = "";
        public string Format { get; set; } = "";
        public bool Hidden { get; set; } = false;
        public bool Hideable { get; set; } = false;
        public bool Resizable { get; set; } = false;
        public bool Money { get; set; } = false;
        public int MinWidth { get; set; } = 0;
        public int MaxWidth { get; set; } = 0;
        public int Width { get; set; } = 0;
        public int Flex { get; set; } = 0;
        //        public string CustomRenderer { get; set; } = "";
        [JsonIgnore]
        public bool Visible { get; set; } = true;
        [JsonIgnore]
        public string OrderBy { get; set; } = "";
        [JsonIgnore]
        public string Expression { get; set; } = "";
        [JsonIgnore]
        public bool FastSearchColumn { get; set; } = false;

        public DataViewColumn(string tableNameEncapsulated, PropertyDefinition property)
        {
            BasicType = property.BasicType;
            IsID = property.IsPrimaryKey;
            Hidden = property.IsPrimaryKey;
            Expression = tableNameEncapsulated + "." + property.FieldName;
            Label = property.Label;

            if (property.BasicType == BasicType.Number)
            {
                Align = HorizontalAlign.Right;
            }

            if (!Hidden)
            {
                OrderBy = tableNameEncapsulated + "." + property.FieldName;
            }

            if (property.BasicType == BasicType.Text)
            {
                FastSearchColumn = true;
            }
        }

        public DataViewColumn(string expression, string label)
        {
            Expression = expression;
            Label = label;
            OrderBy = expression;
        }
    }
}
