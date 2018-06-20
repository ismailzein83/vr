(function (appControllers) {

    'use strict';

    RegisteredApplicationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function RegisteredApplicationAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        var controllerName = 'RegisteredApplication';

        return ({
            GetRemoteRegisteredApplicationsInfo: GetRemoteRegisteredApplicationsInfo,
        });

        function GetRemoteRegisteredApplicationsInfo(securityProviderId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetRemoteRegisteredApplicationsInfo'), {
                securityProviderId: securityProviderId,
                serializedFilter: serializedFilter
            });
        }
    }

    appControllers.service('VR_Sec_RegisteredApplicationAPIService', RegisteredApplicationAPIService);

})(appControllers);
