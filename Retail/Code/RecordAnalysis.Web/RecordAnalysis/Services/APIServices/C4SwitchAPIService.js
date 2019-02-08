(function (appControllers) {

    'use strict';

    C4SwitchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'RecAnal_ModuleConfig'];

    function C4SwitchAPIService(BaseAPIService, UtilsService, RecAnal_ModuleConfig) {
        var controllerName = 'C4Switch';

        function GetC4SwitchTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(RecAnal_ModuleConfig.moduleName, controllerName, "GetC4SwitchTemplates"));
        }

        return {
            GetC4SwitchTemplates: GetC4SwitchTemplates
        };
    }

    appControllers.service('RecAnal_C4SwitchAPIService', C4SwitchAPIService);

})(appControllers);