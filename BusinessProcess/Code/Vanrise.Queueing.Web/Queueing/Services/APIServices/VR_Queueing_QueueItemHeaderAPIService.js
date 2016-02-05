(function (appControllers) {

    'use strict';

    QueueItemHeaderAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueItemHeaderAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetFilteredQueueItemHeader: GetFilteredQueueItemHeader
        });

        function GetFilteredQueueItemHeader(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueItemHeader', 'GetFilteredQueueItemHeader'), input);
        }

       


    }

    appControllers.service('VR_Queueing_QueueItemHeaderAPIService', QueueItemHeaderAPIService);

})(appControllers);
