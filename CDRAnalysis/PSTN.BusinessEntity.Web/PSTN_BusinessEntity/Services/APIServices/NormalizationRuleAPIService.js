app.service("NormalizationRuleAPIService", function (BaseAPIService) {

    return ({
        GetFilteredNormalizationRules: GetFilteredNormalizationRules,
        GetNormalizationRuleActionBehaviorTemplates: GetNormalizationRuleActionBehaviorTemplates,
        GetNormalizationRuleById: GetNormalizationRuleById,
        AddNormalizationRule: AddNormalizationRule,
        UpdateNormalizationRule: UpdateNormalizationRule,
        DeleteNormalizationRule: DeleteNormalizationRule
    });

    function GetFilteredNormalizationRules(input) {
        return BaseAPIService.post("/api/NormalizationRule/GetFilteredNormalizationRules", input);
    }

    function GetNormalizationRuleActionBehaviorTemplates() {
        return BaseAPIService.get("/api/NormalizationRule/GetNormalizationRuleActionBehaviorTemplates");
    }

    function GetNormalizationRuleById(normalizationRuleId) {
        return BaseAPIService.get("/api/NormalizationRule/GetNormalizationRuleById", {
            normalizationRuleId: normalizationRuleId
        });
    }

    function AddNormalizationRule(normalizationRuleObj) {
        return BaseAPIService.post("/api/NormalizationRule/AddNormalizationRule", normalizationRuleObj);
    }

    function UpdateNormalizationRule(normalizationRuleObj) {
        return BaseAPIService.post("/api/NormalizationRule/UpdateNormalizationRule", normalizationRuleObj);
    }

    function DeleteNormalizationRule(normalizationRuleId) {
        return BaseAPIService.get("/api/NormalizationRule/DeleteNormalizationRule", {
            normalizationRuleId: normalizationRuleId
        });
    }
});
