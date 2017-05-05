using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRTileViewSettings : ViewSettings
    {
        public VRTileViewData VRTileViewData { get; set; }

        public override string GetURL(View view)
        {
            return String.Format("#/viewwithparams/Common/Views/VRTile/VRTileManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
    }
    public class VRTileViewData
    {
        public List<VRTile> VRTiles { get; set; }
    }
}
