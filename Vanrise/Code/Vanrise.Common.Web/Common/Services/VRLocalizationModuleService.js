
(function (appControllers) {

    "use strict";

    VRLocalizationModuleService.$inject = ['VRModalService'];

    function VRLocalizationModuleService(VRModalService) {

        var drillDownDefinitions = [];

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

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addVRLocalizationModule: addVRLocalizationModule,
            editVRLocalizationModule: editVRLocalizationModule,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
        };
    }

    appControllers.service('VRCommon_VRLocalizationModuleService', VRLocalizationModuleService);

})(appControllers);