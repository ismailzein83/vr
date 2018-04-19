(function (appControllers) {

    "use strict";

    SwitchCommunicationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_RouteSync_ModuleConfig'];

    function SwitchCommunicationAPIService(BaseAPIService, UtilsService, WhS_RouteSync_ModuleConfig) {

        var controllerName = 'SwitchCommunication';

        function GetSwitchCommunicationTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_RouteSync_ModuleConfig.moduleName, controllerName, "GetSwitchCommunicationTemplates"));
        }

        return ({
            GetSwitchCommunicationTemplates: GetSwitchCommunicationTemplates
        });
    }

    appControllers.service('WhS_RouteSync_SwitchCommunicationAPIService', SwitchCommunicationAPIService);

})(appControllers);