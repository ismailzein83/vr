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
            HasAddGroupPermission: HasAddGroupPermission,
            HasEditGroupPermission: HasEditGroupPermission,
            GetGroupTemplate: GetGroupTemplate,
            GetGroupHistoryDetailbyHistoryId: GetGroupHistoryDetailbyHistoryId
        });

        function GetFilteredGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetFilteredGroups"), input);
        }

        function GetGroupInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetGroupInfo"), {
                filter: filter
            });
        }
        function GetGroupTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetGroupTemplate"));
        }

        function GetGroup(groupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetGroup"), {
                groupId: groupId
            });
        }

        function GetGroupHistoryDetailbyHistoryId(groupHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetGroupHistoryDetailbyHistoryId'), {
                groupHistoryId: groupHistoryId
            });
        }

        function AddGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "AddGroup"), group);
        }

        function UpdateGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "UpdateGroup"), group);
        }

        function HasAddGroupPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddGroup']));
        }

        function HasEditGroupPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateGroup']));
        }
    }

    appControllers.service('VR_Sec_GroupAPIService', GroupAPIService);

})(appControllers);