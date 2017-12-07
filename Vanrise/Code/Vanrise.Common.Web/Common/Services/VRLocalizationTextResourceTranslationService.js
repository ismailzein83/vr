(function (appControllers) {

    "use strict";

    VRLocalizationTextResourceTranslationService.$inject = ['VRCommon_VRLocalizationModuleService', 'VRModalService'];

    function VRLocalizationTextResourceTranslationService(VRCommon_VRLocalizationModuleService, VRModalService) {

        function addVRLocalizationTextResourceTranslation(onVRLocalizationTextResourceTranslationAdded) {
            var settings = {};
            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceTranslationAdded = onVRLocalizationTextResourceTranslationAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceTranslationEditor.html', parameters, settings);
        }

        function editVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslationId, onVRLocalizationTextResourceTranslationUpdated) {
            var settings = {};
            var parameters = {
                vrLocalizationTextResourceTranslationId: vrLocalizationTextResourceTranslationId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceTranslationUpdated = onVRLocalizationTextResourceTranslationUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceTranslationEditor.html', parameters, settings);
        }


        return {
            addVRLocalizationTextResourceTranslation: addVRLocalizationTextResourceTranslation,
            editVRLocalizationTextResourceTranslation: editVRLocalizationTextResourceTranslation,
           
        };
    }

    appControllers.service('VRCommon_VRLocalizationTextResourceTranslationService', VRLocalizationTextResourceTranslationService);

})(appControllers);