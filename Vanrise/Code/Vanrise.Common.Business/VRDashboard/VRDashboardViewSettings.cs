using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRDashboardViewSettings : ViewSettings
    {
        public List<VRDashboardViewSettingsItem> DashboardDefinitionItems { get; set; }

        public override string GetURL(View view)
        {
            return String.Format("#/viewwithparams/Common/Views/VRDashboard/VRDashboardManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
    }

    public class VRDashboardViewSettingsItem
    {
        public int DashboardDefinitionId { get; set; }
    }
}
