using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;


namespace Vanrise.Common.MainExtensions.VRTile
{
    public class TabsTiles : VRTileExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("A3F635B1-2C76-4419-A636-0A6625048DF9"); } }

        public override string RuntimeEditor { get { return "vr-common-tabtiles-runtime"; } }

        public List<TabTile> TabTiles { get; set; } 
    }
}
