using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectTypeDefinitionOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E1CE89E4-178E-4BB9-9AF2-0163B2712C75"); }
        }

        public Guid VRObjectTypeDefinitionId { get; set; }

        public string OverriddenName { get; set; }

        public VRObjectTypeDefinitionSettings OverriddenSettings { get; set; }
    }
}
