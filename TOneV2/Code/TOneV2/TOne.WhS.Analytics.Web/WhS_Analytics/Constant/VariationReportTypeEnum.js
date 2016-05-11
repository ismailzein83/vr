app.constant('WhS_Analytics_VariationReportTypeEnum',
{
    InBoundMinutes: { value: 0, description: 'In Bound Minutes', category: 1, dimensionValue: 0, isVisible: true },
    OutBoundMinutes: { value: 1, description: 'Out Bound Minutes', category: 1, dimensionValue: 1, isVisible: true },
    InOutBoundMinutes: { value: 2, description: 'In Out Bound Minutes', category: 1, dimensionValue: null, isVisible: true },
    TopDestinationMinutes: { value: 3, description: 'Top Destination Minutes', category: 1, dimensionValue: 2, isVisible: true },

    InBoundAmount: { value: 4, description: 'In Bound Amount', category: 2, dimensionValue: 0, isVisible: true },
    OutBoundAmount: { value: 5, description: 'Out Bound Amount', category: 2, dimensionValue: 1, isVisible: true },
    InOutBoundAmount: { value: 6, description: 'In Out Bound Amount', category: 2, dimensionValue: null, isVisible: true },
    TopDestinationAmount: { value: 7, description: 'Top Destination Amount', category: 2, dimensionValue: 2, isVisible: true },

    Profit: { value: 8, description: 'Profit', category: 3, dimensionValue: 0, isVisible: true },
    OutBoundProfit: { value: 9, description: 'Out Bound Profit', category: 3, dimensionValue: 1, isVisible: false },
    TopDestinationProfit: { value: 10, description: 'Top Destination Profit', category: 3, dimensionValue: 2, isVisible: false }
});