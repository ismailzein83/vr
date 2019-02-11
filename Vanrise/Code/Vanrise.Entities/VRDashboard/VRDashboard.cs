using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDashboard
    {
        public Guid VRDashboardId { get; set; }

        public string Name { get; set; }

        public VRTileReportSettings Settings { get; set; }
    }
}
