app.constant('BlockedAttemptsMeasureEnum', {
    CustomerID: { value: 2, propertyName: "CustomerID", description: "Customer", isSum: true, type: "Text" },
    OurZoneID: { value: 3, propertyName: "OurZoneID", description: "Zone", isSum: true, type: "Text" },
    BlockAttempt: { value: 4, propertyName: "BlockAttempt", description: "Block Attempt", isSum: true, type: "Text" },
    ReleaseCode: { value: 5, propertyName: "ReleaseCode", description: "Rel. Code", isSum: true, type: "Text" },
    ReleaseSource: { value: 6, propertyName: "ReleaseSource", description: "ReleaseSource", isSum: true, type: "Text" },
    FirstCall: { value: 7, propertyName: "FirstCall", description: "First Call", isSum: true, type: "DateTime" },
    LastCall: { value: 8, propertyName: "LastCall", description: "Last Call", isSum: true, type: "DateTime" },
});