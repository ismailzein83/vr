(function (appControllers) {

    'use strict';

    ExecutionFlowDefinitionService.$inject = ['VRModalService','VRCommon_ObjectTrackingService'];

    function ExecutionFlowDefinitionService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addExecutionFlowDefinition: addExecutionFlowDefinition,
            editExecutionFlowDefiniton: editExecutionFlowDefiniton,
            registerObjectTrackingDrillDownToQueueExecutionFlowDefinition: registerObjectTrackingDrillDownToQueueExecutionFlowDefinition,
            getDrillDownDefinition: getDrillDownDefinition
        });

        function addExecutionFlowDefinition(onExecutionFlowDefinitionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowDefinitionAdded = onExecutionFlowDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionEditor.html', null, modalSettings);
        }

        function editExecutionFlowDefiniton(ID, onExecutionFlowDefinitionUpdated) {
            var modalParameters = {
                ID: ID
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowDefinitionUpdated = onExecutionFlowDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlowDefinition/ExecutionFlowDefinitionEditor.html', modalParameters, modalSettings);
        }

        function getEntityUniqueName() {
            return "VR_Queueing_QueueExecutionFlowDefinition";
        }

        function registerObjectTrackingDrillDownToQueueExecutionFlowDefinition() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, queueExecutionFlowDefinitionItem) {
                queueExecutionFlowDefinitionItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: queueExecutionFlowDefinitionItem.Entity.ID,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return queueExecutionFlowDefinitionItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
 
    };

    appControllers.service('VR_Queueing_ExecutionFlowDefinitionService', ExecutionFlowDefinitionService);

})(appControllers);
