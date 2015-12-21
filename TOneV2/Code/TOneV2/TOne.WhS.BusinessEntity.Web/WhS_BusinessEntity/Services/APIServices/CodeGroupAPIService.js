(function (appControllers) {

    "use strict";
    codeGroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function codeGroupAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredCodeGroups(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "GetFilteredCodeGroups"), input);
        }
        function GetAllCodeGroups(CodeGroupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "GetAllCodeGroups"));

        }
        function GetCodeGroup(CodeGroupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "GetCodeGroup"), {
                CodeGroupId: CodeGroupId
            });

        }
        function UpdateCodeGroup(codeGroupObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "UpdateCodeGroup"), codeGroupObject);
        }
        function AddCodeGroup(codeGroupObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "AddCodeGroup"), codeGroupObject);
        }

        function UploadCodeGroupList(fileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "UploadCodeGroupList"), {
                fileId: fileId,
            });
        }
        function DownloadCodeGroupListTemplate() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "DownloadCodeGroupListTemplate"), {}, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }
        function DownloadCodeGroupLog(fileID) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CodeGroup", "DownloadCodeGroupLog"), { fileID: fileID }, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        return ({
            GetFilteredCodeGroups: GetFilteredCodeGroups,
            GetAllCodeGroups:GetAllCodeGroups,
            GetCodeGroup: GetCodeGroup,
            UpdateCodeGroup: UpdateCodeGroup,
            AddCodeGroup: AddCodeGroup,
            DownloadCodeGroupListTemplate: DownloadCodeGroupListTemplate,
            UploadCodeGroupList: UploadCodeGroupList,
            DownloadCodeGroupLog: DownloadCodeGroupLog
        });
    }

    appControllers.service('WhS_BE_CodeGroupAPIService', codeGroupAPIService);

})(appControllers);