(function (appControllers) {

    "use strict";
    supplierPriceListPreviewPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SupPL_ModuleConfig"];

    function supplierPriceListPreviewPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig) {

        var controllerName = "SupplierPriceListPreview";

        function GetFilteredZonePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredZonePreview"), input);
        }
        function GetFilteredCodePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredCodePreview"), input);
        }
        function GetFilteredRatePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredRatePreview"), input);
        }

        function GetFilteredCountryPreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredCountryPreview"), input);
        }

        function GetFilteredOtherRatePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredOtherRatePreview"), input);
        }

        function GetFilteredZoneServicesPreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, controllerName, "GetFilteredZoneServicesPreview"), input);
        }
        
        return ({
            GetFilteredZonePreview: GetFilteredZonePreview,
            GetFilteredCodePreview: GetFilteredCodePreview,
            GetFilteredRatePreview: GetFilteredRatePreview,
            GetFilteredCountryPreview: GetFilteredCountryPreview,
            GetFilteredOtherRatePreview: GetFilteredOtherRatePreview,
            GetFilteredZoneServicesPreview: GetFilteredZoneServicesPreview

        });
    }

    appControllers.service("WhS_SupPL_SupplierPriceListPreviewPIService", supplierPriceListPreviewPIService);
})(appControllers);