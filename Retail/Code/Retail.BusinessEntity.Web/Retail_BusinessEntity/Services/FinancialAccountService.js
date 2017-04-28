(function (appControllers) {

    'use strict';

    FinancialAccountService.$inject = ['UtilsService', 'VRModalService'];

    function FinancialAccountService(UtilsService, VRModalService) {
        return ({
            addFinancialAccount: addFinancialAccount,
            editFinancialAccount: editFinancialAccount,
        });

        function addFinancialAccount(onFinancialAccountAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountAdded = onFinancialAccountAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/FinancialAccount/Templates/FinancialAccountEditor.html', modalParameters, modalSettings);
        }

        function editFinancialAccount(onFinancialAccountUpdated,financialAccountId,accountBEDefinitionId) {
            var modalParameters = {
                accountBEDefinitionId:accountBEDefinitionId,
                financialAccountId: financialAccountId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onFinancialAccountUpdated = onFinancialAccountUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/FinancialAccount/Templates/FinancialAccountEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('Retail_BE_FinancialAccountService', FinancialAccountService);

})(appControllers);
