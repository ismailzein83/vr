app.constant('CP_SupplierPricelist_PriceListStatusEnum', {
    New: { value: -1, description: "New" },
    Recieved: { value: 0, description: "Recieved" },
    Processing: { value: 1, description: "Processing" },
    SuspendedDueToBusinessErrors: { value: 2, description: "Suspended Due To Business Errors" },
    SuspendedToProcessingErrors: { value: 3, description: "Suspended Due To Processing Errors" },
    AwaitingWarningsConfirmation: { value: 4, description: "Awaiting Warnings Confirmation" },
    AwaitingSaveConfirmation: { value: 5, description: "Awaiting Save Confirmation" },
    WarningsConfirmed: { value: 6, description: "Warnings Confirmed" },
    SaveConfirmed: { value: 7, description: "Save Confirmed" },
    ProcessedSuccessfuly: { value: 8, description: "Processed Successfuly" },
    FailedDuetoSheetError: { value: 9, description: "Failed Due to Sheet Error" },
    Rejected: { value: 10, description: "Rejected" },
    SuspendedDueToConfigurationErrors: { value: 11, description: "Suspended Due To Configuration Errors" },
    ProcessedSuccessfulyByImport: { value: 12, description: "Processed Successfuly By Importer" },
    AwaitingSaveConfirmationbySystemparam: { value: 13, description: "Awaiting Save Confirmation by System param" },
    Processedwithnochanges: { value: 14, description: "Processed with no changes" }
});