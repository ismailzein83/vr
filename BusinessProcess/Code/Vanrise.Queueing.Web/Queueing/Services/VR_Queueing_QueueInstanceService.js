(function (appControllers) {

    'use strict';

    QueueInstanceService.$inject = ['VR_Queueing_ExecutionFlowService','VR_Queueing_QueueItemStatusEnum'];

    function QueueInstanceService(VR_Queueing_ExecutionFlowService, VR_Queueing_QueueItemStatusEnum) {
        var drillDownDefinitions = [];
        return ({
            registerDrillDownToExecutionFlow: registerDrillDownToExecutionFlow,
            getQueueItemStatus: getQueueItemStatus
        });

        function registerDrillDownToExecutionFlow() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Queue Instances";
            drillDownDefinition.directive = "vr-queueing-queueinstance-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, ExecutionFlowItem) {
                ExecutionFlowItem.queueInstanceGridAPI = directiveAPI;
                var query = {
                    ExecutionFlowId: [ExecutionFlowItem.Entity.ExecutionFlowId],
                };

                return ExecutionFlowItem.queueInstanceGridAPI.loadGrid(query);
            };

            VR_Queueing_ExecutionFlowService.addDrillDownDefinition(drillDownDefinition);
        }

        function getQueueItemStatus() {
            var queueItemStatus = [];
            for (var key in VR_Queueing_QueueItemStatusEnum) {
                if (VR_Queueing_QueueItemStatusEnum.hasOwnProperty(key)) {
                    queueItemStatus.push({ Id: VR_Queueing_QueueItemStatusEnum[key].value, Name: key });
                }
            }
            
            
            return queueItemStatus;
        }
    

    };

    appControllers.service('VR_Queueing_QueueInstanceService', QueueInstanceService);

})(appControllers);
