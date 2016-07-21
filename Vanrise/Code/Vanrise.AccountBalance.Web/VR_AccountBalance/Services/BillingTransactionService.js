(function (appControllers) {

    'use stict';

    BillingTransactionService.$inject = ['VRModalService', 'VRNotificationService'];

    function BillingTransactionService(VRModalService, VRNotificationService) {
        function addBillingTransaction(accountId, onBillingTransactionAdded) {
            var parameters = {
                accountId: accountId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBillingTransactionAdded = onBillingTransactionAdded
            };

            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Views/BillingTransaction/BillingTransactionEditor.html', parameters, settings);
        };

        return {
            addBillingTransaction: addBillingTransaction,
        };
    }

    appControllers.service('VR_AccountBalance_BillingTransactionService', BillingTransactionService);

})(appControllers);