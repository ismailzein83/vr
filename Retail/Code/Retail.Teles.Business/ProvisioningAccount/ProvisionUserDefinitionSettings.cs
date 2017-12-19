using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business.Provisioning
{
    public class ProvisionUserDefinitionSettings : AccountProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9A63B2ED-A0B9-4364-AD6C-0977F410C1C4"); }
        }
        public Guid VRConnectionId { get; set; }
        public Guid CompanyTypeId { get; set; }
        public Guid SiteTypeId { get; set; }
        public Guid UserTypeId { get; set; }
        public string CountryCode { get; set; }
        public string LastNameField { get; set; }
        public string FirstNameField { get; set; }
        public string LoginNameField { get; set; }
        public string LoginPassword { get; set; }
        public string Pin { get; set; }

    }
    public class UserAccountSetting
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public string Pin { get; set; }
        public int? MaxRegistrations { get; set; }
        public int? MaxCalls { get; set; }
    }
}
