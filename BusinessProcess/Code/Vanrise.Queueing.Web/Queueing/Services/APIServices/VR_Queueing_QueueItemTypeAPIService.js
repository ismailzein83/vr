(function (appControllers) {

    'use strict';

    QueueItemTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueItemTypeAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetItemTypes: GetItemTypes,
            GetItemStatusSummary: GetItemStatusSummary
        });

        function GetItemTypes(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueItemType', 'GetItemTypes'), {
                filter: filter
            });
        }

        function GetItemStatusSummary() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueItemType', 'GetItemStatusSummary'));
        }


    }

    appControllers.service('VR_Queueing_QueueItemTypeAPIService', QueueItemTypeAPIService);

})(appControllers);
