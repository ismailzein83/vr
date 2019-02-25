using System;

namespace Vanrise.Entities
{
    public class VRTile
    {
        public Guid VRTileId { get; set; }
        public string Name { get; set; }
        public bool ShowTitle { get; set; }
        public VRTileSettings Settings { get; set; }
        public bool AutoRefresh { get; set; }
        public int? AutoRefreshInterval { get; set; }
    }

}
