using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class FilterGroupFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3C9143C3-0361-4D29-8D94-7C7C87F36B0E"); }
        }
        public override string RuntimeEditor
        {
            get { return "vr-genericdata-genericbe-filterruntime-filtergroup"; }
        }
        public List<string> AvailableFieldNames { get; set; }
    }

}
