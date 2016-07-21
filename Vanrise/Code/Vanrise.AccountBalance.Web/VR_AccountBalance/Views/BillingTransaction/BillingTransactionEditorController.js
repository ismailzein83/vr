(function (appControllers) {

    'use strict';

    BillingTransactionController.$inject = ['$scope', 'VR_AccountBalance_BillingTransactionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function BillingTransactionController($scope, VR_AccountBalance_BillingTransactionAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var accountId;

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                accountId = parameters.accountId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return  insertBillingTransaction();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
           $scope.title = UtilsService.buildTitleForAddEditor('Payment');
        }

        function loadStaticData() {
           
        }

        function loadCurrencySelector() {
            var currencyLoadDeferred = UtilsService.createPromiseDeferred();
            currencySelectorReadyDeferred.promise.then(function () {
                var currencySelectorPayload;
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencyLoadDeferred);
            });
            return currencyLoadDeferred.promises;
        }
        
        function insertBillingTransaction() {
            $scope.scopeModel.isLoading = true;

            var billingTransactionObj = buildBuillingTransactionObjFromScope();

            return VR_AccountBalance_BillingTransactionAPIService.AddBillingTransaction(billingTransactionObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Payment', response, 'Name')) {
                    if ($scope.onBillingTransactionAdded != undefined)
                        $scope.onBillingTransactionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildBuillingTransactionObjFromScope() {
            var obj = {
                AccountId: accountId,
                Amount: $scope.scopeModel.amount,
                CurrencyId: currencySelectorAPI.getSelectedIds(),
                Notes: $scope.scopeModel.notes
            };
            return obj;
        }
    }

    appControllers.controller('VR_AccountBalance_BillingTransactionController', BillingTransactionController);

})(appControllers);