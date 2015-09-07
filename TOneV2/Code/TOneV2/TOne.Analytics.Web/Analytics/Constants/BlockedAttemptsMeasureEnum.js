app.constant('BlockedAttemptsMeasureEnum', {
    CustomerID: { value: 2, propertyName: "CustomerName", description: "Customer", isSum: true, type: "Text",isShown:true },
    OurZoneID: { value: 3, propertyName: "OurZoneName", description: "Zone", isSum: true, type: "Text", isShown: true },
    BlockAttempt: { value: 4, propertyName: "BlockAttempt", description: "Block Attempt", isSum: true, type: "Text", isShown: true },
    CLI: { value: 5, propertyName: "CLI", description: "CLI", isSum: true, type: "Text", isShown: false },
    PhoneNumber: { value: 6, propertyName: "PhoneNumber", description: "PhoneNumber", isSum: true, type: "Text", isShown: false },
    ReleaseCode: { value: 7, propertyName: "ReleaseCode", description: "Rel. Code", isSum: true, type: "Text", isShown: true },
    ReleaseSource: { value: 8, propertyName: "ReleaseSource", description: "ReleaseSource", isSum: true, type: "Text", isShown: true },
    FirstCall: { value: 9, propertyName: "FirstCall", description: "First Call", isSum: true, type: "'DateTime'", isShown: true },
    LastCall: { value: 10, propertyName: "LastCall", description: "Last Call", isSum: true, type: "'DateTime'", isShown: true },
});