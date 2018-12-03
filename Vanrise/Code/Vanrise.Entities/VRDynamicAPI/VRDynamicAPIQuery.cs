using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPIQuery
    {
        public string Name { get; set; }
        public int? VRDynamicAPIModuleId { get; set; }
    }



    public class VRDynamicCode
    {
        public Guid VRDynamicCodeId { get; set; }
        public string Title { get; set; }
        public VRDynamicCodeSettings Settings { get; set; }
    }
    public abstract class VRDynamicCodeSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string Generate(IVRDynamicCodeSettingsContext context);
     }
    public interface IVRDynamicCodeSettingsContext
    {

    }
    public class VRDynamicCodeSettingsContext: IVRDynamicCodeSettingsContext
    {

    }
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
