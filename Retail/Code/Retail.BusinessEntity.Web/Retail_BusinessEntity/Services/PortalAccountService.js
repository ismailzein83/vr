(function (appControllers) {

    'use stict';

    PortalAccountService.$inject = ['VRModalService'];

    function PortalAccountService(VRModalService) {

        function addPortalAccount(onPortalAccountAdded, accountBEDefinitionId, parentAccountId, accountViewDefinition, context) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId,
                accountViewDefinition: accountViewDefinition,
                context: context,
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onPortalAccountAdded = onPortalAccountAdded
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountViews/PortalAccountEditor.html', parameters, settings);
        };
        function editPortalAccount(accountBEDefinitionId, parentAccountId, accountViewDefinitionId, userId,context, onPortalAccountUpdated) {
            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId,
                accountViewDefinitionId: accountViewDefinitionId,
                userId: userId,
                context: context
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onPortalAccountUpdated = onPortalAccountUpdated
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountViews/PortalAccountEditor.html', parameters, settings);
        };
        function resetPassword(accountBEDefinitionId, parentAccountId, context, userId) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId,
                context: context,
                userId: userId,
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountViews/ResetPasswordEditor.html', parameters, settings);
        };


        return {
            addPortalAccount: addPortalAccount,
            resetPassword: resetPassword,
            editPortalAccount: editPortalAccount
        };
    }

    appControllers.service('Retail_BE_PortalAccountService', PortalAccountService);

})(appControllers);