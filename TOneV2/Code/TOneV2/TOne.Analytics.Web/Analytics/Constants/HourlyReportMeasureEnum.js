
app.constant('HourlyReportMeasureEnum', {
    Hour: { value: 0, propertyName: "Hour", description: "Hour", isSum: true, type: "Number", fieldName: "Data.Hour" },
    Date: { value: 1, propertyName: "Date", description: "Date", isSum: true, type: "Date", fieldName: "Data.Date" },
    Attempts: { value: 2, propertyName: "Attempts", description: "Attempts", isSum: true, type: "Number", fieldName: "Data.Attempts" },
    SuccessfulAttempts: { value: 3, propertyName: "SuccessfulAttempt", description: "Successful", isSum: true, type: "Number", fieldName: "Data.SuccessfulAttempt" },
    ASR: { value: 4, propertyName: "ASR", description: "ASR", isSum: true, type: "Number", fieldName: "Data.ASR" },
    NER: { value: 5, propertyName: "NER", description: "NER", isSum: true, type: "Number", fieldName: "Data.NER" },
    ACD: { value: 6, propertyName: "ACD", description: "ACD", isSum: true, type: "Number", fieldName: "Data.ACD" },
    GrayArea: { value: 7, propertyName: "GrayArea", description: "% GrayArea", isSum: true, type: "Number", fieldName: "Data.Attempts" },
    CapacityUsageDetails: { value: 8, propertyName: "CapacityUsageDetails", description: "Capacity Usage Details", isSum: true, type: "Progress", fieldName: "Data.Hour" },
    FailedAttempts: { value: 9, propertyName: "FailedAttempts", description: "Failed", isSum: true, type: "Number", fieldName: "Data.FailedAttempts" },
    DurationsInMinutes: { value: 10, propertyName: "DurationsInMinutes", description: "Durations (min)", isSum: true, type: "Number", fieldName: "Data.DurationsInMinutes" },
    LastCDRAttempt: { value: 11, propertyName: "LastAttempt", description: "Last Call", type: "Datetime", fieldName: "Data.LastAttempt" },


});