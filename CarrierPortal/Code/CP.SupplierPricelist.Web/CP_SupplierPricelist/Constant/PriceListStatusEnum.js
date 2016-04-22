app.constant('CP_SupplierPricelist_PriceListStatusEnum', {
    New: { value: 0, description: "New" },
    Uploaded: { value: 10, description: "Successfully Uploaded" },
    WaitingReview: { value: 20, description: "Waiting Review" },
    Completed: { value: 30, description: "Completed" },
    UploadFailedWithRetry: { value: 40, description: "Upload Failed With Retry" },
    ResultFailedWithRetry: { value: 50, description: "Result Failed With Retry" },
    UploadFailedWithNoRetry: { value: 60, description: "Upload Failed With NoRetry" },
    ResultFailedWithNoRetry: { value: 70, description: "Result Failed With NoRetry" },
    UnderProcessing: { value: 80, description: "Successfully Uploaded" }
});

app.constant('CP_SupplierPricelist_PriceListGroupedStatusEnum', {
    New: { value: 1,valuesEnum:[0], description: "New" },
    Uploaded: { value: 2, valuesEnum:[10], description: "Successfully Uploaded" },
    WaitingReview: { value: 3,valuesEnum:[20], description: "Waiting Review" },
    Completed: { value: 4,valuesEnum:[30], description: "Completed" },
    UploadFailedWithRetry: { value: 5, valuesEnum: [40,50], description: "Suspended" },
    ResultFailedWithNoRetry: { value: 6, valuesEnum: [60,70], description: "Aborted" },
    UnderProcessing: { value: 7, valuesEnum: [80], description: "Successfully Uploaded" }
});