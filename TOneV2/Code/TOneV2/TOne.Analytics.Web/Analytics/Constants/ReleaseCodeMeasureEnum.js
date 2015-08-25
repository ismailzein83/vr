app.constant('ReleaseCodeMeasureEnum', {
    RelCode: { value: 0, propertyName: "RelCode", description: "Rel. Code", type: "text", fieldName: "Data.ReleaseCode" },
    ReleaseSource: { value: 1, propertyName: "ReleaseSource", description: "Release Source", type: "text", fieldName: "Data.ReleaseSource" },
    FailedAttempts: { value: 2, propertyName: "FailedAttempts", description: "Failed Attempts", type: "Number", fieldName: "Data.FailedAttempts" },
    Attempts: { value: 3, propertyName: "Attempts", description: "Attempts", type: "Number", fieldName: "Data.Attempts" },
    Percentage: { value: 4, propertyName: "Percentage", description: "%", type: "Number", fieldName: "Data.Percentage" },
    DurationInMinutes: { value: 5, propertyName: "DurationInMinutes", description: "(Minutes)", type: "Number", fieldName: "Data.DurationInMinutes" },
    FirstCall: { value: 6, propertyName: "FirstCall", description: "First Call", type: "Datetime", fieldName: "Data.FirstAttempt" },
    LastCall: { value: 7, propertyName: "LastCall", description: "Last Call", type: "Datetime", fieldName: "Data.LastAttempt" }
});