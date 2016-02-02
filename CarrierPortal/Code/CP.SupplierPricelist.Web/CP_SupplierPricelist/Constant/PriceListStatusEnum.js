app.constant('CP_SupplierPricelist_PriceListStatusEnum', {
    New: { value: 0, description: "New" },
    Uploaded: { value: 10, description: "Uploaded" },
    Failed: { value: 20, description: "Failed" },
    WaitingReview: { value: 30, description: "WaitingReview" },
    GetStatusFailedWithRetry: { value: 40, description: "GetStatusFailedWithRetry" },
    Completed: { value: 50, description: "Completed" },
    Suspended: { value: 60, description: "Suspended" },
    GetStatusFailedWithNoRetry: { value: 70, description: "GetStatusFailedWithNoRetry" }
});