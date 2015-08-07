app.constant('TrafficStatisticsMeasureEnum', {
    //FirstCDRAttempt: { value: 0, propertyName: "FirstCDRAttempt", description: "First CDR Attempt" },
    //LastCDRAttempt: { value: 1, propertyName: "LastCDRAttempt", description: "Last CDR Attempt" },
    Attempts: { value: 2, propertyName: "Attempts", description: "Attempts", isSum: true ,type:"Number"},
    SuccessfulAttempts: { value: 3, propertyName: "SuccessfulAttempts", description: "Successful", isSum: true, type: "Number" },
    FailedAttempts: { value: 4, propertyName: "FailedAttempts", description: "Failed", isSum: true, type: "Number" },
    
    DurationsInMinutes: { value: 5, propertyName: "DurationsInMinutes", description: "Durations (min)", isSum: true, type: "Number" },
    CeiledDuration: { value: 6, propertyName: "CeiledDuration", description: "CeiledDuration", isSum: true, type: "Number" },
    ACD: { value: 7, propertyName: "ACD", description: "ACD", isSum: true, type: "Number" },
    PDDInSeconds: { value: 8, propertyName: "PDDInSeconds", description: "PDD (sec)", isSum: true, type: "Number" },
    PGAD: { value: 9, propertyName: "PGAD", description: "PGAD", isSum: true, type: "Number" },
    MaxDurationInMinutes: { value: 10, propertyName: "MaxDurationInMinutes", description: "Max Duration (min)", isSum: true, type: "Number" },
    LastCDRAttempt: { value: 11, propertyName: "LastCDRAttempt", description: "Last Call", type: "Datetime" },
   
    
});