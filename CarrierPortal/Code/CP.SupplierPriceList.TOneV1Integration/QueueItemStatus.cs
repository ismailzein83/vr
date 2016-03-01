using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public enum QueueItemStatus : int
    {
        Recieved = 0,

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
        Processedwithnochanges = 14,

        [Description("Partially Rejected")]
        PartiallyRejected = 15

    }
}
