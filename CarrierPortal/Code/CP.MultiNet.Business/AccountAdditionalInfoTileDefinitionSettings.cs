using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CP.MultiNet.Business
{
    public class AccountAdditionalInfoTileDefinitionSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("DA5653FC-7305-4AC8-B64C-1EB9B253D44B"); }
        }

        public override string RuntimeEditor
        {
            get { return "cp-multinet-accountadditionalinfotileruntimesettings"; }
        }
        public Guid VRConnectionId { get; set; }
    }
}
