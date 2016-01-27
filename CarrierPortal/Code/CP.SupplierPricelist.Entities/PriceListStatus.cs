
using System.ComponentModel;

namespace CP.SupplierPricelist.Entities
{
    public enum PriceListStatus
    {
        [Description("New")]
        New = 0,

        [Description("Uploaded")]
        Uploaded = 1,

        [Description("Failed")]
        Failed = 2
    }

    public enum PriceListType
    {
        [Description("Full")]
        Full_Pricelist = 0,
        [Description("Country")]
        Country_Pricelist = 1,
        [Description("Rate Change")]
        Rate_Change_Pricelist = 2,
        [Description("Mixed_Full_Country_And_Changes")]
        Mixed_Full_Country_And_Changes = 3
    }
}
