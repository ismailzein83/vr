using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6D005236-ECE6-43A1-B8EA-281BC0E7643E"); }
        }

        public override string RuntimeEditor
        {
            get { return "vr-genericdata-genericbe-filterruntime-generic"; }
        }
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public bool IsRequired { get; set; }

    }
}
