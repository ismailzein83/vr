using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailUserSubaccountsBEDefinition : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("CBF7EF73-0D80-43A6-ADC6-EDC037542165");
        public override Guid ConfigId { get { return s_configId; } }
        public override string DefinitionEditor
        {
            get { return "partnerportal-customeraccess-retailusersubaccountsdefinition-editor"; }
        }
        public override string IdType
        {
            get { return "System.Int64"; }
        }
        public override string SelectorUIControl {
            get { return "partnerportal-customeraccess-retailsubaccounts-selector"; }
        }
        public override string ManagerFQTN
        {
            get { return "PartnerPortal.CustomerAccess.Business.RetailAccountUserManager, PartnerPortal.CustomerAccess.Business"; }
        }
        public Guid VRConnectionId { get; set; }
        public List<Guid> AccountTypeIds { get; set; }
    }
}
