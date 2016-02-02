(function (appControllers) {

    'use strict';

    ExecutionFlowService.$inject = ['VRModalService'];

    function ExecutionFlowService(VRModalService) {
        var drillDownDefinitions = [];
        return ({
            addExecutionFlow: addExecutionFlow,
            editExecutionFlow: editExecutionFlow,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition
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

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
 
    };

    appControllers.service('VR_Queueing_ExecutionFlowService', ExecutionFlowService);

})(appControllers);
