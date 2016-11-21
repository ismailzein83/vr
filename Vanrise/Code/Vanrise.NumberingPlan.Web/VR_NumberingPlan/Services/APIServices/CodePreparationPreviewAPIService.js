(function (appControllers) {

    "use strict";
    codePreparationPreviewAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_NumberingPlan_ModuleConfig", "VRModalService"];

    function codePreparationPreviewAPIService(BaseAPIService, UtilsService, VR_NumberingPlan_ModuleConfig, VRModalService) {

        var controllerName = "CodePreparationPreview";

        function GetFilteredZonePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredZonePreview"), input);
        }
        function GetFilteredCodePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredCodePreview"), input);
        }
        function GetFilteredRatesPreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredRatesPreview"), input);
        }

        function GetFilteredCountryPreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredCountryPreview"), input);
        }


        return ({
            GetFilteredZonePreview: GetFilteredZonePreview,
            GetFilteredCodePreview: GetFilteredCodePreview,
            GetFilteredRatesPreview: GetFilteredRatesPreview,
            GetFilteredCountryPreview: GetFilteredCountryPreview
        });
    }

    appControllers.service("Vr_NP_CodePreparationPreviewAPIService", codePreparationPreviewAPIService);
})(appControllers);