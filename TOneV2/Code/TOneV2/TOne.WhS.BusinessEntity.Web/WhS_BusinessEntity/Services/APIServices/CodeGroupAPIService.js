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
        return ({
            GetFilteredCodeGroups: GetFilteredCodeGroups,
            GetAllCodeGroups:GetAllCodeGroups,
            GetCodeGroup: GetCodeGroup,
            UpdateCodeGroup: UpdateCodeGroup,
            AddCodeGroup: AddCodeGroup
        });
    }

    appControllers.service('WhS_BE_CodeGroupAPIService', codeGroupAPIService);

})(appControllers);