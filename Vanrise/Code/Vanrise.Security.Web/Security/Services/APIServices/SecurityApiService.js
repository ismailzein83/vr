﻿(function (appControllers) {

    'use strict';

    SecurityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function SecurityAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        var controllerName = 'Security';

        return ({
            Authenticate: Authenticate,
            ChangePassword: ChangePassword,
            IsAllowed: IsAllowed,
            HasPermissionToActions: HasPermissionToActions
        });

        function Authenticate(credentialsObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'Authenticate'), credentialsObject);
        }

        function ChangePassword(changedPasswordObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'ChangePassword'), changedPasswordObject);
        }

        function IsAllowed(requiredPermissions) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'IsAllowed'), {
                requiredPermissions: requiredPermissions
            });
        }

        function HasPermissionToActions(systemActionNames) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'HasPermissionToActions'), { systemActionNames: systemActionNames }, { useCache: true });
        }
    }

    appControllers.service('VR_Sec_SecurityAPIService', SecurityAPIService);

})(appControllers);
