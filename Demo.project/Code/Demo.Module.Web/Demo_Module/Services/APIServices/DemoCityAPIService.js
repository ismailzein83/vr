(function (appControllers) {
    "use strict";
    demoCityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function demoCityAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "DemoCity";


        function GetDemoCityById(demoCityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDemoCityById"),
                {
                    demoCityId: demoCityId
                });
        }

        function GetDemoCitiesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetDemoCitiesInfo"), { filter: filter });
        };


        return {
            GetDemoCityById: GetDemoCityById,
            GetDemoCitiesInfo: GetDemoCitiesInfo,
        };
    };
    appControllers.service("Demo_Module_DemoCityAPIService", demoCityAPIService);
    
})(appControllers);