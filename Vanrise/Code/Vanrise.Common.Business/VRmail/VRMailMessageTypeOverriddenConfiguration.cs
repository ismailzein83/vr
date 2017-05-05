using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRMailMessageTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("66A97FDC-156B-4F87-B12E-89B912D1E74A"); }
        }
        public Guid VRMailMessageTypeId { get; set; }

        public string OverriddenName { get; set; }

        public VRMailMessageTypeSettings OverriddenSettings { get; set; }
    }
}
