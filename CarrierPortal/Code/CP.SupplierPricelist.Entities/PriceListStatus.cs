
using System.ComponentModel;

namespace CP.SupplierPricelist.Entities
{
    public enum PriceListStatus
    {
        [Description("New")]
        New = -1,

        [Description("Received")]
        Recieved = 0,

        [Description("Processing")]
        Processing = 1,

        [Description("Suspended Due To Business Errors")]
        SuspendedDueToBusinessErrors = 2,

        [Description("Suspended Due To Processing Errors")]
        SuspendedToProcessingErrors = 3,

        [Description("Awaiting Warnings Confirmation")]
        AwaitingWarningsConfirmation = 4,

        [Description("Awaiting Save Confirmation")]
        AwaitingSaveConfirmation = 5,

        [Description("Warnings Confirmed")]
        WarningsConfirmed = 6,

        [Description("Save Confirmed")]
        SaveConfirmed = 7,

        [Description("Processed Successfuly")]
        ProcessedSuccessfuly = 8,

        [Description("Failed due to Sheet Errors")]
        FailedDuetoSheetError = 9,

        [Description("Rejected")]
        Rejected = 10,

        [Description("Suspended Due To Configuration Errors")]
        SuspendedDueToConfigurationErrors = 11,

        [Description("ProcessedSuccessfulyByImport")]
        ProcessedSuccessfulyByImport = 12,

        [Description("Awaiting Save Confirmation by System param")]
        AwaitingSaveConfirmationbySystemparam = 13,

        [Description("Processed - No Changes")]
        Processedwithnochanges = 14

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
