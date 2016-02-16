(function (appControllers) {

    "use strict";
    saleCodeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function saleCodeAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {
        function GetFilteredSaleCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "SaleCode", "GetFilteredSaleCodes"), input);
        }
        return ({
            GetFilteredSaleCodes: GetFilteredSaleCodes
        });
    }

    appControllers.service('WhS_BE_SaleCodeAPIService', saleCodeAPIService);

})(appControllers);