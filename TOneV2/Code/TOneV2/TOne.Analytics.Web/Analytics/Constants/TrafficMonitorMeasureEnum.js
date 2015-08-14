app.constant('TrafficMonitorMeasureEnum', {
    Attempts: { value: 2, propertyName: "Attempts", description: "Attempts", isSum: true, type: "Number", fieldName: "Data.Attempts" },
    SuccessfulAttempts: { value: 3, propertyName: "SuccessfulAttempts", description: "Successful", isSum: true, type: "Number", fieldName: "Data.SuccessfulAttempts" },
    FailedAttempts: { value: 4, propertyName: "FailedAttempts", description: "Failed", isSum: true, type: "Number", fieldName: "Data.FailedAttempts" },
    
    DurationsInMinutes: { value: 5, propertyName: "DurationsInMinutes", description: "Durations (min)", isSum: true, type: "Number", fieldName: "Data.DurationsInMinutes" },
    CeiledDuration: { value: 6, propertyName: "CeiledDuration", description: "CeiledDuration", isSum: true, type: "Number", fieldName: "Data.CeiledDuration" },
    ASR: { value: 7, propertyName: "ASR", description: "ASR", isSum: true, type: "Number", fieldName: "Data.ASR" },
    ABR: { value: 8, propertyName: "ABR", description: "ABR", isSum: true, type: "Number", fieldName: "Data.ABR" },
    ACD: { value: 9, propertyName: "ACD", description: "ACD", isSum: true, type: "Number", fieldName: "Data.ACD" },
    NER: { value: 10, propertyName: "NER", description: "NER", isSum: true, type: "Number", fieldName: "Data.NER" },
    PDDInSeconds: { value: 11, propertyName: "PDDInSeconds", description: "PDD (sec)", isSum: true, type: "Number", fieldName: "Data.PDDInSeconds" },
    PGAD: { value: 12, propertyName: "PGAD", description: "PGAD", isSum: true, type: "Number", fieldName: "Data.PGAD" },
    MaxDurationInMinutes: { value: 13, propertyName: "MaxDurationInMinutes", description: "Max Duration (min)", isSum: true, type: "Number", fieldName: "Data.MaxDurationInMinutes" },
    LastCDRAttempt: { value: 14, propertyName: "LastCDRAttempt", description: "Last Call", type: "Datetime", fieldName: "Data.LastCDRAttempt" },
   
    
});