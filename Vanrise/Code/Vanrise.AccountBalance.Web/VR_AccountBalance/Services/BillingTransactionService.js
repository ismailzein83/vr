(function (appControllers) {

    'use stict';

    BillingTransactionService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService'];

    function BillingTransactionService(VRModalService, VRNotificationService, UtilsService) {
        function addBillingTransaction(accountId, accountTypeId, onBillingTransactionAdded) {
            var parameters = {
                accountId: accountId,
                accountTypeId: accountTypeId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBillingTransactionAdded = onBillingTransactionAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Views/BillingTransaction/BillingTransactionEditor.html', parameters, settings);
        };


        function viewBillingTransaction(billingTransactionId, context) {
            var parameters = {
                billingTransactionId: billingTransactionId,
                context: context
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };

            VRModalService.showModal('/Client/Modules/VR_AccountBalance/Views/BillingTransaction/BillingTransactionEditor.html', parameters, settings);
        }
        return {
            addBillingTransaction: addBillingTransaction,
            viewBillingTransaction: viewBillingTransaction
        };
    }

    appControllers.service('VR_AccountBalance_BillingTransactionService', BillingTransactionService);

})(appControllers);