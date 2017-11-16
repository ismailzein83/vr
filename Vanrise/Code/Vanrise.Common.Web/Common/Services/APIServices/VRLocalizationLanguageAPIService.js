(function (appControllers) {

    "use strict";

    VRLocalizationLanguageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRLocalizationLanguageAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = 'VRLocalizationLanguage';

        function GetFilteredVRLocalizationLanguages(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRLocalizationLanguages'), input);
        }
        function GetVRLocalizationLanguage(vrLocalizationLanguageId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRLocalizationLanguage'), {
                vrLocalizationLanguage: vrLocalizationLanguageId
            });
        }
        function GetVRLocalizationLanguageInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRLocalizationLanguageInfo"));
        }

        function AddVRLocalizationLanguage(vrLocalizationLanguageItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddVRLocalizationLanguage"), vrLocalizationLanguageItem);
        }
        function UpdateVRLocalizationLanguage(vrLocalizationLanguageItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateVRLocalizationLanguage"), vrLocalizationLanguageItem);
        }

        return ({
            GetFilteredVRLocalizationLanguages: GetFilteredVRLocalizationLanguages,
            GetVRLocalizationLanguage: GetVRLocalizationLanguage,
            GetVRLocalizationLanguageInfo: GetVRLocalizationLanguageInfo,
            AddVRLocalizationLanguage: AddVRLocalizationLanguage,
            UpdateVRLocalizationLanguage: UpdateVRLocalizationLanguage
        });

    }
    appControllers.service('VRCommon_VRLocalizationLanguageAPIService', VRLocalizationLanguageAPIService);

})(appControllers);