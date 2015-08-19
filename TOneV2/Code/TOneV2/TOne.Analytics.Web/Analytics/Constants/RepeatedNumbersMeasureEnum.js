app.constant('RepeatedNumbersMeasureEnum', {
    CustomerID: { value: 2, propertyName: "CustomerInfo", description: "Customer", isSum: true, type: "Text", isShown: true },
   
    SupplierID: { value: 4, propertyName: "SupplierName", description: "Supplier", isSum: true, type: "Text", isShown: true },
    SwitchName: { value: 3, propertyName: "SwitchName", description: "Switch Name", isSum: true, type: "Text", isShown: false },
    ZoneID: { value: 5, propertyName: "OurZoneName", description: "Zone", isSum: true, type: "Text", isShown: true },
    Attempts: { value: 6, propertyName: "Attempts", description: "Attempts", isSum: true, type: "Number", isShown: true },
    ReleaseSource: { value: 7, propertyName: "DurationsInMinutes", description: "(Minutes)", isSum: true, type: "Number", isShown: true },
    PhoneNumber: { value: 8, propertyName: "PhoneNumber", description: "Phone Number", isSum: true, type: "Text", isShown: true },
});