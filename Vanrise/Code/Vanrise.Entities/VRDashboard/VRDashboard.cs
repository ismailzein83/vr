using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDashboard
    {
        public int VRDashboardId { get; set; }
        public string Name { get; set; }
        public VRTileSettings Settings { get; set; }
    }
}
