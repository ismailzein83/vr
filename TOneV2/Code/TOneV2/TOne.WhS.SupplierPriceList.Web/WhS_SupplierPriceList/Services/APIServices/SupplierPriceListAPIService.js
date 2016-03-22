(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_SupPL_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig) {

        function DownloadSupplierPriceListTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, "SupplierPriceList", "DownloadSupplierPriceListTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        return ({
            DownloadSupplierPriceListTemplate: DownloadSupplierPriceListTemplate
        });
    }

    appControllers.service('WhS_SupPL_SupplierPriceListAPIService', supplierZoneAPIService);
})(appControllers);