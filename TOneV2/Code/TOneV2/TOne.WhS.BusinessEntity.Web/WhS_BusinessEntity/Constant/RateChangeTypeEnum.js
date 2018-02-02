app.constant("WhS_BE_RateChangeTypeEnum", {
    NotChanged: { value: 0, description: "Not Changed", iconType: null, iconUrl: null },
    New: { value: 1, description: "New", iconType: null, iconUrl: "Client/Modules/WhS_BusinessEntity/Images/New.png" },
    Deleted: { value: 2, description: "Deleted", iconType: null, iconUrl: "Client/Modules/WhS_BusinessEntity/Images/Closed.png" },
    Increase: { value: 3, description: "Increase", iconType: 'increase', iconUrl: "Client/Modules/WhS_BusinessEntity/Images/Increase.png" },
    Decrease: { value: 4, description: "Decrease", iconType: 'decrease', iconUrl: "Client/Modules/WhS_BusinessEntity/Images/Decrease.png" }
});

app.constant("WhS_BE_SubscriberStatusEnum", {
    Success: { value: 0, description: "Success" },
    NoChange: { value: 1, description: "No Change" },
    Failed: { value: 2, description: "Failed" }
});