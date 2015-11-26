(function (appControllers) {

    "use strict";
    supplierCodeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function supplierCodeAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredSupplierCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierCode", "GetFilteredSupplierCodes"), input);
        }


        return ({
            GetFilteredSupplierCodes: GetFilteredSupplierCodes
        });
    }

    appControllers.service('WhS_BE_SupplierCodeAPIService', supplierCodeAPIService);
})(appControllers);