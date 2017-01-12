using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("44F7D357-CD66-4397-A159-7A597A8C1164"); }
        }

        public Guid AccountBEDefinitionId { get; set; }

        public ProductDefinitionExtendedSettings ExtendedSettings { get; set; }
    }
}
