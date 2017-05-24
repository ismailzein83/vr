using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Business
{
    public class LiveBalanceTileDefinitionSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("48FA768C-7482-476D-9DB0-26C6A0CEB9A0"); }
        }

        public override string RuntimeEditor
        {
            get { return "partnerportal-customeraccess-livebalancetileruntimesettings"; }
        }
        public Guid VRConnectionId { get; set; }
        public Guid AccountTypeId { get; set; }
        public Guid? ViewId { get; set; }
    }
}
