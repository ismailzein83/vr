using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business.Provisioning
{
    public class ProvisionSiteDefinitionSettings : AccountProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("91D8FBF2-A22E-46CB-A004-4966B5C1A87C"); }
        }
        public Guid VRConnectionId { get; set; }
        public string CountryCode { get; set; }
        public ProvisionSiteSetting Settings { get; set; }

    }
    public class ProvisionSiteSetting
    {
        public string CentrexFeatSet { get; set; }
        public SiteAccountSetting SiteAccountSetting { get; set; }
    }
}
