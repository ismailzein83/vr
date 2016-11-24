(function (appControllers) {

    "use strict";
    sellingNumberPlanAPIService.$inject = ["BaseAPIService", "UtilsService", "VR_NumberingPlan_ModuleConfig", "VRModalService", "SecurityService"];

    function sellingNumberPlanAPIService(BaseAPIService, UtilsService, VR_NumberingPlan_ModuleConfig, VRModalService, SecurityService) {

        var controllerName = "SellingNumberPlan";


        function GetSellingNumberPlans() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSellingNumberPlans"));
        }
        function GetSellingNumberPlan(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetSellingNumberPlan"), { sellingNumberPlanId: sellingNumberPlanId });
        }

        function GetMasterSellingNumberPlan() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetMasterSellingNumberPlan"));
        }

        function GetFilteredSellingNumberPlans(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredSellingNumberPlans"), input);
        }

        function UpdateSellingNumberPlan(sellingNumberPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "UpdateSellingNumberPlan"), sellingNumberPlanObject);
        }

        function AddSellingNumberPlan(sellingNumberPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "AddSellingNumberPlan"), sellingNumberPlanObject);
        }

        function HasUpdateSellingNumberPlanPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, ['UpdateSellingNumberPlan']));
        }

        function HasAddSellingNumberPlanPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, ['AddSellingNumberPlan']));
        }

        return ({
            GetSellingNumberPlans: GetSellingNumberPlans,
            GetFilteredSellingNumberPlans: GetFilteredSellingNumberPlans,
            AddSellingNumberPlan: AddSellingNumberPlan,
            UpdateSellingNumberPlan: UpdateSellingNumberPlan,
            GetSellingNumberPlan: GetSellingNumberPlan,
            HasUpdateSellingNumberPlanPermission: HasUpdateSellingNumberPlanPermission,
            HasAddSellingNumberPlanPermission: HasAddSellingNumberPlanPermission,
            GetMasterSellingNumberPlan: GetMasterSellingNumberPlan
        });
    }

    appControllers.service("Vr_NP_SellingNumberPlanAPIService", sellingNumberPlanAPIService);
})(appControllers);