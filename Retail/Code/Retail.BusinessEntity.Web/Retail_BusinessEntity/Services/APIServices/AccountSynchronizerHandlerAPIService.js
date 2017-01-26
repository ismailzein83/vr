(function (appControllers) {

    "use strict";
    accountSynchronizerHandlerAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function accountSynchronizerHandlerAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "AccountSynchronizerHandler";

        function GetAccountSynchronizerInsertHandlerConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetAccountSynchronizerInsertHandlerConfigs'));
        }

        return ({
            GetAccountSynchronizerInsertHandlerConfigs: GetAccountSynchronizerInsertHandlerConfigs
        });
    }

    appControllers.service('Retail_BE_AccountSynchronizerHandlerAPIService', accountSynchronizerHandlerAPIService);

})(appControllers);