app.constant('ReleaseCodeMeasureEnum', {
    RelCode: { value: 0, propertyName: "RelCode", description: "Rel. Code", type: "text" },
    ReleaseSource: { value: 1, propertyName: "ReleaseSource", description: "Release Source", type: "text" },
    FailedAttempts: { value: 2, propertyName: "FailedAttempts", description: "Failed Attempts", type: "Number" },
    Attempts: { value: 3, propertyName: "Attempts", description: "Attempts", type: "Number" },
    Percentage: { value: 4, propertyName: "Percentage", description: "%", type: "Number" },
    DurationInMinutes: { value: 5, propertyName: "DurationInMinutes", description: "(Minutes)", type: "Number" },
    FirstCall: { value: 6, propertyName: "FirstCall", description: "First Call", type: "Datetime" },
    LastCall: { value: 7, propertyName: "LastCall", description: "Last Call", type: "Datetime" }
});