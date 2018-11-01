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
        return ({
            GetCarrierProfilePortalAccounts: GetCarrierProfilePortalAccounts,
            AddPortalAccount: AddPortalAccount,
            UpdatePortalAccount: UpdatePortalAccount,
            GetPortalAccount: GetPortalAccount,
            ResetPassword: ResetPassword
        });
    }

    appControllers.service('WhS_BE_PortalAccountAPIService', PortalAccountAPIService);

})(appControllers);