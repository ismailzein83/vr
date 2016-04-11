(function (appControllers) {

    "use strict";
    salePricelistAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];
    function salePricelistAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "SalePricelist";

        function GetFilteredSalePriceLists(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSalePriceLists"), input);
        }

        return ({
            GetFilteredSalePriceLists: GetFilteredSalePriceLists
        });
    }

    appControllers.service("WhS_BE_SalePricelistAPIService", salePricelistAPIService);

})(appControllers);