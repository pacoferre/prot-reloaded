using PROTR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PROTR.Web.Dtos
{
    public class PropertyDefinitionDto
    {
        public PropertyInputType Type { get; set; }
        public bool IsIdentity { get; set; } = false;
        public bool IsReadOnly { get; set; } = false;
        public bool IsOnlyOnNew { get; set; } = false;
        public string FieldName { get; } = "";
        public string Label { get; set; } = "";
        public bool LabelIsFieldName { get; set; } = false;
        public string ClientFormat { get; set; } = "";
        public string Pattern { get; set; } = "";
        public int MaxLength { get; set; } = 0;
        public bool Required { get; set; } = false;
        public string RequiredErrorMessage { get; set; } = "";
        public bool NoLabelRequired { get; set; } = false;
        public bool NoChecking { get; set; } = false;
        public string Min { get; set; } = "";
        public string Max { get; set; } = "";
        public string Step { get; set; } = "";
        public string ListObjectName { get; set; } = "";
        public string ListName { get; set; } = "";
        public bool IsObjectView { get; set; } = false;
        public bool ListAjax { get; set; } = false;
        public int Rows { get; set; } = 0;
        public string DefaultSearch { get; set; } = "";
        public bool SearchMultipleSelect { get; set; } = true;
        public bool AlwaysFloatLabel { get; set; } = true;

        public PropertyDefinitionDto(PropertyDefinition property)
        {
            Type = property.Type;
            IsIdentity = property.IsIdentity;
            IsReadOnly = property.IsReadOnly || property.IsIdentity;
            IsOnlyOnNew = property.IsOnlyOnNew;
            FieldName = property.FieldName;
            Label = property.Label;
            ClientFormat = property.ClientFormat;
            Pattern = property.Pattern;
            MaxLength = property.MaxLength;
            Required = property.Required;
            RequiredErrorMessage = property.RequiredErrorMessage;
            NoLabelRequired = property.NoLabelRequired;
            NoChecking = property.NoChecking;
            Min = property.Min;
            Max = property.Max;
            Step = property.Step;
            ListObjectName = property.ListObjectName;
            ListName = property.ListName;
            IsObjectView = property.SetModified;
            ListAjax = property.SetModified;
            Rows = property.Rows;
            DefaultSearch = property.DefaultSearch;
            SearchMultipleSelect = property.SearchMultipleSelect;
            AlwaysFloatLabel = property.AlwaysFloatLabel;
        }
}
}
