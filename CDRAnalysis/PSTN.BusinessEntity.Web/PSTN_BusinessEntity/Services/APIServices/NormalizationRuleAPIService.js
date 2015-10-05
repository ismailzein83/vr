app.service("NormalizationRuleAPIService", function (BaseAPIService) {

    return ({
        GetFilteredNormalizationRules: GetFilteredNormalizationRules
    });

    function GetFilteredNormalizationRules(input) {
        return BaseAPIService.post("/api/NormalizationRule/GetFilteredNormalizationRules", input);
    }
});
