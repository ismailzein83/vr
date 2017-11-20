(function (appControllers) {

    'use strict';

    ChangeStatusActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function ChangeStatusActionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {
        var controllerName = 'ChangeStatusAction';

        function ChangeAccountStatus(accountBEDefinitionId, accountId, actionDefinitionId, statusChangedDate) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "ChangeAccountStatus"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                actionDefinitionId: actionDefinitionId,
                statusChangedDate: statusChangedDate
            });
        }
        return {
            ChangeAccountStatus: ChangeAccountStatus
        };
    }

    appControllers.service('Retail_BE_ChangeStatusActionAPIService', ChangeStatusActionAPIService);

})(appControllers);