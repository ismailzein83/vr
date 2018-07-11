(function (appControllers) {
    "use strict";
    interactionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function interactionAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Interaction";

        function GetMessages() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetMessages"));
        }

        return {
            GetMessages: GetMessages
        };
    };
    appControllers.service("Demo_Module_InteractionAPIService", interactionAPIService);

})(appControllers);