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


        return {
            addPortalAccount: addPortalAccount
        };
    }

    appControllers.service('Retail_BE_PortalAccountService', PortalAccountService);

})(appControllers);