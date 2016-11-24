(function (appControllers) {

    "use strict";
    codeGroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_NumberingPlan_ModuleConfig', 'SecurityService'];

    function codeGroupAPIService(BaseAPIService, UtilsService, VR_NumberingPlan_ModuleConfig, SecurityService) {

        var controllerName = "CodeGroup";

        function GetFilteredCodeGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetFilteredCodeGroups"), input);
        }
        function GetAllCodeGroups(CodeGroupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetAllCodeGroups"));

        }
        function GetCodeGroup(CodeGroupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "GetCodeGroup"), {
                CodeGroupId: CodeGroupId
            });

        }
        function UpdateCodeGroup(codeGroupObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "UpdateCodeGroup"), codeGroupObject);
        }
        function AddCodeGroup(codeGroupObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "AddCodeGroup"), codeGroupObject);
        }

        function UploadCodeGroupList(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "UploadCodeGroupList"), {
                fileId: fileId
            });
        }
        function DownloadCodeGroupListTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "DownloadCodeGroupListTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        function DownloadCodeGroupLog(fileID) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, "DownloadCodeGroupLog"), { fileID: fileID }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function HasUpdateCodeGroupPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, ['UpdateCodeGroup']));
        }

        function HasAddCodeGroupPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, ['AddCodeGroup']));
        }

        function HasDownloadCodeGroupListTemplatePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, ['DownloadCodeGroupListTemplate']));
        }

        function HasDownloadCodeGroupLogPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, ['DownloadCodeGroupLog']));
        }

        function HasUploadCodeGroupListPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_NumberingPlan_ModuleConfig.moduleName, controllerName, ['UploadCodeGroupList']));
        }

        return ({
            GetFilteredCodeGroups: GetFilteredCodeGroups,
            GetAllCodeGroups: GetAllCodeGroups,
            GetCodeGroup: GetCodeGroup,
            UpdateCodeGroup: UpdateCodeGroup,
            AddCodeGroup: AddCodeGroup,
            DownloadCodeGroupListTemplate: DownloadCodeGroupListTemplate,
            UploadCodeGroupList: UploadCodeGroupList,
            DownloadCodeGroupLog: DownloadCodeGroupLog,
            HasUpdateCodeGroupPermission: HasUpdateCodeGroupPermission,
            HasAddCodeGroupPermission: HasAddCodeGroupPermission,
            HasDownloadCodeGroupListTemplatePermission: HasDownloadCodeGroupListTemplatePermission,
            HasDownloadCodeGroupLogPermission: HasDownloadCodeGroupLogPermission,
            HasUploadCodeGroupListPermission: HasUploadCodeGroupListPermission
        });
    }

    appControllers.service('Vr_NP_CodeGroupAPIService', codeGroupAPIService);

})(appControllers);