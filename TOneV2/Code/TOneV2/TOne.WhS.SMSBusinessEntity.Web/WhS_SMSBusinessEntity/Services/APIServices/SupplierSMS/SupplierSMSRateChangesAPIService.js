(function (appControllers) {

    "use strict";

    supplierSMSRateChangesAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_SMSBusinessEntity_ModuleConfig"];

    function supplierSMSRateChangesAPIService(BaseAPIService, UtilsService, WhS_SMSBusinessEntity_ModuleConfig) {

        var controllerName = "SupplierSMSRateChanges";

        function GetFilteredChanges(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetFilteredChanges"), input);
        }

        function InsertOrUpdateChanges(supplierChanges) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "InsertOrUpdateChanges"), supplierChanges);
        }

        function UpdateSMSRateChangesStatus(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "UpdateSMSRateChangesStatus"), input);
        }

        function GetDraftData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, "GetDraftData"), input);
        }

        function HasApplyChangesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['ApplySupplierSMSRateChanges']));
        }

        function HasCancelDraftPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['CancelSupplierSMSRateDraft']));
        }

        function HasSaveChangesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_SMSBusinessEntity_ModuleConfig.moduleName, controllerName, ['SaveSupplierSMSRateChanges']));
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

    appControllers.service("WhS_SMSBusinessEntity_SupplierSMSRateChangesAPIService", supplierSMSRateChangesAPIService);

})(appControllers);