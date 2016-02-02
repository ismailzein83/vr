(function (appControllers) {

    'use strict';

    QueueInstanceService.$inject = ['VR_Queueing_ExecutionFlowService'];

    function QueueInstanceService(VR_Queueing_ExecutionFlowService) {
        var drillDownDefinitions = [];
        return ({
            registerDrillDownToExecutionFlow: registerDrillDownToExecutionFlow
        });

        function registerDrillDownToExecutionFlow() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Queue Instances";
            drillDownDefinition.directive = "vr-queueinstance-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, ExecutionFlowItem) {
                ExecutionFlowItem.queueInstanceGridAPI = directiveAPI;
                var query = {
                    ExecutionFlowId: [ExecutionFlowItem.Entity.ExecutionFlowId],
                };

                return ExecutionFlowItem.queueInstanceGridAPI.loadGrid(query);
            };

            VR_Queueing_ExecutionFlowService.addDrillDownDefinition(drillDownDefinition);
        }
    

    };

    appControllers.service('VR_Queueing_QueueInstanceService', QueueInstanceService);

})(appControllers);
