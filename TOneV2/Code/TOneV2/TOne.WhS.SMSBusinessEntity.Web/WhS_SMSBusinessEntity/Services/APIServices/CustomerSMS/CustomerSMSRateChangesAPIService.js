(function (appControllers) {

    "use strict";

    customerSMSRateChangesAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig", "SecurityService"];

    function customerSMSRateChangesAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig, SecurityService) {

        var controllerName = "CustomerSMSRateChanges";

        function GetFilteredChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredChanges"), input);
        }

        function InsertOrUpdateChanges(customerChanges) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "InsertOrUpdateChanges"), customerChanges);
        }

        function UpdateSMSRateChangesStatus(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "UpdateSMSRateChangesStatus"), input);
        }

        function GetDraftData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetDraftData"), input);
        }

        function HasApplyChangesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['ApplyCustomerSMSRateChanges']));
        }

        function HasCancelDraftPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['CancelCustomerSMSRateDraft']));
        }

        function HasSaveChangesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['SaveCustomerSMSRateChanges']));
        }

        return {
            GetFilteredChanges: GetFilteredChanges,
            InsertOrUpdateChanges: InsertOrUpdateChanges,
            UpdateSMSRateChangesStatus: UpdateSMSRateChangesStatus,
            GetDraftData: GetDraftData,
            HasApplyChangesPermission: HasApplyChangesPermission,
            HasCancelDraftPermission: HasCancelDraftPermission,
            HasSaveChangesPermission: HasSaveChangesPermission
        };

    }

    appControllers.service("WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService", customerSMSRateChangesAPIService);

})(appControllers);