(function (appControllers) {

    "use strict";

    normalizationRuleAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_Rules_ModuleConfig"];

    function normalizationRuleAPIService(BaseAPIService, UtilsService, VR_Rules_ModuleConfig) {

        function GetNormalizeNumberActionSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Rules_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizeNumberActionSettingsTemplates"));
        }

        return ({
            GetNormalizeNumberActionSettingsTemplates: GetNormalizeNumberActionSettingsTemplates,
        });
    }

    appControllers.service("VR_Rules_NormalizationRuleAPIService", normalizationRuleAPIService);

})(appControllers);
