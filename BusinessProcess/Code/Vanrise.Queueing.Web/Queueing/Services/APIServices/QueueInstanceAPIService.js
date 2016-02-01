(function (appControllers) {

    'use strict';

    QueueInstanceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueInstanceAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetFilteredQueueInstances: GetFilteredQueueInstances,
            GetStageNames: GetStageNames,
            GetItemTypes: GetItemTypes
        });

        function GetFilteredQueueInstances(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueInstance', 'GetFilteredQueueInstances'),input);
        }

        function GetStageNames() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueInstance', 'GetStageNames'));
        }

        
        function GetItemTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueInstance', 'GetItemTypes'));
        }
       

    }

    appControllers.service('VR_Queueing_QueueInstanceAPIService', QueueInstanceAPIService);

})(appControllers);
