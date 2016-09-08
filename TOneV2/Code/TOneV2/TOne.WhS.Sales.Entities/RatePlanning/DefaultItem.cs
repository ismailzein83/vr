using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultItem
    {
        #region Routing Product
        public int? CurrentRoutingProductId { get; set; }
        public string CurrentRoutingProductName { get; set; }
        public DateTime? CurrentRoutingProductBED { get; set; }
        public DateTime? CurrentRoutingProductEED { get; set; }
        public bool? IsCurrentRoutingProductEditable { get; set; }
        #region New
        public DraftNewDefaultRoutingProduct NewRoutingProduct { get; set; }
        public DraftChangedDefaultRoutingProduct ChangedRoutingProduct { get; set; }
        #endregion
        public int? NewRoutingProductId { get; set; }
        public string NewRoutingProductName { get; set; }
        public DateTime? NewRoutingProductBED { get; set; }
        public DateTime? NewRoutingProductEED { get; set; }
        public DateTime? RoutingProductChangeEED { get; set; }
        #endregion

        #region Service
        public List<ZoneService> CurrentServices { get; set; }
        public DateTime? CurrentServiceBED { get; set; }
        public DateTime? CurrentServiceEED { get; set; }
        public bool? IsCurrentServiceEditable { get; set; }
        public DraftNewDefaultService NewService { get; set; }
        public DraftClosedDefaultService ClosedService { get; set; }
        public DraftResetDefaultService ResetService { get; set; }
        #endregion
    }
}
