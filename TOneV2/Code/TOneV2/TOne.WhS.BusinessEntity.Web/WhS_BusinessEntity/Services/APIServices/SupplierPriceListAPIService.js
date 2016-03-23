(function (appControllers) {

    "use strict";
    supplierPriceListAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function supplierPriceListAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = 'SupplierPricelist';

        function GetFilteredSupplierPricelist(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierPricelist"), input);
        }

        return ({
            GetFilteredSupplierPricelist: GetFilteredSupplierPricelist
        });
    }

    appControllers.service('WhS_BE_SupplierPriceListAPIService', supplierPriceListAPIService);

})(appControllers);