
(function (appControllers) {

    "use strict";
    CodecProfileAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function CodecProfileAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "CodecProfile";


        function GetFilteredCodecProfiles(input) {
             return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredCodecProfiles'), input);
        }

        function GetCodecProfile(CodecProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetCodecProfile'), {
                CodecProfileId: CodecProfileId
            });
        }

        function AddCodecProfile(CodecProfileItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'AddCodecProfile'), CodecProfileItem);
        }

        function UpdateCodecProfile(CodecProfileItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'UpdateCodecProfile'), CodecProfileItem);
        }

        function GetCodecProfileEditorRuntime(CodecProfileId) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetCodecProfileEditorRuntime'), {
                CodecProfileId: CodecProfileId
            });
        }
         
        function GetCodecProfilesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, "GetCodecProfilesInfo"), {
                filter: filter
            });
        }

        function HasAddCodecProfilePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['AddCodecProfile']));
        }

        function HasEditCodecProfilePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['UpdateCodecProfile']));
        }


        return ({
            GetFilteredCodecProfiles: GetFilteredCodecProfiles,
            GetCodecProfile: GetCodecProfile,
            GetCodecProfileEditorRuntime: GetCodecProfileEditorRuntime,
            AddCodecProfile: AddCodecProfile,
            UpdateCodecProfile: UpdateCodecProfile,
            GetCodecProfilesInfo: GetCodecProfilesInfo,
            HasAddCodecProfilePermission: HasAddCodecProfilePermission,
            HasEditCodecProfilePermission: HasEditCodecProfilePermission,
        });
    }

    appControllers.service('NP_IVSwitch_CodecProfileAPIService', CodecProfileAPIService);

})(appControllers);