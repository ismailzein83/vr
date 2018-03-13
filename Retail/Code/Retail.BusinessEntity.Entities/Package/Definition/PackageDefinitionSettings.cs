using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId { get { return new Guid("ce9260a7-732f-4573-bef8-9a3f8fc7bcc6"); } }

        public Guid AccountBEDefinitionId { get; set; }

        public PackageDefinitionExtendedSettings ExtendedSettings { get; set; }
    }
}
