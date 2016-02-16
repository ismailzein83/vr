(function (appControllers) {

    "use strict";
    nationalNumberingPlanAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function nationalNumberingPlanAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredNationalNumberingPlans(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "NationalNumberingPlan", "GetFilteredNationalNumberingPlans"), input);
        }

        function GetNationalNumberingPlan(nationalNumberingPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "NationalNumberingPlan", "GetNationalNumberingPlan"), {
                nationalNumberingPlanId: nationalNumberingPlanId
            });

        }
        function GetNationalNumberingPlansInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "NationalNumberingPlan", "GetNationalNumberingPlansInfo"));

        }
        function UpdateNationalNumberingPlan(nationalNumberingPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "NationalNumberingPlan", "UpdateNationalNumberingPlan"), nationalNumberingPlanObject);
        }
        function AddNationalNumberingPlan(nationalNumberingPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "NationalNumberingPlan", "AddNationalNumberingPlan"), nationalNumberingPlanObject);
        }
        return ({
            GetNationalNumberingPlansInfo: GetNationalNumberingPlansInfo,
            GetFilteredNationalNumberingPlans: GetFilteredNationalNumberingPlans,
            GetNationalNumberingPlan: GetNationalNumberingPlan,
            AddNationalNumberingPlan:AddNationalNumberingPlan,
            UpdateNationalNumberingPlan: UpdateNationalNumberingPlan
        });
    }

    appControllers.service('Demo_NationalNumberingPlanAPIService', nationalNumberingPlanAPIService);

})(appControllers);