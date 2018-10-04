(function (appControllers) {

    "use strict";

    BusinessProcess_ProcessSynchronisationService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    var drillDownDefinitions = [];

    function BusinessProcess_ProcessSynchronisationService(VRModalService, VRCommon_ObjectTrackingService) {

        function addProcessSynchronisation(onProcessSynchronisationAdded) {

            var modalParameters = {
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onProcessSynchronisationAdded = onProcessSynchronisationAdded;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/ProcessSynchronisation/ProcessSynchronisationEditor.html', modalParameters, modalSettings);
        }
        function editProcessSynchronisation(processSynchronisationId, onProcessSynchronisationUpdated) {
            var modalParameters = {
                processSynchronisationId: processSynchronisationId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onProcessSynchronisationUpdated = onProcessSynchronisationUpdated;
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/ProcessSynchronisation/ProcessSynchronisationEditor.html', modalParameters, modalSettings);
        }

        function getEntityUniqueName() {
            return "BusinessProcess_ProcessSynchronisation";
        }

        function registerObjectTrackingDrillDownToBPDefinition() {
            var drillDownDefinition = {};
            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, processSynchronisationItem) {
                processSynchronisationItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: processSynchronisationItem.Entity.ProcessSynchronisationId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return processSynchronisationItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);
        }

        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return ({
            addProcessSynchronisation: addProcessSynchronisation,
            editProcessSynchronisation: editProcessSynchronisation,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToBPDefinition: registerObjectTrackingDrillDownToBPDefinition
        });
    }

    appControllers.service('BusinessProcess_ProcessSynchronisationService', BusinessProcess_ProcessSynchronisationService);
})(appControllers);