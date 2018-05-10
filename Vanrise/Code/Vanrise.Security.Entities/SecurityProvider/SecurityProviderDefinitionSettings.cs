using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class SecurityProviderDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("3D3D8A9E-B8A7-4C74-8AF1-FFAAC7A80900"); }
        }

        public SecurityProviderDefinitionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class SecurityProviderDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string RuntimeEditor { get; set; }
    }
}
