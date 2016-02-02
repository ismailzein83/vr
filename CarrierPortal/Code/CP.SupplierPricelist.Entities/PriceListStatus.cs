
using System.ComponentModel;

namespace CP.SupplierPricelist.Entities
{
    public enum PriceListStatus
    {
        [Description("New")]
        New = 0,

        [Description("Uploaded")]
        Uploaded = 10,

        [Description("Failed")]
        UploadFailedWithRetry = 20,
        WaitingReview = 30,
        GetStatusFailedWithRetry = 40,

        Completed = 50,

        [Description("Suspended")]
        UploadFailedWithNoRetry = 60,

        [Description("Suspended")]
        GetStatusFailedWithNoRetry = 70
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
