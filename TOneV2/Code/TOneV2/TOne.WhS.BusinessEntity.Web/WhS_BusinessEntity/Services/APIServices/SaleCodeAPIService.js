(function (appControllers) {

    "use strict";
    saleCodesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function saleCodesAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        function GetFilteredSaleCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SaleCode", "GetFilteredSaleCodes"), input);
        }
        return ({
            GetFilteredSaleCodes: GetFilteredSaleCodes
        });
    }

    appControllers.service('WhS_BE_SaleCodesAPIService', saleCodesAPIService);

})(appControllers);