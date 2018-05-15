(function (appControllers) {

    "use strict";

    OtherRatesPreviewAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function OtherRatesPreviewAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        var controllerName = "OtherRatesPreview";

        function GetFilteredRatePreviews(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredRatesPreview"), input);
        }
        return ({
            GetFilteredRatePreviews: GetFilteredRatePreviews,
        });
    }

    appControllers.service("WhS_BE_OtherRatesPreviewAPIService", OtherRatesPreviewAPIService);

})(appControllers);
