(function (appControllers) {

    'use strict';

    QueueingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueingAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetQueueItemTypes: getQueueItemTypes,
            GetQueueInstances: getQueueInstances,
            GetHeaders: getHeaders,
            GetItemStatusList: getItemStatusList,
            GetQueueItemHeaders: GetQueueItemHeaders
        });
        function getQueueInstances(queueItemTypes) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'Queueing', 'GetQueueInstances'), queueItemTypes);
        }

        function getHeaders(queueIds, fromRow, toRow, statuses, fromDate, toDate) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'Queueing', 'GetHeaders'), {
                FromRow: fromRow,
                ToRow: toRow,
                QueueIds: queueIds,
                Statuses: statuses,
                DateFrom: fromDate,
                DateTo: toDate
            });
        }

        function getQueueItemTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'Queueing', 'GetQueueItemTypes'));
        }

        function getItemStatusList() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'Queueing', 'GetItemStatusList'));
        }

        function GetQueueItemHeaders(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'Queueing', 'GetQueueItemHeaders'), input);
        }
    }

    appControllers.service('VR_Queueing_QueueingAPIService', QueueingAPIService);

})(appControllers);
