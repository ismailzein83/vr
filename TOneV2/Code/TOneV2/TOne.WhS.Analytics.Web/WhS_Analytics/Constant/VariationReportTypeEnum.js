app.constant('WhS_Analytics_VariationReportTypeEnum',
{
    InBoundMinutes: { value: 0, description: 'InBound Minutes', category: 1, dimensionValue: 0, isVisible: true, isBilling: false },
    OutBoundMinutes: { value: 1, description: 'OutBound Minutes', category: 1, dimensionValue: 1, isVisible: true, isBilling: false},
    InOutBoundMinutes: { value: 2, description: 'InOutBound Minutes', category: 1, dimensionValue: null, isVisible: true, isBilling: false },
    TopDestinationMinutes: { value: 3, description: 'TopDestination Minutes', category: 1, dimensionValue: 2, isVisible: true, isBilling: false },

    InBoundAmount: { value: 4, description: 'InBound Amount', category: 2, dimensionValue: 0, isVisible: true, isBilling: true },
    OutBoundAmount: { value: 5, description: 'OutBound Amount', category: 2, dimensionValue: 1, isVisible: true, isBilling: true },
    InOutBoundAmount: { value: 6, description: 'InOutBound Amount', category: 2, dimensionValue: null, isVisible: true, isBilling: true },
    TopDestinationAmount: { value: 7, description: 'TopDestination Amount', category: 2, dimensionValue: 2, isVisible: true, isBilling: true },

    Profit: { value: 8, description: 'Profit', category: 3, dimensionValue: 0, isVisible: true, isBilling: true },
    OutBoundProfit: { value: 9, description: 'OutBound Profit', category: 3, dimensionValue: 1, isVisible: false, isBilling: true },
    TopDestinationProfit: { value: 10, description: 'TopDestination Profit', category: 3, dimensionValue: 2, isVisible: false, isBilling: true }
});