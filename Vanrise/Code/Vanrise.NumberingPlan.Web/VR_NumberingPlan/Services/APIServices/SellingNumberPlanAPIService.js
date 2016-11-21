(function (appControllers) {

    "use strict";
    sellingNumberPlanAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_NumberingPlan_ModuleConfig", "VRModalService"];

    function sellingNumberPlanAPIService(BaseAPIService, UtilsService, VR_NumberingPlan_ModuleConfig, VRModalService) {

        var controllerName = "SellingNumberPlan";


        function GetSellingNumberPlans() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSellingNumberPlans"));
        }

        return ({
            GetSellingNumberPlans: GetSellingNumberPlans
        });
    }

    appControllers.service("Vr_NP_SellingNumberPlanAPIService", sellingNumberPlanAPIService);
})(appControllers);