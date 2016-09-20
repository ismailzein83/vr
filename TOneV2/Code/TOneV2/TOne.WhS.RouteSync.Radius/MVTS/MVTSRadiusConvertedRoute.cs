using System.Collections.Generic;
using System.Linq;

namespace TOne.WhS.RouteSync.Radius
{
    public class MVTSRadiusConvertedRoute : RadiusConvertedRoute
    {
        public List<MVTSRadiusOption> MVTSRadiusOptions { get; set; }

        public bool HasPercentage
        {
            get
            {
                if (this.MVTSRadiusOptions == null || this.MVTSRadiusOptions.Count == 0)
                    return false;

                return this.MVTSRadiusOptions.FirstOrDefault(itm => itm.Percentage.HasValue) != null;
            }
        }
    }

    public class MVTSRadiusOption
    {
        public string Option { get; set; }
        public int? Percentage { get; set; }
        public int? Priority { get; set; }
    }
}