app.constant('TrafficStatisticsMeasureEnum', {
    //FirstCDRAttempt: { value: 0, propertyName: "FirstCDRAttempt", description: "First CDR Attempt" },
    //LastCDRAttempt: { value: 1, propertyName: "LastCDRAttempt", description: "Last CDR Attempt" },
    Attempts: { value: 2, propertyName: "Attempts", description: "Attempts", isSum: true },
    DeliveredAttempts: { value: 3, propertyName: "DeliveredAttempts", description: "Delivered Attempts", isSum: true },
    SuccessfulAttempts: { value: 4, propertyName: "SuccessfulAttempts", description: "Successful Attempts", isSum: true },
    DurationsInMinutes: { value: 5, propertyName: "DurationsInMinutes", description: "Durations (min)", isSum: true },
    MaxDurationInMinutes: { value: 6, propertyName: "MaxDurationInMinutes", description: "Max Duration (min)" },
    PDDInSeconds: { value: 7, propertyName: "PDDInSeconds", description: "PDD (sec)" },
    UtilizationInSeconds: { value: 8, propertyName: "UtilizationInSeconds", description: "Utilization (sec)" },
    NumberOfCalls: { value: 9, propertyName: "NumberOfCalls", description: "Nb of Calls", isSum: true },
    DeliveredNumberOfCalls: { value: 10, propertyName: "DeliveredNumberOfCalls", description: "Delivered Nb of Calls", isSum: true },
    PGAD: { value: 11, propertyName: "PGAD", description: "PGAD" }
});