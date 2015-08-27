app.constant('CarrierSummaryMeasureEnum', {
    Attempts: { value: 2, propertyName: "Attempts", description: "Attempts", isSum: true, type: "Number", fieldName: "Data.Attempts" },
    DurationsInMinutes: { value: 5, propertyName: "DurationsInMinutes", description: "Durations (min)", isSum: true, type: "Number", fieldName: "Data.DurationsInMinutes" },
    ASR: { value: 7, propertyName: "ASR", description: "ASR", isSum: true, type: "Number", fieldName: "Data.ASR" },
    ACD: { value: 9, propertyName: "ACD", description: "ACD", isSum: true, type: "Number", fieldName: "Data.ACD" },
    NER: { value: 10, propertyName: "NER", description: "NER", isSum: true, type: "Number", fieldName: "Data.NER" },
    PDDInSeconds: { value: 11, propertyName: "PDD", description: "PDD (sec)", isSum: true, type: "Number", fieldName: "Data.PDD" },
    PricedDuration: { value: 11, propertyName: "PricedDuration", description: "Priced Duration", type: "Number", fieldName: "Data.PricedDuration" },
    Sale_Nets: { value: 11, propertyName: "Sale_Nets", description: "Sale Nets", isSum: true, type: "Number", fieldName: "Data.Sale_Nets" },
    Cost_Nets: { value: 11, propertyName: "Cost_Nets", description: "Cost Nets", isSum: true, type: "Number", fieldName: "Data.Cost_Nets" },
    Profit: { value: 13, propertyName: "Profit", description: "Profit", isSum: true, type: "Number", fieldName: "Data.Profit" },
    Percentage: { value: 14, propertyName: "Percentage", description: "Percentage", type: "Datetime", fieldName: "Data.Percentage" },
});