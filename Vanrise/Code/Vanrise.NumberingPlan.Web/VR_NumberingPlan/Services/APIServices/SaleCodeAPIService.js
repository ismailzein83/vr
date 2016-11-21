(function (appControllers) {

    "use strict";
    saleCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_NumberingPlan_ModuleConfig"];
    function saleCodeAPIService(BaseAPIService, UtilsService, VR_NumberingPlan_ModuleConfig) {

        var controllerName = "SaleCode";

        function GetFilteredSaleCodes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredSaleCodes"), input);
        }

        return ({
            GetFilteredSaleCodes: GetFilteredSaleCodes
        });
    }

    appControllers.service("Vr_NP_SaleCodeAPIService", saleCodeAPIService);

})(appControllers);