app.constant("WhS_Deal_ZoneRateEvaluationTypeEnum", {
	MaximumRate: { value: 0, description: "Maximum Rate", directive: 'vr-whs-deal-zonerateeval-max' },
	AverageRate: { value: 1, description: "Average Rate", directive: 'vr-whs-deal-zonerateeval-avg' },
	BasedOnTraffic: { value: 2, description: "Based on Traffic", directive: 'vr-whs-deal-zonerateeval-traffic' }
});