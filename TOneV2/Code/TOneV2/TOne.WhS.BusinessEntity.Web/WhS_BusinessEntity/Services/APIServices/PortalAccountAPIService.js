(function (appControllers) {

    "use strict";

    PortalAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function PortalAccountAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "PortalAccount";

        function GetCarrierProfilePortalAccounts(carrierProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetCarrierProfilePortalAccounts'), {
                carrierProfileId: carrierProfileId
            });
        }

        function AddPortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'AddPortalAccount'), portalAccountEditorObject);
        }

        function UpdatePortalAccount(portalAccountEditorObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'UpdatePortalAccount'), portalAccountEditorObject);
        }

        function GetPortalAccount(carrierProfileId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetPortalAccount'), {
                carrierProfileId: carrierProfileId,
                userId: userId
            });
        }
        function ResetPassword(resetPasswordInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'ResetPassword'), resetPasswordInput);
        }

        function DisablePortalAccount(carrierProfileId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'DisablePortalAccount'), {
                carrierProfileId: carrierProfileId,
                userId: userId
            });
        }
        function EnablePortalAccount(carrierProfileId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'EnablePortalAccount'), {
                carrierProfileId: carrierProfileId,
                userId: userId
            });
        }
        function UnlockPortalAccount(carrierProfileId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'UnlockPortalAccount'), {
                carrierProfileId: carrierProfileId,
                userId: userId
            });
        }

        return ({
            GetCarrierProfilePortalAccounts: GetCarrierProfilePortalAccounts,
            AddPortalAccount: AddPortalAccount,
            UpdatePortalAccount: UpdatePortalAccount,
            GetPortalAccount: GetPortalAccount,
            ResetPassword: ResetPassword,
            DisablePortalAccount: DisablePortalAccount,
            EnablePortalAccount: EnablePortalAccount,
            UnlockPortalAccount: UnlockPortalAccount
        });
    }

    appControllers.service('WhS_BE_PortalAccountAPIService', PortalAccountAPIService);

})(appControllers);