(function (appControllers) {
    "use strict";

    groupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function groupAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {

        function GetFilteredGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Groups", "GetFilteredGroups"), input);
        }

        function GetGroup(groupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Groups", "GetGroup"), {
                groupId: groupId
            });
        }

        function GetGroups() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Groups", "GetGroups"));
        }

        function AddGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Groups", "AddGroup"), group);
        }

        function UpdateGroup(group) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Groups", "UpdateGroup"), group);
        }

        return ({
            GetFilteredGroups: GetFilteredGroups,
            GetGroup: GetGroup,
            GetGroups: GetGroups,
            AddGroup: AddGroup,
            UpdateGroup: UpdateGroup
        });
    }

    appControllers.service('VR_Sec_GroupAPIService', groupAPIService);

})(appControllers);