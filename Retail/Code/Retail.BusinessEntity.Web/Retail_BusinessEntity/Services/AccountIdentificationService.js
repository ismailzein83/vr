(function (appControllers) {

    'use stict';

    AccountIdentificationService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountIdentificationService(VRModalService, VRNotificationService)
    {

        function assignIdentificationRuleToAccount(accountBEDefinitionId, accountId, onAccountIdentificationRuleAdded)
        {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountIdentificationRuleAdded = onAccountIdentificationRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountIdentificationAssignmentEditor.html', parameters, settings);
        };

        return {
            assignIdentificationRuleToAccount: assignIdentificationRuleToAccount
        };
    }

    appControllers.service('Retail_BE_AccountIdentificationService', AccountIdentificationService);

})(appControllers);