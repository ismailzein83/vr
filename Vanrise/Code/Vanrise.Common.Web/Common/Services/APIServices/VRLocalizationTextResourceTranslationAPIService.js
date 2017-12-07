(function (appControllers) {

    "use strict";

    VRLocalizationTextResourceTranslationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRLocalizationTextResourceTranslationAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = 'VRLocalizationTextResourceTranslation';

        function GetFilteredVRLocalizationTextResourcesTranslation(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRLocalizationTextResourcesTranslation'), input);
        }
        function AddVRLocalizationTextResourceTranslation(vrLocalizationTextResourcetranslation) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddVRLocalizationTextResourceTranslation"), vrLocalizationTextResourcetranslation);
        }

        function UpdateVRLocalizationTextResourceTranslation(vrLocalizationTextResourcetranslation) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateVRLocalizationTextResourceTranslation"), vrLocalizationTextResourcetranslation);
        }
        function GetVRLocalizationTextResourceTranslation(vrLocalizationTextResourcetranslationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRLocalizationTextResourceTranslation"), { vrLocalizationTextResourcetranslationId: vrLocalizationTextResourcetranslationId });
        }
        return ({
            GetFilteredVRLocalizationTextResourcesTranslation: GetFilteredVRLocalizationTextResourcesTranslation,
            UpdateVRLocalizationTextResourceTranslation: UpdateVRLocalizationTextResourceTranslation,
            AddVRLocalizationTextResourceTranslation: AddVRLocalizationTextResourceTranslation,
            GetVRLocalizationTextResourceTranslation: GetVRLocalizationTextResourceTranslation
        });

    }
    appControllers.service('VRCommon_VRLocalizationTextResourceTranslationAPIService', VRLocalizationTextResourceTranslationAPIService);

})(appControllers);