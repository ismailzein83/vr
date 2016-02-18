(function (appControllers) {

    'use strict';

    QueueItemHeaderAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueItemHeaderAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetFilteredQueueItemHeader: GetFilteredQueueItemHeader,
            GetItemStatusSummary: GetItemStatusSummary,
            GetExecutionFlowStatusSummary: GetExecutionFlowStatusSummary
        });

        function GetFilteredQueueItemHeader(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueItemHeader', 'GetFilteredQueueItemHeader'), input);
        }


        function GetItemStatusSummary() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueItemHeader', 'GetItemStatusSummary'));
        }

        function GetExecutionFlowStatusSummary() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueItemHeader', 'GetExecutionFlowStatusSummary'));
        }


    }

    appControllers.service('VR_Queueing_QueueItemHeaderAPIService', QueueItemHeaderAPIService);

})(appControllers);
