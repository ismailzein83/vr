(function (appControllers) {

    "use strict";
    supplierPriceListAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_SupPriceList_ModuleConfig'];

    function supplierPriceListAPIService(baseApiService, utilsService, moduleConfig) {

        function importPriceList(obj) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "PriceList", "ImportPriceList"), obj);
        }
        function GetUpdated(input) {
            return baseApiService.post(utilsService.getServiceURL(moduleConfig.moduleName, "PriceList", "GetUpdated"), input);
        }

        return ({
            importPriceList: importPriceList,
            GetUpdated: GetUpdated
        });

    }

    appControllers.service('CP_SupplierPricelist_SupplierPriceListAPIService', supplierPriceListAPIService);

})(appControllers);