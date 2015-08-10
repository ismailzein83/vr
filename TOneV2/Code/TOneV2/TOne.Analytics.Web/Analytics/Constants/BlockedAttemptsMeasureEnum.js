app.constant('BlockedAttemptsMeasureEnum', {
    Customer: { value: 2, propertyName: "Customer", description: "Customer", isSum: true, type: "Text" },
    Zone: { value: 3, propertyName: "Zone", description: "Zone", isSum: true, type: "Text" },
    BlockAttempt: { value: 4, propertyName: "Block Attempt", description: "Block Attempt", isSum: true, type: "Text" },
    RelCode: { value: 5, propertyName: "RelCode", description: "Rel. Code", isSum: true, type: "Text" },
    ReleaseSource: { value: 6, propertyName: "ReleaseSource", description: "ReleaseSource", isSum: true, type: "Text" },
    FirstCall: { value: 7, propertyName: "First Call", description: "First Call", isSum: true, type: "DateTime" },
    LastCall: { value: 8, propertyName: "Last Call", description: "Last Call", isSum: true, type: "DateTime" },
});