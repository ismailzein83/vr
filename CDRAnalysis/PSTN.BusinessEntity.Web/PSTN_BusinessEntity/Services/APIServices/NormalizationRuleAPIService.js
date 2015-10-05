app.service("NormalizationRuleAPIService", function (BaseAPIService) {

    return ({
        GetFilteredNormalizationRules: GetFilteredNormalizationRules,
        GetNormalizationRuleActionBehaviorTemplates: GetNormalizationRuleActionBehaviorTemplates
    });

    function GetFilteredNormalizationRules(input) {
        return BaseAPIService.post("/api/NormalizationRule/GetFilteredNormalizationRules", input);
    }

    function GetNormalizationRuleActionBehaviorTemplates() {
        return BaseAPIService.get("/api/NormalizationRule/GetNormalizationRuleActionBehaviorTemplates");
    }
});
