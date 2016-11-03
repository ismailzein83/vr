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
        public DraftNewDefaultRoutingProduct NewRoutingProduct { get; set; }
        public DraftChangedDefaultRoutingProduct ResetRoutingProduct { get; set; }
        #endregion

        #region Service
		public IEnumerable<int> CurrentServiceIds { get; set; }
		public IEnumerable<int> EffectiveServiceIds { get; set; }
        #endregion
    }
}
