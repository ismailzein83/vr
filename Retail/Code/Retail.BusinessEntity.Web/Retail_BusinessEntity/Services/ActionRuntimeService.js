﻿(function (appControllers) {

    'use stict';

    ActionRuntimeService.$inject = ['VRModalService'];

    function ActionRuntimeService(VRModalService) {

        function openActionRuntime(accountBEDefinitionId, accountActionDefinition,accountId, onActionExecuted) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onActionExecuted =  onActionExecuted
            };
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountActionDefinition: accountActionDefinition,
                accountId:accountId,
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Action/Runtime/ActionRuntimeEditor.html', parameters, settings);
        };

        return {
            openActionRuntime: openActionRuntime,
        };
    }

    appControllers.service('Retail_BE_ActionRuntimeService', ActionRuntimeService);

})(appControllers);