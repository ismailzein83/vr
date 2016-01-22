(function (appControllers) {
    "use strict";

    GroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function GroupAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        return ({
            GetFilteredGroups: GetFilteredGroups,
            GetGroupInfo: GetGroupInfo,
            GetGroup: GetGroup,
            AddGroup: AddGroup,
            UpdateGroup: UpdateGroup
        });

        function GetFilteredGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Group", "GetFilteredGroups"), input);
        }

        function GetGroupInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Group", "GetGroupInfo"), {
                filter: filter
            });
        }

        function GetGroup(groupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Group", "GetGroup"), {
                groupId: groupId
            });
        }

        function AddGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Group", "AddGroup"), group);
        }

        function UpdateGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Group", "UpdateGroup"), group);
        }
    }

    appControllers.service('VR_Sec_GroupAPIService', GroupAPIService);

})(appControllers);