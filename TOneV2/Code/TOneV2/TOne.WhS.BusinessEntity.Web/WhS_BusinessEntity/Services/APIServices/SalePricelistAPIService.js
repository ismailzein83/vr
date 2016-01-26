(function (appControllers) {

    "use strict";
    salePricelistAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function salePricelistAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        this.GetFilteredSalePricelists = function (input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "salePricelist", "GetFilteredSalePriceLists"), input);
        }

    }

    appControllers.service('VR_BE_SalePricelistAPIService', salePricelistAPIService);

})(appControllers);