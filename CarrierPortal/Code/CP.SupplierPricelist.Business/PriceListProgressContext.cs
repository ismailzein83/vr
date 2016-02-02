
using CP.SupplierPricelist.Entities;

namespace CP.SupplierPricelist.Business
{
    public class PriceListProgressContext : IPriceListProgressContext
    {
        public object InitiateTestInformation { get; set; }

        public object RecentTestProgress { get; set; }

        public int QueueId { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
