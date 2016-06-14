(function (appControllers) {

    'use stict';

    AccountIdentificationService.$inject = ['VRModalService', 'VRNotificationService'];

    function AccountIdentificationService(VRModalService, VRNotificationService)
    {

        function assignIdentificationRuleToAccount(accountId, onAccountIdentificationRuleAssigned)
        {
            var parameters = {
                accountId: accountId,
                parentAccountId: parentAccountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAccountIdentificationRuleAssigned = onAccountIdentificationRuleAssigned;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/AccountEditor.html', parameters, settings);
        };

        return {
            assignIdentificationRuleToAccount: assignIdentificationRuleToAccount
        };
    }

    appControllers.service('Retail_BE_AccountIdentificationService', AccountIdentificationService);

})(appControllers);