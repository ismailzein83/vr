using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class RouteSyncBPInputArgument : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int RouteSyncDefinitionId { get; set; }

        public RouteSyncBPSettings Settings { get; set; }

        public override string ProcessName
        {
            get
            {
                var routeSyncBPDefinitionSettings = BEManagerFactory.GetManager<IRouteSyncDefinitionManager>().GetRouteSyncBPDefinitionSettings(this.RouteSyncDefinitionId);
                if (routeSyncBPDefinitionSettings == null)
                    throw new NullReferenceException(String.Format("routeSyncBPDefinitionSettings. RouteSyncDefinitionId '{0}'", this.RouteSyncDefinitionId));
                return String.Format("TOne_WhS_RouteSyncBPInputArgument_{0}", routeSyncBPDefinitionSettings.ConfigId);
            }
        }

        public override string GetTitle()
        {
            throw new NotImplementedException();
        }
    }
}
