
(function (appControllers) {

    'use stict';

    StatusDefinitionService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function StatusDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addStatusDefinition(onStatusDefinitionAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusDefinitionAdded = onStatusDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Common/Views/StatusDefinition/StatusDefinitionEditor.html', null, settings);
        };

        function editStatusDefinition(statusDefinitionId, onStatusDefinitionUpdated) {
            var settings = {};

            var parameters = {
                statusDefinitionId: statusDefinitionId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onStatusDefinitionUpdated = onStatusDefinitionUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/StatusDefinition/StatusDefinitionEditor.html', parameters, settings);
        }
        function getEntityUniqueName() {
            return "VR_Common_StatusDefinition";
        }

        function registerObjectTrackingDrillDownToStatusDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, statusDefinitionItem) {

                statusDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: statusDefinitionItem.Entity.StatusDefinitionId,
                    EntityUniqueName: getEntityUniqueName(),

                };

                return statusDefinitionItem.objectTrackingGridAPI.load(query);
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
            addStatusDefinition: addStatusDefinition,
            editStatusDefinition: editStatusDefinition,
            registerObjectTrackingDrillDownToStatusDefinition: registerObjectTrackingDrillDownToStatusDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VR_Common_StatusDefinitionService', StatusDefinitionService);

})(appControllers);