
using System.ComponentModel;

namespace CP.SupplierPricelist.Entities
{
    public enum PriceListStatus
    {
        [Description("New")]
        New = 0,
        [Description("Successfully Imported")]
        SuccessfullyImported = 10,
        WaitingReview = 20,
        Completed = 30,
        Suspended = 40,
        Failed= 50
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
