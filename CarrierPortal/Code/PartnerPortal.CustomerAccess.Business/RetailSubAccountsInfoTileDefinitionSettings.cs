using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailSubAccountsInfoTileDefinitionSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("138B9459-67A5-4031-AEA7-D86093F731D5"); }
        }

        public override string RuntimeEditor
        {
            get { return "partnerportal-customeraccess-retailsubaccountsinfotileruntimesettings"; }
        }
        public Guid VRConnectionId { get; set; }
        public List<AccountGridField> AccountGridFields { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
    }
    
}
