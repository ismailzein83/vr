using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid AccountBEDefinitionId { get; set; }

        public PackageDefinitionExtendedSettings ExtendedSettings { get; set; }
    }
}
