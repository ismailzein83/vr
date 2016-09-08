using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultChanges
    {
        #region Routing Product

        public DraftNewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }
        public DraftChangedDefaultRoutingProduct DefaultRoutingProductChange { get; set; }

        #endregion

        #region Service

        public DraftNewDefaultService NewService { get; set; }
        public DraftClosedDefaultService ClosedService { get; set; }
        public DraftResetDefaultService ResetService { get; set; }
        
        #endregion
    }
}
