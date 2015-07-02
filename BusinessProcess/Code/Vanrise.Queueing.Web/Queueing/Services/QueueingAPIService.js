(function (appControllers) {

    "use strict";

    queueingApiService.$inject = ['BaseAPIService'];
    appControllers.service('QueueingAPIService', queueingApiService);

    function queueingApiService(BaseAPIService) {

        function getQueueInstances(queueItemTypes) {
            return BaseAPIService.post("/api/Queueing/GetQueueInstances",queueItemTypes);
        }

        function getQueueItemTypes() {
            return BaseAPIService.get("/api/Queueing/GetQueueItemTypes");
        }

        return ({
            GetQueueItemTypes: getQueueItemTypes,
            GetQueueInstances: getQueueInstances
        });
    }
    
    
})(appControllers);

