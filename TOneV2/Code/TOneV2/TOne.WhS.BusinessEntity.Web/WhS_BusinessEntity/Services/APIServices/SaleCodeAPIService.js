(function (appControllers) {

    "use strict";
    saleCodeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function saleCodeAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        function GetFilteredSaleCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleCode", "GetFilteredSaleCodes"), input);
        }
        return ({
            GetFilteredSaleCodes: GetFilteredSaleCodes
        });
    }

    appControllers.service('WhS_BE_SaleCodeAPIService', saleCodeAPIService);

})(appControllers);