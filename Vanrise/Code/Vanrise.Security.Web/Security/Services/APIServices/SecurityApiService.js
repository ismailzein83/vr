(function (appControllers) {

    'use strict';

    SecurityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function SecurityAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        var controllerName = 'Security';

        return ({
            Authenticate: Authenticate,
            TryRenewCurrentSecurityToken: TryRenewCurrentSecurityToken,
            ChangePassword: ChangePassword,
            IsAllowed: IsAllowed,
            HasPermissionToActions: HasPermissionToActions,
            GetPasswordValidationInfo: GetPasswordValidationInfo,
            GetRemotePasswordValidationInfo: GetRemotePasswordValidationInfo,
            ChangeExpiredPassword: ChangeExpiredPassword,
            RedirectToApplication: RedirectToApplication
        });

        function Authenticate(authenticateObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'Authenticate2'), authenticateObject);
        }

        function TryRenewCurrentSecurityToken() {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'TryRenewCurrentSecurityToken'), null);
        }

        function ChangePassword(changedPasswordObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ChangePassword'), changedPasswordObject);
        }

        function ChangeExpiredPassword(changeExpiredPasswordObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ChangeExpiredPassword'), changeExpiredPasswordObject);
        }

        function IsAllowed(requiredPermissions) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'IsAllowed'), {
                requiredPermissions: requiredPermissions
            });
        }

        function HasPermissionToActions(systemActionNames) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'HasPermissionToActions'), { systemActionNames: systemActionNames }, { useCache: true });
        }

        function GetPasswordValidationInfo(securityProviderId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetPasswordValidationInfo'), {
                securityProviderId: securityProviderId
            });
        }

        function GetRemotePasswordValidationInfo(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetRemotePasswordValidationInfo'), input);
        }

        function RedirectToApplication(applicationURL) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'RedirectToApplication'), {
                applicationURL: applicationURL
            });
        }
    }

    appControllers.service('VR_Sec_SecurityAPIService', SecurityAPIService);

})(appControllers);
