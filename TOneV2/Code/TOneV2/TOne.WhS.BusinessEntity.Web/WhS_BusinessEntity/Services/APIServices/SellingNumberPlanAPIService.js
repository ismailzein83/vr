(function (appControllers) {

    "use strict";
    SellingNumberPlanAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig', 'SecurityService'];

    function SellingNumberPlanAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = "SellingNumberPlan";

        function GetSellingNumberPlans() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSellingNumberPlans"));
        }

        function GetSellingNumberPlan(sellingNumberPlanId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetSellingNumberPlan"), { sellingNumberPlanId: sellingNumberPlanId });
        }

        function GetMasterSellingNumberPlan() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetMasterSellingNumberPlan"));
        }

        function GetFilteredSellingNumberPlans(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredSellingNumberPlans"), input);
        }

        function UpdateSellingNumberPlan(sellingNumberPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateSellingNumberPlan"), sellingNumberPlanObject);
        }

        function AddSellingNumberPlan(sellingNumberPlanObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddSellingNumberPlan"), sellingNumberPlanObject);
        }

        function HasUpdateSellingNumberPlanPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateSellingNumberPlan']));
        }

        function HasAddSellingNumberPlanPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddSellingNumberPlan']));
        }
        function GetSellingNumberPlanHistoryDetailbyHistoryId(sellingNumberPlanHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetSellingNumberPlanHistoryDetailbyHistoryId'), {
                sellingNumberPlanHistoryId: sellingNumberPlanHistoryId
            });
        }
        return ({
            GetSellingNumberPlans: GetSellingNumberPlans,
            GetFilteredSellingNumberPlans: GetFilteredSellingNumberPlans,
            AddSellingNumberPlan: AddSellingNumberPlan,
            UpdateSellingNumberPlan: UpdateSellingNumberPlan,
            GetSellingNumberPlan: GetSellingNumberPlan,
            HasUpdateSellingNumberPlanPermission: HasUpdateSellingNumberPlanPermission,
            HasAddSellingNumberPlanPermission: HasAddSellingNumberPlanPermission,
            GetMasterSellingNumberPlan: GetMasterSellingNumberPlan,
            GetSellingNumberPlanHistoryDetailbyHistoryId: GetSellingNumberPlanHistoryDetailbyHistoryId
        });
    }

    appControllers.service('WhS_BE_SellingNumberPlanAPIService', SellingNumberPlanAPIService);

})(appControllers);