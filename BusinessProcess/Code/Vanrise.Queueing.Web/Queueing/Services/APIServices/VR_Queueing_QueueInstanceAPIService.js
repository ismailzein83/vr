(function (appControllers) {

    'use strict';

    QueueInstanceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueInstanceAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetFilteredQueueInstances: GetFilteredQueueInstances,
            GetQueueInstances: GetQueueInstances
        });

        function GetFilteredQueueInstances(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueInstance', 'GetFilteredQueueInstances'),input);
        }

        function GetQueueInstances(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueInstance', 'GetQueueInstances'), filter);
        }

        
       
        
    }

    appControllers.service('VR_Queueing_QueueInstanceAPIService', QueueInstanceAPIService);

})(appControllers);
