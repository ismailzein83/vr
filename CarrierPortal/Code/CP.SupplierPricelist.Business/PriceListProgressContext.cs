
using CP.SupplierPricelist.Entities;

namespace CP.SupplierPricelist.Business
{
    public class PriceListProgressContext : IPriceListProgressContext
    {
        public object InitiateTestInformation { get; set; }

        public object RecentTestProgress { get; set; }

        public int QueueId { get; set; }
    }
}
