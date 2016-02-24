(function (appControllers) {
    "use strict";

    GroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'VR_Sec_SecurityAPIService'];

    function GroupAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, VR_Sec_SecurityAPIService) {
        return ({
            GetFilteredGroups: GetFilteredGroups,
            GetGroupInfo: GetGroupInfo,
            GetGroup: GetGroup,
            AddGroup: AddGroup,
            UpdateGroup: UpdateGroup,
            HasAddGroupPermission: HasAddGroupPermission
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

        function HasAddGroupPermission() {
            return VR_Sec_SecurityAPIService.IsAllowed(VR_Sec_ModuleConfig.moduleName + '/Group/AddGroup');
        }
    }

    appControllers.service('VR_Sec_GroupAPIService', GroupAPIService);

})(appControllers);