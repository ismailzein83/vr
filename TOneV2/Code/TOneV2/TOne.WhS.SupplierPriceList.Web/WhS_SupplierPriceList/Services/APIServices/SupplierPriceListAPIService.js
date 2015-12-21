(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_SupPL_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_SupPL_ModuleConfig) {

        function UploadSupplierPriceList(supplierAccountId, currencyId, fileId, effectiveDate) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_SupPL_ModuleConfig.moduleName, "SupplierPriceList", "UploadSupplierPriceList"), {
                supplierAccountId: supplierAccountId,
                currencyId: currencyId,
                fileId: fileId,
                effectiveDate: effectiveDate
            });
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