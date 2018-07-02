app.constant("WhS_SupPL_ReceivedPricelistStatusEnum", {
    Received: { value: 0, description: "Received" },
    Processing: { value: 10, description: "Processing" },
    Succeeded: { value: 60, description: "Succeeded" },
    CompletedWithNoChanges: { value: 65, description: "Completed With No Changes" },
    FailedDueToBusinessRuleError: { value: 70, description: "Failed Due To Business Rule Error" },
    FailedDueToProcessingError: { value: 75, description: "Failed Due To Processing Error" },
    FailedDueToConfigurationError: { value: 80, description: "Failed Due To Configuration Error" },
    FailedDueToReceivedMailError: { value: 85, description: "Failed Due To Received Mail Error" },
    ImportedManually: { value: 66, description: "Imported Manually" },
});