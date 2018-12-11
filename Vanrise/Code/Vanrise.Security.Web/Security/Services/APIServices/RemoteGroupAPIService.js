(function (appControllers) {
    "use strict";

    RemoteGroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'SecurityService'];

    function RemoteGroupAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, SecurityService) {
        var controllerName = 'RestGroup';

        return ({
            GetRemoteGroupInfo: GetRemoteGroupInfo,
            GetRemoteAssignedUserGroups: GetRemoteAssignedUserGroups
        });


        function GetRemoteGroupInfo(connectionId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetRemoteGroupInfo"), {
                connectionId: connectionId,
                filter: filter
            });
        }
        function GetRemoteAssignedUserGroups(connectionId, userId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetRemoteAssignedUserGroups"), {
                connectionId: connectionId,
                userId: userId
            });
        }
        
    }

    appControllers.service('VR_Sec_RemoteGroupAPIService', RemoteGroupAPIService);

})(appControllers);