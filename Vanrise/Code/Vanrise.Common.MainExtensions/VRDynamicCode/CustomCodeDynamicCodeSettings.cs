using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRDynamicCode
{
    public class CustomCodeDynamicCodeSettings : VRDynamicCodeSettings
    {
        public override Guid ConfigId { get { return new Guid("BDD289DF-573C-44A1-9A95-D0DE2ED9DD71"); } }
        public string CustomCode { get; set; }
        public override string Generate(IVRDynamicCodeSettingsContext context)
        {
            return CustomCode;
        }
    }
}
