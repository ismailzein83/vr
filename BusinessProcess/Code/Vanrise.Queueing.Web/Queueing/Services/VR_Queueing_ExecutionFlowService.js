(function (appControllers) {

    'use strict';

    ExecutionFlowService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function ExecutionFlowService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addExecutionFlow: addExecutionFlow,
            editExecutionFlow: editExecutionFlow,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToQueueExecutionFlow: registerObjectTrackingDrillDownToQueueExecutionFlow

        });

        function addExecutionFlow(onExecutionFlowAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowAdded = onExecutionFlowAdded;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlow/ExecutionFlowEditor.html', null, modalSettings);
        }

        function editExecutionFlow(ID, onExecutionFlowUpdated) {
            var modalParameters = {
                ID: ID
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onExecutionFlowUpdated = onExecutionFlowUpdated;
            };

            VRModalService.showModal('/Client/Modules/Queueing/Views/ExecutionFlow/ExecutionFlowEditor.html', modalParameters, modalSettings);
        }
        function getEntityUniqueName() {
            return "VR_Queueing_QueueExecutionFlow";
        }

        function registerObjectTrackingDrillDownToQueueExecutionFlow() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, queueExecutionFlowItem) {
                queueExecutionFlowItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: queueExecutionFlowItem.Entity.ExecutionFlowId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return queueExecutionFlowItem.objectTrackingGridAPI.load(query);
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

    appControllers.service('VR_Queueing_ExecutionFlowService', ExecutionFlowService);

})(appControllers);
