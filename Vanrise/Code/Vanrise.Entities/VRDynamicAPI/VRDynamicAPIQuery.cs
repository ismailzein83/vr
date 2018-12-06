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
}
