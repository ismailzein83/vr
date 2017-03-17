(function (appService) {

    'use strict';
    SchedulerTaskActionTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig', 'SecurityService'];

    function SchedulerTaskActionTypeAPIService(BaseAPIService, UtilsService, VR_Runtime_ModuleConfig, SecurityService) {
        var controllerName = 'SchedulerTaskActionType';

        function GetSchedulerTaskActionTypes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetSchedulerTaskActionTypes'));
        }
        return ({
           
            GetSchedulerTaskActionTypes: GetSchedulerTaskActionTypes
        });
    }
    appService.service('SchedulerTaskActionTypeAPIService', SchedulerTaskActionTypeAPIService);
})(appService);