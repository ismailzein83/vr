using CP.SupplierPricelist.Entities;

namespace CP.SupplierPricelist.Business.PriceListTasks
{
    public class UploadPriceListTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public int CustomerId { get; set; }
        public int MaximumRetryCount { get; set; }
    }
}
