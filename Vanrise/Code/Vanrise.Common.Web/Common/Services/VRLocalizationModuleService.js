
(function (appControllers) {

    "use strict";

    VRLocalizationModuleService.$inject = ['VRModalService'];

    function VRLocalizationModuleService(VRModalService) {

        function addVRLocalizationModule(onVRLocalizationModuleAdded) {
            var settings = {};
            var parameters = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationModuleAdded = onVRLocalizationModuleAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/Module/VRLocalizationModuleEditor.html', parameters, settings);
        }

        function editVRLocalizationModule(vrLocalizationModuleId, onVRLocalizationModuleUpdated) {
            var settings = {};
            var parameters = {
                vrLocalizationModuleId: vrLocalizationModuleId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationModuleUpdated = onVRLocalizationModuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/Module/VRLocalizationModuleEditor.html', parameters, settings);
        }

        function getEntityUniqueName() {
            return "VR_Common_Connection";
        }

        return {
            addVRLocalizationModule: addVRLocalizationModule,
            editVRLocalizationModule: editVRLocalizationModule
        };
    }

    appControllers.service('VRCommon_VRLocalizationModuleService', VRLocalizationModuleService);

})(appControllers);