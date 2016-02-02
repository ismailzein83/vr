using CP.SupplierPricelist.Entities;

namespace CP.SupplierPricelist.Business.PriceListTasks
{
    public class ResultTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public SupplierPriceListConnectorBase SupplierPriceListConnector { get; set; }

    }
}
