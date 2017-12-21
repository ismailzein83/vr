(function (appControllers) {

    'use stict';

    PortalAccountService.$inject = ['VRModalService'];

    function PortalAccountService(VRModalService) {

        function addPortalAccount(onPortalAccountAdded, accountBEDefinitionId, parentAccountId, context) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId,
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPortalAccountAdded = onPortalAccountAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountViews/PortalAccountEditor.html', parameters, settings);
        };

        function addAddintionalPortalAccount(onPortalAccountAdded, accountBEDefinitionId, parentAccountId, accountViewDefinition) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId,
                accountViewDefinition: accountViewDefinition
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onPortalAccountAdded = onPortalAccountAdded
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountViews/AdditionalPortalAccountEditor.html', parameters, settings);
        };
        function editPortalAccount(portalAccountEntity,onAssignmentDefinitionUpdated) {
            var parameters = {
                portalAccountEntity: portalAccountEntity
            }
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onAssignmentDefinitionUpdated = onAssignmentDefinitionUpdated
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountViews/AdditionalPortalAccountEditor.html', parameters, settings);
        };
        function resetPassword(accountBEDefinitionId, parentAccountId, context, userId, accountViewDefinitionId) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId,
                parentAccountId: parentAccountId,
                context: context,
                userId: userId,
                accountViewDefinitionId: accountViewDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/AccountViews/ResetPasswordEditor.html', parameters, settings);
        };


        return {
            addPortalAccount: addPortalAccount,
            resetPassword: resetPassword,
            addAddintionalPortalAccount: addAddintionalPortalAccount
        };
    }

    appControllers.service('Retail_BE_PortalAccountService', PortalAccountService);

})(appControllers);