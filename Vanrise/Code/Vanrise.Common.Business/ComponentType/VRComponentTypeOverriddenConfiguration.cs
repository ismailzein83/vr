using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRComponentTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6C7BA6EC-1FC6-45E7-BF8D-5F8074DF98E0"); }
        }

        public Guid VRComponentTypeId { get; set; }

        public string OverriddenName { get; set; }

        public VRComponentTypeSettings OverriddenSettings { get; set; }
    }
}
