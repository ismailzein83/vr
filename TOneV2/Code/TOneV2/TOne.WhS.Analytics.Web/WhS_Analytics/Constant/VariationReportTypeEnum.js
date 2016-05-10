app.constant('WhS_Analytics_VariationReportTypeEnum',
{
    InBoundMinutes: { value: 0, description: 'In Bound Minutes', category: 1, dimensionValue: 0 },
    OutBoundMinutes: { value: 1, description: 'Out Bound Minutes', category: 1, dimensionValue: 1 },
    InOutBoundMinutes: { value: 2, description: 'In Out Bound Minutes', category: 1, dimensionValue: null },
    TopDestinationMinutes: { value: 3, description: 'Top Destination Minutes', category: 1, dimensionValue: 2 },

    InBoundAmount: { value: 4, description: 'In Bound Amount', category: 2, dimensionValue: 0 },
    OutBoundAmount: { value: 5, description: 'Out Bound Amount', category: 2, dimensionValue: 1 },
    InOutBoundAmount: { value: 6, description: 'In Out Bound Amount', category: 2, dimensionValue: null },
    TopDestinationAmount: { value: 7, description: 'Top Destination Amount', category: 2, dimensionValue: 2 },

    Profit: { value: 8, description: 'Profit', category: 3, dimensionValue: 0 }
});