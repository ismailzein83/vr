(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_SupPL_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig) {

        function UploadSupplierPriceList(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, "SupplierPriceList", "UploadSupplierPriceList"), input);
        }
        function DownloadSupplierPriceListTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, "SupplierPriceList", "DownloadSupplierPriceListTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        return ({
            UploadSupplierPriceList: UploadSupplierPriceList,
            DownloadSupplierPriceListTemplate: DownloadSupplierPriceListTemplate
        });
    }

    appControllers.service('WhS_SupPL_SupplierPriceListAPIService', supplierZoneAPIService);
})(appControllers);