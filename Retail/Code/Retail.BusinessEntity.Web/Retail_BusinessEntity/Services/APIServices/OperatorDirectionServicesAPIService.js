(function (appControllers) {

    'use strict';

    OperatorDirectionServicesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function OperatorDirectionServicesAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'OperatorDirectionServices';

        function GetMappedCellsExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetMappedCellsExtensionConfigs'));
        }
        return {
            GetMappedCellsExtensionConfigs: GetMappedCellsExtensionConfigs
        };
    }

    appControllers.service('Retail_BE_OperatorDirectionServicesAPIService', OperatorDirectionServicesAPIService);

})(appControllers);