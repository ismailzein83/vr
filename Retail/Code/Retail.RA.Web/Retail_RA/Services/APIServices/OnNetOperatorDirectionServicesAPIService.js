(function (appControllers) {

    'use strict';

    OnNetOperatorDirectionServicesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_RA_ModuleConfig'];

    function OnNetOperatorDirectionServicesAPIService(BaseAPIService, UtilsService, Retail_RA_ModuleConfig) {
        var controllerName = 'OnNetOperatorDirectionServices';

        function GetMappedCellsExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_RA_ModuleConfig.moduleName, controllerName, 'GetMappedCellsExtensionConfigs'));
        }
        return {
            GetMappedCellsExtensionConfigs: GetMappedCellsExtensionConfigs
        };
    }

    appControllers.service('Retail_RA_OnNetOperatorDirectionServicesAPIService', OnNetOperatorDirectionServicesAPIService);

})(appControllers);