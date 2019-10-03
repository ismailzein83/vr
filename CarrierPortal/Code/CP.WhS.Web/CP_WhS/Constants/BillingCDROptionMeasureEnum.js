﻿app.constant('CP_WhS_BillingCDROptionMeasureEnum', {
    All: { value: 0, propertyName: "All", description: "All", cdrSourceName: "AllCDRs" },
    Failed: { value: 2, propertyName: "Failed", description: "Failed", cdrSourceName: "FailedCDRs" },
    Invalid: { value: 3, propertyName: "Invalid", description: "Invalid", cdrSourceName: "InvalidCDRs" },
    Successful: { value: 1, propertyName: "Successful", description: "Successful", cdrSourceName: "MainCDRs" },
    PartialPriced: { value: 4, propertyName: "PartialPriced", description: "Partial Priced", cdrSourceName: "PartialPricedCDRs" }
});