(function (appControllers) {

    'use strict';

    ExecutionControlDataAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_Queueing_ModuleConfig'];

    function ExecutionControlDataAPIService(BaseAPIService, UtilsService, SecurityService, VR_Queueing_ModuleConfig) {

        return ({            
            IsExecutionPaused: IsExecutionPaused,
            UpdateExecutionPaused: UpdateExecutionPaused
        });

        function IsExecutionPaused() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionControlData', 'IsExecutionPaused'));
        }
    
        function UpdateExecutionPaused(isPaused) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Queueing_ModuleConfig.moduleName, 'ExecutionControlData', 'UpdateExecutionPaused'), { isPaused: isPaused });
        }
        
    }

    appControllers.service('VR_Queueing_ExecutionControlDataAPIService', ExecutionControlDataAPIService);

})(appControllers);
