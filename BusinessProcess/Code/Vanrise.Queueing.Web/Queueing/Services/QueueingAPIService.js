(function (appControllers) {

    "use strict";

    queueingApiService.$inject = ['BaseAPIService'];
    appControllers.service('QueueingAPIService', queueingApiService);

    function queueingApiService(BaseAPIService) {

        function getQueueItemTypes() {
            return BaseAPIService.get("/api/Queueing/GetQueueItemTypes");
        }

        return ({
            GetQueueItemTypes: getQueueItemTypes
        });
    }
    
    
})(appControllers);

