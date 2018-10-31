(function (appControllers) {
    "use strict";
    pageRunTimeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function pageRunTimeAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "PageRunTime";

        function GetFilteredPageRunTimes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredPageRunTimes"), input);
        }

        function AddPageRunTime(pageRunTime) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddPageRunTime"), pageRunTime);
        };

        function GetPageRunTimeById(pageRunTimeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetPageRunTimeById"),
                {
                    pageRunTimeId: pageRunTimeId
                });
        }

        function UpdatePageRunTime(pageRunTime) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdatePageRunTime"), pageRunTime);
        }

        return {
            GetFilteredPageRunTimes:GetFilteredPageRunTimes,
            AddPageRunTime: AddPageRunTime,
            GetPageRunTimeById: GetPageRunTimeById,
            UpdatePageRunTime: UpdatePageRunTime
       };
    };
    appControllers.service("Demo_Module_PageRunTimeAPIService", pageRunTimeAPIService);

})(appControllers);