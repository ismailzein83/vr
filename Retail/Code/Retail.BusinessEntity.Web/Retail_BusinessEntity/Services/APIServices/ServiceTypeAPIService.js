(function (appControllers) {

    'use strict';

    ServiceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function ServiceTypeAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'ServiceType';

        function GetFilteredServiceTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredServiceTypes'), input);
        }

        function GetServiceType(serviceTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetServiceType'), {
                serviceTypeId: serviceTypeId
            });
        }

        function UpdateServiceType(serviceType) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateServiceType'), serviceType);
        }

        return {
            GetFilteredServiceTypes: GetFilteredServiceTypes,
            GetServiceType: GetServiceType,
            UpdateServiceType: UpdateServiceType,
        };
    }

    appControllers.service('Retail_BE_ServiceTypeAPIService', ServiceTypeAPIService);

})(appControllers);