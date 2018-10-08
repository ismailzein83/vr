app.constant('WhS_Routing_OrderTypeEnum', {
    Percentage: { value: 0, description: "Percentage", hasCheckValidation: true, from: 2, showPercentageColumn: true, showPercentageSettings: true },
    Sequential: { value: 1, description: "Sequential", hasCheckValidation: false, from: 2, showPercentageColumn: false, showPercentageSettings: true },
    OptionDistribution: { value: 2, description: "Option Distribution", hasCheckValidation: true, from: 1, showPercentageColumn: true, showPercentageSettings: false },
    Order: { value: 3, description: "Order", from: 0, to: 1, showPercentageColumn: false, showPercentageSettings: true }
});
