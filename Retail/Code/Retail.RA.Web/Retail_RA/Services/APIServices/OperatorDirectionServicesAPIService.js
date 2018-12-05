(function (appControllers) {

    'use strict';

    OperatorDirectionServicesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_RA_ModuleConfig'];

    function OperatorDirectionServicesAPIService(BaseAPIService, UtilsService, Retail_RA_ModuleConfig) {
        var controllerName = 'OperatorDirectionServices';

        function GetMappedCellsExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_RA_ModuleConfig.moduleName, controllerName, 'GetMappedCellsExtensionConfigs'));
        }
        return {
            GetMappedCellsExtensionConfigs: GetMappedCellsExtensionConfigs
        };
    }

    appControllers.service('Retail_RA_OperatorDirectionServicesAPIService', OperatorDirectionServicesAPIService);

})(appControllers);