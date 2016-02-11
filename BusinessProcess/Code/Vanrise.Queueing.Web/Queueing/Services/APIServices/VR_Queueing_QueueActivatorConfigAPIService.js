(function (appControllers) {

    'use strict';

    QueueActivatorConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueActivatorConfigAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetQueueActivatorsConfig: GetQueueActivatorsConfig
        });

        function GetQueueActivatorsConfig() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueActivatorConfig', 'GetQueueActivatorsConfig'));
        }
    }

    appControllers.service('VR_Queueing_QueueActivatorConfigAPIService', QueueActivatorConfigAPIService);

})(appControllers);
