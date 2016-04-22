app.constant('CP_SupplierPricelist_PriceListGroupedStatusEnum', {
    New: { value: 1,valuesEnum:[0], description: "New" },
    Uploaded: { value: 2, valuesEnum:[10], description: "Successfully Uploaded" },
    WaitingReview: { value: 3,valuesEnum:[20], description: "Waiting Review" },
    Completed: { value: 4,valuesEnum:[30], description: "Completed" },
    UploadFailedWithRetry: { value: 5, valuesEnum: [40,50], description: "Suspended" },
    ResultFailedWithNoRetry: { value: 6, valuesEnum: [60,70], description: "Aborted" },
    UnderProcessing: { value: 7, valuesEnum: [80], description: "Under Processing" }
});