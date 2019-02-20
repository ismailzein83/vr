(function (appControllers) {

    "use strict";

    supplierSMSRateAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig", "SecurityService"];

    function supplierSMSRateAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig, SecurityService) {

        var controllerName = "SupplierSMSRate";

        function GetFilteredSupplierSMSRate(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredSupplierSMSRate"), input);
        }

        function GetFilteredSMSCostDetails(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredSMSCostDetails"), input);
        }

        function HasSearchRatesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['GetFilteredSupplierSMSRate']));
        }

        function HasAddDraftPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['AddSupplierSMSRateDraft']));
        }

        function HasViewSMSCostAnalysisPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['GetFilteredSMSCostDetails']));
        }

        return {
            GetFilteredSupplierSMSRate: GetFilteredSupplierSMSRate,
            GetFilteredSMSCostDetails: GetFilteredSMSCostDetails,
            HasSearchRatesPermission: HasSearchRatesPermission,
            HasAddDraftPermission: HasAddDraftPermission,
            HasViewSMSCostAnalysisPermission: HasViewSMSCostAnalysisPermission
        };

    }

    appControllers.service("WhS_SMSBusinessEntity_SupplierSMSRateAPIService", supplierSMSRateAPIService);

})(appControllers);