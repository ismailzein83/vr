(function (appControllers) {

    'use strict';

    QueueStageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Queueing_ModuleConfig'];

    function QueueStageAPIService(BaseAPIService, UtilsService, VR_Queueing_ModuleConfig) {
        return ({
            GetStageNames: GetStageNames
        });


        function GetStageNames(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'QueueStage', 'GetStageNames'),
                { filter: filter });
        }

    }

    appControllers.service('VR_Queueing_QueueStageAPIService', QueueStageAPIService);

})(appControllers);
