
app.constant('HourlyReportMeasureEnum', {
    Hour: { value: 0, propertyName: "Hour", description: "Hour", isSum: true, type: "Number" },
    Date: { value: 1, propertyName: "Date", description: "Date", isSum: true, type: "Date" },
    Attempts: { value: 2, propertyName: "Attempts", description: "Attempts", isSum: true, type: "Number" },
    SuccessfulAttempts: { value: 3, propertyName: "SuccessfulAttempts", description: "Successful", isSum: true, type: "Number" },
    ASR: { value: 4, propertyName: "ASR", description: "ASR", isSum: true, type: "Number" },
    NER: { value: 5, propertyName: "NER", description: "NER", isSum: true, type: "Number" },
    ACD: { value: 6, propertyName: "ACD", description: "ACD", isSum: true, type: "Number" },
    GrayArea: { value: 7, propertyName: "GrayArea", description: "% GrayArea", isSum: true, type: "Number" },
    CapacityUsageDetails: { value: 8, propertyName: "CapacityUsageDetails", description: "Capacity Usage Details", isSum: true, type: "Number" },
    FailedAttempts: { value: 9, propertyName: "FailedAttempts", description: "Failed", isSum: true, type: "Number" },
    DurationsInMinutes: { value: 10, propertyName: "DurationsInMinutes", description: "Durations (min)", isSum: true, type: "Number" },
    LastCDRAttempt: { value: 11, propertyName: "LastCDRAttempt", description: "Last Call", type: "Datetime" },


});