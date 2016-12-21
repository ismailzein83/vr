(function (appControllers) {

    'use strict';

    AccountConditionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountConditionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'AccountCondition';


        function GetAccountConditionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetAccountConditionConfigs"));
        }


        return {
            GetAccountConditionConfigs: GetAccountConditionConfigs
        };
    }

    appControllers.service('Retail_BE_AccountConditionAPIService', AccountConditionAPIService);

})(appControllers);