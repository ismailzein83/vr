using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class DIDTileDefinitionSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3EFAF2E0-BCD2-4C98-890E-6EC5B4E4DD10"); }
        }

        public override string RuntimeEditor
        {
            get { return "partnerportal-customeraccess-didtileruntimesettings"; }
        }
        public Guid VRConnectionId { get; set; }
        public bool WithSubAccounts { get; set; }
    }
}
