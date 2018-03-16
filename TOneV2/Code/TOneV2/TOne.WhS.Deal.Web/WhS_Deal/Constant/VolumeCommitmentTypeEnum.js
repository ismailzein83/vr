app.constant('WhS_Deal_VolumeCommitmentTypeEnum', {
    Buy: { value: 0, description: "Buy", carrierAccountSelector: "vr-whs-be-supplier-selector", zoneSelector: "vr-whs-be-supplierzone-selector", RateEvaluatorSelective: "vr-whs-deal-supplierrateevaluator-selective" },
    Sell: { value: 1, description: "Sell", carrierAccountSelector: "vr-whs-be-customer-selector", zoneSelector: "vr-whs-be-salezone-selector", RateEvaluatorSelective: "vr-whs-deal-salerateevaluator-selective" }
});