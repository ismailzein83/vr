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
        #region Current Routing Product Properties

        public bool? IsCurrentRoutingProductEditable { get; set; }
        public int? CurrentRoutingProductId { get; set; }
        public string CurrentRoutingProductName { get; set; }
        public DateTime? CurrentRoutingProductBED { get; set; }
        public DateTime? CurrentRoutingProductEED { get; set; }
        
        #endregion
        
        #region New Routing Product Properties
        
        public int? NewRoutingProductId { get; set; }
        public DateTime? NewRoutingProductBED { get; set; }
        public DateTime? NewRoutingProductEED { get; set; }
        
        #endregion
    }
}
