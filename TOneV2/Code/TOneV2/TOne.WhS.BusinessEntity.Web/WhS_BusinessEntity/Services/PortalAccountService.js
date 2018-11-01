(function (appControllers) {

    'use stict';

    PortalAccountService.$inject = ['VRModalService'];

    function PortalAccountService(VRModalService) {

        function addPortalAccount(onPortalAccountAdded, carrierProfileId, context) {
            var parameters = {
                carrierProfileId: carrierProfileId,
                context: context
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onPortalAccountAdded = onPortalAccountAdded;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PortalAccount/PortalAccountEditor.html', parameters, settings);
        };
        function editPortalAccount(carrierProfileId, userId, context, onPortalAccountUpdated) {
            var parameters = {
                carrierProfileId: carrierProfileId,
                userId: userId,
                context: context
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onPortalAccountUpdated = onPortalAccountUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PortalAccount/PortalAccountEditor.html', parameters, settings);
        };
        function resetPassword(carrierProfileId, context, userId) {
            var parameters = {
                carrierProfileId: carrierProfileId,
                context: context,
                userId: userId
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PortalAccount/ResetPasswordEditor.html', parameters, settings);
        };


        return {
            addPortalAccount: addPortalAccount,
            resetPassword: resetPassword,
            editPortalAccount: editPortalAccount
        };
    }

    appControllers.service('WhS_BE_PortalAccountService', PortalAccountService);

})(appControllers);