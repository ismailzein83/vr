
(function (appControllers) {

    "use strict";

    VRLocalizationLanguageService.$inject = ['VRModalService'];

    function VRLocalizationLanguageService(VRModalService) {

        function addVRLocalizationLanguage(onVRLocalizationLanguageAdded) {
            var settings = {};
            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationLanguageAdded = onVRLocalizationLanguageAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalizationLanguage/VRLocalizationLanguageEditor.html', parameters, settings);
        };

        function editVRLocalizationLanguage(vrLocalizationLanguageId, onVRLocalizationLanguageUpdated) {
            var settings = {};

            var parameters = {
                vrLocalizationLanguageId: vrLocalizationLanguageId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationLanguageUpdated = onVRLocalizationLanguageUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalizationLanguage/VRLocalizationLanguageEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_Connection";
        }

        return {
            addVRLocalizationLanguage: addVRLocalizationLanguage,
            editVRLocalizationLanguage: editVRLocalizationLanguage
        };
    }

    appControllers.service('VRCommon_VRLocalizationLanguageService', VRLocalizationLanguageService);

})(appControllers);