(function (appControllers) {

    "use strict";
    supplierPriceListPreviewPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_SupPL_ModuleConfig'];

    function supplierPriceListPreviewPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig) {
        
        function GetFilteredZonePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, "SupplierPriceListPreview", "GetFilteredZonePreview"), input);
        }
        function GetFilteredCodePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, "SupplierPriceListPreview", "GetFilteredCodePreview"), input);
        }
        function GetFilteredRatePreview(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, "SupplierPriceListPreview", "GetFilteredRatePreview"), input);
        }
        return ({
            GetFilteredZonePreview:GetFilteredZonePreview,
            GetFilteredCodePreview: GetFilteredCodePreview,
            GetFilteredRatePreview: GetFilteredRatePreview
        });
    }

    appControllers.service('WhS_SupPL_SupplierPriceListPreviewPIService', supplierPriceListPreviewPIService);
})(appControllers);