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

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/ExtendedSettings/PortalAccountEditor.html', parameters, settings);
        };
        //function editPortalAccount(portalAccountId, onPortalAccountUpdated) {

        //    var parameters = {
        //        portalAccountId: portalAccountId
        //    };

        //    var settings = {};

        //    settings.onScopeReady = function (modalScope) {
        //        modalScope.onPortalAccountUpdated = onPortalAccountUpdated;
        //    };

        //    VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Account/ExtendedSettings/PortalAccountEditor.html', parameters, settings);
        //}

        return {
            addPortalAccount: addPortalAccount,
            //editPortalAccount: editPortalAccount
        };
    }

    appControllers.service('Retail_BE_PortalAccountService', PortalAccountService);

})(appControllers);