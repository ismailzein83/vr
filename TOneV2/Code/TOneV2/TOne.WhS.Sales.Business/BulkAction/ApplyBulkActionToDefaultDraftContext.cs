using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyBulkActionToDefaultDraftContext : IApplyBulkActionToDefaultDraftContext
    {
        #region Fields / Constructors

        private Func<SaleEntityZoneRoutingProduct> _getCustomerDefaultRoutingProduct;

        public ApplyBulkActionToDefaultDraftContext(Func<SaleEntityZoneRoutingProduct> getCustomerDefaultRoutingProduct)
        {
            _getCustomerDefaultRoutingProduct = getCustomerDefaultRoutingProduct;
        }

        #endregion

        public SaleEntityZoneRoutingProduct GetCustomerDefaultRoutingProduct()
        {
            return _getCustomerDefaultRoutingProduct();
        }

        public DefaultChanges DefaultDraft { get; set; }
    }
}
