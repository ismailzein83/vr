
using CP.SupplierPricelist.Entities;

namespace CP.SupplierPricelist.Business
{
    public class PriceListProgressContext : IPriceListProgressContext
    {
        public object UploadInformation { get; set; }

        public object UploadProgress { get; set; }

    }
}
