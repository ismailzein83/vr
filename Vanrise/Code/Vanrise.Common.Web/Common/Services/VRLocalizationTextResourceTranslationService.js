(function (appControllers) {

    "use strict";

    VRLocalizationTextResourceTranslationService.$inject = ['VRCommon_VRLocalizationModuleService', 'VRModalService', 'VRCommon_VRLocalizationTextResourceService'];

    function VRLocalizationTextResourceTranslationService(VRCommon_VRLocalizationModuleService, VRModalService, VRCommon_VRLocalizationTextResourceService) {

        function addVRLocalizationTextResourceTranslation(onVRLocalizationTextResourceTranslationAdded,textResourceId) {
            var settings = {};
            var parameters = {
                textResourceId: textResourceId
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceTranslationAdded = onVRLocalizationTextResourceTranslationAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceTranslationEditor.html', parameters, settings);
        }

        function editVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslationId, textResourceId, onVRLocalizationTextResourceTranslationUpdated) {
            var settings = {};
            var parameters = {
                vrLocalizationTextResourceTranslationId: vrLocalizationTextResourceTranslationId,
                textResourceId: textResourceId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceTranslationUpdated = onVRLocalizationTextResourceTranslationUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceTranslationEditor.html', parameters, settings);
        }
        function registerDrillDownToTextResource() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Translations";
            drillDownDefinition.directive = "vr-common-vrlocalizationtextresourcetranslation-search";
            drillDownDefinition.loadDirective = function (directiveAPI, textResourceItem) {
                textResourceItem.textReasourceTranslationSearchAPI = directiveAPI;
                var query = {
                    TextResourceIds: [textResourceItem.VRLocalizationTextResourceId],
                };

                return textResourceItem.textReasourceTranslationSearchAPI.load(query);
            };

            VRCommon_VRLocalizationTextResourceService.addDrillDownDefinition(drillDownDefinition);
        }

        return {
            addVRLocalizationTextResourceTranslation: addVRLocalizationTextResourceTranslation,
            editVRLocalizationTextResourceTranslation: editVRLocalizationTextResourceTranslation,
            registerDrillDownToTextResource: registerDrillDownToTextResource
           
        };
    }

    appControllers.service('VRCommon_VRLocalizationTextResourceTranslationService', VRLocalizationTextResourceTranslationService);

})(appControllers);