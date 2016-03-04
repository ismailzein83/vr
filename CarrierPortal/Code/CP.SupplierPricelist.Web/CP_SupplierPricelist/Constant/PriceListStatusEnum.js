app.constant('CP_SupplierPricelist_PriceListStatusEnum', {
    New: { value: 0, description: "New" },
    Uploaded: { value: 10, description: "Successfully Uploaded" },
    WaitingReview: { value: 20, description: "Waiting Review" },
    Completed: { value: 30, description: "Completed" },
    UploadFailedWithRetry: { value: 40, description: "UploadFailedWithRetry" },
    ResultFailedWithRetry: { value: 50, description: "ResultFailedWithRetry" },
    UploadFailedWithNoRetry: { value: 60, description: "UploadFailedWithNoRetry" },
    ResultFailedWithNoRetry: { value: 70, description: "ResultFailedWithNoRetry" },
    UnderProcessing: { value: 80, description: "Successfully Uploaded" }
});