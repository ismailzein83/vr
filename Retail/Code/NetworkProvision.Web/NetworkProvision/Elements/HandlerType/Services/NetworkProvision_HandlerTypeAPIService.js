(function (appControllers) {

    'use strict';

    handlerTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NetworkProvision_ModuleConfig'];

    function handlerTypeAPIService(BaseAPIService, UtilsService, NetworkProvision_ModuleConfig) {

        var controllerName = 'HandlerType';

        function GetHandlerTypeExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(NetworkProvision_ModuleConfig.moduleName, controllerName, 'GetHandlerTypeExtendedSettingsConfigs'));
        }

        function TryCompileCustomCode(customCodeObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(NetworkProvision_ModuleConfig.moduleName, controllerName, 'TryCompileCustomCode'), customCodeObj);
        }

        return {
            GetHandlerTypeExtendedSettingsConfigs: GetHandlerTypeExtendedSettingsConfigs,
            TryCompileCustomCode: TryCompileCustomCode
        };
    }

    appControllers.service('NetworkProvision_HandlerTypeAPIService', handlerTypeAPIService);

})(appControllers);