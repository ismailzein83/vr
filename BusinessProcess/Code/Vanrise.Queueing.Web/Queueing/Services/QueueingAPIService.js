(function (appControllers) {

    "use strict";

    queueingApiService.$inject = ['BaseAPIService'];
    appControllers.service('QueueingAPIService', queueingApiService);

    function queueingApiService(BaseAPIService) {

        function getQueueInstances(queueItemTypes) {
            return BaseAPIService.post("/api/Queueing/GetQueueInstances",queueItemTypes);
        }

        function getHeaders(queueIds, fromRow, toRow, statuses, fromDate,toDate) {
            return BaseAPIService.post("/api/Queueing/GetHeaders", {
                FromRow: fromRow,
                ToRow: toRow,
                QueueIds: queueIds,
                Statuses: statuses,
                DateFrom: fromDate,
                DateTo: toDate
            });
        }

        function getQueueItemTypes() {
            return BaseAPIService.get("/api/Queueing/GetQueueItemTypes");
        }

        function getItemStatusList() {
            return BaseAPIService.get("/api/Queueing/GetItemStatusList");
        }

        return ({
            GetQueueItemTypes: getQueueItemTypes,
            GetQueueInstances: getQueueInstances,
            GetHeaders: getHeaders,
            GetItemStatusList: getItemStatusList
        });
    }
    
    
})(appControllers);

