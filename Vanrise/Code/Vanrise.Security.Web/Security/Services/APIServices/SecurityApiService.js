(function (appControllers) {

    'use strict';

    SecurityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function SecurityAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        return ({
            Authenticate: Authenticate,
            ChangePassword: ChangePassword
        });

        function Authenticate(credentialsObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Security', 'Authenticate'), credentialsObject);
        }

        function ChangePassword(changedPasswordObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Security', 'ChangePassword'), changedPasswordObject);
        }
    }

    appControllers.service('VR_Sec_SecurityAPIService', SecurityAPIService);

})(appControllers);
