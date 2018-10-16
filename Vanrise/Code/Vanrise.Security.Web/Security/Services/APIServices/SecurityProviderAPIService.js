﻿(function (appControllers) {

    'use strict';

    SecurityProviderAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function SecurityProviderAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        var controllerName = 'SecurityProvider';

        return ({
            GetSecurityProvidersInfo: GetSecurityProvidersInfo,
            GetRemoteSecurityProvidersInfo: GetRemoteSecurityProvidersInfo,
            GetSecurityProviderInfobyId: GetSecurityProviderInfobyId,
            GetSecurityProviderbyId: GetSecurityProviderbyId,
            GetSecurityProviderConfigs: GetSecurityProviderConfigs,
            GetDefaultSecurityProviderId: GetDefaultSecurityProviderId,
            ChangeSecurityProviderStatus: ChangeSecurityProviderStatus,
            SetDefaultSecurityProvider: SetDefaultSecurityProvider
        });

        function GetSecurityProvidersInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetSecurityProvidersInfo'), {
                filter: filter
            });
        }

        function GetRemoteSecurityProvidersInfo(connectionId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetRemoteSecurityProvidersInfo'), {
                connectionId: connectionId,
                serializedFilter: serializedFilter
            });
        }

        function GetSecurityProviderInfobyId(securityProviderId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetSecurityProviderInfobyId'), {
                securityProviderId: securityProviderId
            });
        }

        function GetSecurityProviderbyId(securityProviderId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetSecurityProviderbyId'), {
                securityProviderId: securityProviderId
            });
        }

        function GetSecurityProviderConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetSecurityProviderConfigs"));
        }

        function GetDefaultSecurityProviderId() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetDefaultSecurityProviderId"));
        }

        function ChangeSecurityProviderStatus(securityProviderId, isEnabled) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "ChangeSecurityProviderStatus"),
                {
                    securityProviderId: securityProviderId,
                    isEnabled: isEnabled
                }
                );
        }
         function SetDefaultSecurityProvider(securityProviderId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "SetDefaultSecurityProvider"),
                {
                    securityProviderId: securityProviderId
                }
            );
        }
    }

    appControllers.service('VR_Sec_SecurityProviderAPIService', SecurityProviderAPIService);

})(appControllers);
