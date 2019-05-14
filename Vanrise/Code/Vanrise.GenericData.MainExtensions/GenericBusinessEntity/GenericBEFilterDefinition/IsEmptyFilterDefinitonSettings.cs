using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions
{
    public class IsEmptyFilterDefinitonSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("177D3C5C-4A3D-4B3E-B63C-60B1021C0C40"); } }
        public override string RuntimeEditor { get { return "vr-genericdata-genericbe-filterruntime-isempty"; }  }
        public bool IsRequired { get; set; }
        public string FieldName { get; set; }
        public IsEmptyFilterDefinitonSettingsField AllField { get; set; }
        public IsEmptyFilterDefinitonSettingsField NullField { get; set; }
        public IsEmptyFilterDefinitonSettingsField NotNullField { get; set; }
    }

    public class IsEmptyFilterDefinitonSettingsField
    {
        public string Title { get; set; }
        public string Resourcekey { get; set; }
        public bool IsDefault { get; set; }
    }
}
