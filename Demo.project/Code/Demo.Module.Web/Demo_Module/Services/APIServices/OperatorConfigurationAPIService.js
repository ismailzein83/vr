(function (appControllers) {

    "use strict";
    operatorConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function operatorConfigurationAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetFilteredOperatorConfigurations(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorConfiguration", "GetFilteredOperatorConfigurations"), input);
        }

        function GetOperatorConfiguration(operatorConfigurationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorConfiguration", "GetOperatorConfiguration"), {
                operatorConfigurationId: operatorConfigurationId
            });

        }
       
        function UpdateOperatorConfiguration(operatorConfigurationObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorConfiguration", "UpdateOperatorConfiguration"), operatorConfigurationObject);
        }
        function AddOperatorConfiguration(operatorConfigurationObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "OperatorConfiguration", "AddOperatorConfiguration"), operatorConfigurationObject);
        }
        return ({
            GetFilteredOperatorConfigurations: GetFilteredOperatorConfigurations,
            GetOperatorConfiguration: GetOperatorConfiguration,
            AddOperatorConfiguration: AddOperatorConfiguration,
            UpdateOperatorConfiguration: UpdateOperatorConfiguration
        });
    }

    appControllers.service('Demo_OperatorConfigurationAPIService', operatorConfigurationAPIService);

})(appControllers);