
using System.ComponentModel;

namespace CP.SupplierPricelist.Entities
{
    public enum PriceListStatus
    {
        [Description("New")]
        New = 0,
        [Description("Successfully Uploaded")]
        SuccessfullyUploaded = 10,
        WaitingReview = 20,
        Completed = 30,
        [Description("Suspended")]
        UploadFailedWithRetry = 40,
        [Description("Suspended")]
        ResultFailedWithRetry = 50,

        [Description("Aborted")]
        UploadFailedWithNoRetry = 60,
        [Description("Aborted")]
        ResultFailedWithNoRetry = 70,
        [Description("Successfully Uploaded")]
        UnderProcessing = 80
    }

    public enum PriceListType
    {
        [Description("Full")]
        Full_Pricelist = 0,
        [Description("Country")]
        Country_Pricelist = 1,
        [Description("Rate Change")]
        Rate_Change_Pricelist = 2,
        [Description("Mixed")]
        Mixed_Full_Country_And_Changes = 3
    }
}
