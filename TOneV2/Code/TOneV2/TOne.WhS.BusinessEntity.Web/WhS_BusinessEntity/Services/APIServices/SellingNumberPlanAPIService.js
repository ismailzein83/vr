(function (appControllers) {

    "use strict";
    SellingNumberPlanAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function SellingNumberPlanAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {


        function GetSellingNumberPlans() {
            return BaseAPIService.get("/api/SellingNumberPlan/GetSellingNumberPlans");
        }
        function GetSellingNumberPlan(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingNumberPlan", "GetSellingNumberPlan"), { sellingNumberPlanId: sellingNumberPlanId });
        }


        function GetFilteredSellingNumberPlans(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingNumberPlan", "GetFilteredSellingNumberPlans"), input);
        }

        function UpdateSellingNumberPlan(sellingNumberPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingNumberPlan", "UpdateSellingNumberPlan"), sellingNumberPlanObject);
        }
        function AddSellingNumberPlan(sellingNumberPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SellingNumberPlan", "AddSellingNumberPlan"), sellingNumberPlanObject);
        }

        return ({
            GetSellingNumberPlans: GetSellingNumberPlans,
            GetFilteredSellingNumberPlans: GetFilteredSellingNumberPlans,
            AddSellingNumberPlan: AddSellingNumberPlan,
            UpdateSellingNumberPlan: UpdateSellingNumberPlan,
            GetSellingNumberPlan: GetSellingNumberPlan
        });
    }

    appControllers.service('WhS_BE_SellingNumberPlanAPIService', SellingNumberPlanAPIService);

})(appControllers);