(function (appControllers) {

    'use strict';

    AccountIdentificationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function AccountIdentificationAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService)
    {
        var controllerName = 'AccountIdentification';

        function GetFilteredAccountIdentificationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetFilteredAccountIdentificationRules"), input);
        }

        return {
            GetFilteredAccountIdentificationRules: GetFilteredAccountIdentificationRules
        };
    }

    appControllers.service('Retail_BE_AccountIdentificationAPIService', AccountIdentificationAPIService);

})(appControllers);