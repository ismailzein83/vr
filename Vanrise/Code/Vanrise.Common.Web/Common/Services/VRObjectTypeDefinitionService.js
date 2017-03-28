
(function (appControllers) {

    "use strict";

    VRObjectTypeDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function VRObjectTypeDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addVRObjectTypeDefinition(onVRObjectTypeDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRObjectTypeDefinitionAdded = onVRObjectTypeDefinitionAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectTypeDefinition/VRObjectTypeDefinitionEditor.html', null, settings);
        };

        function editVRObjectTypeDefinition(vrObjectTypeDefinitionId, onVRObjectTypeDefinitionUpdated) {
            var settings = {};

            var parameters = {
                vrObjectTypeDefinitionId: vrObjectTypeDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRObjectTypeDefinitionUpdated = onVRObjectTypeDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRObjectTypeDefinition/VRObjectTypeDefinitionEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_ObjectTypeDefinition";
        }

        function registerObjectTrackingDrillDownToVRObjectTypeDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrObjectTypeDefinitionItem) {

                vrObjectTypeDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: vrObjectTypeDefinitionItem.Entity.VRObjectTypeDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };

                return vrObjectTypeDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addVRObjectTypeDefinition: addVRObjectTypeDefinition,
            editVRObjectTypeDefinition: editVRObjectTypeDefinition,
            registerObjectTrackingDrillDownToVRObjectTypeDefinition: registerObjectTrackingDrillDownToVRObjectTypeDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_VRObjectTypeDefinitionService', VRObjectTypeDefinitionService);

})(appControllers);