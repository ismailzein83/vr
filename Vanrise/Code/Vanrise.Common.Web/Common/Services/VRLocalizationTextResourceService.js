
(function (appControllers) {

    "use strict";

    VRLocalizationTextResourceService.$inject = ['VRCommon_VRLocalizationModuleService', 'VRModalService'];

    function VRLocalizationTextResourceService(VRCommon_VRLocalizationModuleService, VRModalService) {
       var drillDownDefinitions = [];

        function addVRLocalizationTextResource(onVRLocalizationTextResourceAdded,moduleId) {
            var settings = {};
            var parameters = {
                moduleId: moduleId
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceAdded = onVRLocalizationTextResourceAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceEditor.html', parameters, settings);
        }

        function editVRLocalizationTextResource(vrLocalizationTextResourceId, onVRLocalizationTextResourceUpdated) {
            var settings = {};
            var parameters = {
                vrLocalizationTextResourceId: vrLocalizationTextResourceId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRLocalizationTextResourceUpdated = onVRLocalizationTextResourceUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRLocalization/TextResource/VRLocalizationTextResourceEditor.html', parameters, settings);
        }

        function registerDrillDowntoModule() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Text Resources";
            drillDownDefinition.directive = "vr-common-localizationtextresource-search";

            drillDownDefinition.loadDirective = function (directiveAPI, moduleItem) {

                moduleItem.textResourceGridAPI = directiveAPI;
                var query = {
                    moduleIds: [moduleItem.VRLocalizationModuleId],
                };
                return moduleItem.textResourceGridAPI.load(query);
            };

            VRCommon_VRLocalizationModuleService.addDrillDownDefinition(drillDownDefinition);
        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addVRLocalizationTextResource: addVRLocalizationTextResource,
            editVRLocalizationTextResource: editVRLocalizationTextResource,
            registerDrillDownToModule: registerDrillDowntoModule,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_VRLocalizationTextResourceService', VRLocalizationTextResourceService);

})(appControllers);