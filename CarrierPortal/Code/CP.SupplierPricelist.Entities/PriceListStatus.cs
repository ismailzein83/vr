
using System.ComponentModel;

namespace CP.SupplierPricelist.Entities
{
    public enum PriceListStatus
    {
        [Description("New")]
        New = 1,

        [Description("Initiated")]
        Initiated = 10,

        [Description("Failed")]
        Failed = 20,

        [Description("Completed")]
        Completed = 50

    }
}
