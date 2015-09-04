app.constant('GenericAnalyticMeasureEnum', {
    Measure_Attempts: { value: 4, description: "Attempts", type: "Text" },
    Measure_FirstCDRAttempt: { value: 0, description: "First Attempt", type: "Text" },
    Measure_SuccessfulAttempts: { value: 5, description: "Successful", type: "Text" },
    Measure_FailedAttempts: { value: 6, description: "Failed", type: "Text" },
    Measure_DeliveredAttempts: { value: 7, description: "Delivered", type: "Text" },
    Measure_DurationsInSeconds: { value: 8, description: "Durations (MIN)", type: "Text" },
    Measure_CeiledDuration: { value: 13, description: "Ceiled Duration", type: "Text" },
    Measure_ASR: { value: 2, description: "ASR", type: "Text" },
    Measure_ABR: { value: 1, description: "ABR", type: "Text" },
    Measure_ACD: { value: 14, description: "ACD", type: "Text" },
    Measure_NER: { value: 3, description: "NER", type: "Text" },
    Measure_PDDInSeconds: { value: 9, description: "PDD(SEC)", type: "Text" },
    Measure_PGAD: { value: 17, description: "PGAD", type: "Text" },
    MeasureMaxDurationInSeconds: { value: 16, description: "Max Duration(min)", type: "Text" },
    Measure_LastCDRAttempt: { value: 15, description: "Last call", type: "Text" },
    
    Measure_UtilizationInSeconds: { value: 10, description: "Utilization", type: "Text" },
    Measure_NumberOfCalls: { value: 11, description: "Number Of Calls", type: "Text" },
    Measure_DeliveredNumberOfCalls: { value: 12, description: "Delivered Number Of Calls", type: "Text" },
    Measure_AveragePDD: { value: 18, description: "avg PDD", type: "Text" }
});