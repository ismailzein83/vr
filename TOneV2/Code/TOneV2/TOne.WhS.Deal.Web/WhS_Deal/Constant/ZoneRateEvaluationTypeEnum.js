app.constant("WhS_Deal_ZoneRateEvaluationTypeEnum", {
	AverageRate: { value: 0, description: "Average Rate", directive: 'vr-whs-deal-zonerateeval-avg' },
	MaximumRate: { value: 1, description: "Maximum Rate", directive: 'vr-whs-deal-zonerateeval-max' },
	BasedOnTraffic: { value: 2, description: "Based on Traffic", directive: 'vr-whs-deal-zonerateeval-traffic' }
});