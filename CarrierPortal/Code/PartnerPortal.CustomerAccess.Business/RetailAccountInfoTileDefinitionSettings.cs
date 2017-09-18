using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class RetailAccountInfoTileDefinitionSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FF8E7752-9DF7-4017-83AB-6FA8A9CDB30F"); }
        }

        public override string RuntimeEditor
        {
            get { return "partnerportal-customeraccess-retailaccountinfotileruntimesettings"; }
        }
        public Guid VRConnectionId { get; set; }
    }
}
