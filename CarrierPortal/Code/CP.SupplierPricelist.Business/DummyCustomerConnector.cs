using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Business
{
    public class DummyCustomerConnector : SupplierPriceListConnectorBase
    {
        public override PriceListUploadOutput PriceListUploadOutput(IPriceListUploadContext context)
        {
            throw new NotImplementedException();
        }

        public override PriceListProgressOutput GetPriceListProgressOutput(IPriceListProgressContext context)
        {
            throw new NotImplementedException();
        }

        public override List<SupplierInfo> GetSuppliers(GetSuppliersContext context)
        {
            return new List<SupplierInfo>
            {
                new SupplierInfo{ SupplierId = "C0024", SupplierName = "Supplier 1"},
                new SupplierInfo{ SupplierId = "Er345", SupplierName = "Supplier 2"},
                new SupplierInfo{ SupplierId = "sdgt", SupplierName = "Supplier 3"},
                new SupplierInfo{ SupplierId = "Ffrdsg", SupplierName = "Supplier 4"},
            };
        }
    }
}
