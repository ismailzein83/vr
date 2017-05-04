using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business.Provisioning
{
    public class ProvisionAccountDefinitionSettings : AccountProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FD6ED9B7-F870-4C6D-A51E-36FD2219F64B"); }
        }
        public Guid VRConnectionId { get; set; }
        public string CountryCode { get; set; }
        public ProvisionAccountSetting Settings { get; set; }

    }
    public class ProvisionAccountSetting
    {
        public string CentrexFeatSet { get; set; }
        public EnterpriseAccountSetting EnterpriseAccountSetting { get; set; }
        public SiteAccountSetting SiteAccountSetting { get; set; }
    }
    public class EnterpriseAccountSetting
    {
        public int EnterpriseMaxCalls { get; set; }
        public int EnterpriseMaxCallsPerUser { get; set; }
        public int EnterpriseMaxRegistrations { get; set; }
        public int EnterpriseMaxRegsPerUser { get; set; }
        public int EnterpriseMaxSubsPerUser { get; set; }
        public int EnterpriseMaxBusinessTrunkCalls { get; set; }
        public int EnterpriseMaxUsers { get; set; }
    }
    public class SiteAccountSetting
    {
        public int SiteMaxCalls { get; set; }
        public int SiteMaxCallsPerUser { get; set; }
        public int SiteMaxRegistrations { get; set; }
        public int SiteMaxRegsPerUser { get; set; }
        public int SiteMaxSubsPerUser { get; set; }
        public int SiteMaxBusinessTrunkCalls { get; set; }
        public int SiteMaxUsers { get; set; }
    }
}
