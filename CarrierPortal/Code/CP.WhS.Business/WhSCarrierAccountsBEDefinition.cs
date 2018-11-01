using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace CP.WhS.Business
{
    public  class WhSCarrierAccountsBEDefinition : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("1D04AAD6-B9DB-4608-8635-470568A4D193");
        public override Guid ConfigId { get { return s_configId; } }

        public override string DefinitionEditor { get { return "cp-whs-carrieraccountsbedefinition-editor"; } }

        public override string IdType { get { return "System.Int32"; } }

        public override string SelectorUIControl { get { return "cp-whs-carrieraccounts-selector"; } }

        public override string ManagerFQTN { get { return "CP.WhS.Business.WhSCarrierAccountBEManager, CP.WhS.Business"; } }

        public bool GetCustomers { get; set; }
        public bool GetSuppliers { get; set; }
        public Guid VRConnectionId { get; set; }
    }
}
