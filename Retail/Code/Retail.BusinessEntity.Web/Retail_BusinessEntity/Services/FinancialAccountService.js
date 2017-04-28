(function (appControllers) {

    'use strict';

    FinancialAccountService.$inject = ['UtilsService', 'VRModalService'];

    function FinancialAccountService(UtilsService, VRModalService) {
        return ({
            addFinancialAccount: addFinancialAccount,
            editFinancialAccount: editFinancialAccount,
        });

        function addFinancialAccount(onFinancialAccountAdded, accountBEDefinitionId, accountId) {
            var modalParameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountAdded = onFinancialAccountAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/FinancialAccount/FinancialAccountEditor.html', modalParameters, modalSettings);
        }

        function editFinancialAccount(onFinancialAccountUpdated, accountBEDefinitionId,accountId, sequenceNumber) {
            var modalParameters = {
                accountBEDefinitionId:accountBEDefinitionId,
                sequenceNumber: sequenceNumber,
                accountId: accountId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountUpdated = onFinancialAccountUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/FinancialAccount/FinancialAccountEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('Retail_BE_FinancialAccountService', FinancialAccountService);

})(appControllers);
