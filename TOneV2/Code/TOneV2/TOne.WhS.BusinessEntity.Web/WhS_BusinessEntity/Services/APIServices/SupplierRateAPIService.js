(function (appControllers) {

    "use strict";
    supplierRateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function supplierRateAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredSupplierRates(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierRate", "GetFilteredSupplierRates"), input);
        }


        return ({
            GetFilteredSupplierRates:GetFilteredSupplierRates
        });
    }

    appControllers.service('WhS_BE_SupplierRateAPIService', supplierRateAPIService);
})(appControllers);