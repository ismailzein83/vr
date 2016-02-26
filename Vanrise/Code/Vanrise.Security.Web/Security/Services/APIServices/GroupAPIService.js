(function (appControllers) {
    "use strict";

    GroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'SecurityService'];

    function GroupAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, SecurityService) {
        var controllerName = 'Group';

        return ({
            GetFilteredGroups: GetFilteredGroups,
            GetGroupInfo: GetGroupInfo,
            GetGroup: GetGroup,
            AddGroup: AddGroup,
            UpdateGroup: UpdateGroup,
            HasAddGroupPermission: HasAddGroupPermission
        });

        function GetFilteredGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetFilteredGroups"), input);
        }

        function GetGroupInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetGroupInfo"), {
                filter: filter
            });
        }

        function GetGroup(groupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetGroup"), {
                groupId: groupId
            });
        }

        function AddGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "AddGroup"), group);
        }

        function UpdateGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "UpdateGroup"), group);
        }

        function HasAddGroupPermission() {
            return SecurityService.IsAllowedBySystemActionNames(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddGroup']));
        }
    }

    appControllers.service('VR_Sec_GroupAPIService', GroupAPIService);

})(appControllers);