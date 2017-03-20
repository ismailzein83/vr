(function (app) {

    'use strict';
    SchedulerTaskActionTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig'];

    function SchedulerTaskActionTypeAPIService(BaseAPIService, UtilsService, VR_Runtime_ModuleConfig) {
        var controllerName = 'SchedulerTaskActionType';

        function GetSchedulerTaskActionTypes(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Runtime_ModuleConfig.moduleName, controllerName, 'GetSchedulerTaskActionTypes'), {
                filter:filter
            });
        }

        return ({
            GetSchedulerTaskActionTypes: GetSchedulerTaskActionTypes
        });
    }
    app.service('VR_Runtime_SchedulerTaskActionTypeAPIService', SchedulerTaskActionTypeAPIService);
})(app);