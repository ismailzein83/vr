using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchSWSync : BaseIVSwitchSWSync
    {
        #region properties
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }
        public string Separator { get; set; }

        #endregion

        public override Guid ConfigId { get { return new Guid("64152327-5DB5-47AE-9569-23D38BCB18CC"); } }

        public override PreparedConfiguration GetPreparedConfiguration()
        {
            return PreparedConfiguration.GetCachedPreparedConfiguration(this);
        }
    }
    public class CarrierMapping
    {
        public string CarrierId { get; set; }

        public List<string> CustomerMapping { get; set; }

        public List<string> SupplierMapping { get; set; }

    }
}
