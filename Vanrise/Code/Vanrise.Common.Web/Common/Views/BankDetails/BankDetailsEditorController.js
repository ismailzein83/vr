(function (appControllers) {

    "use strict";

    bankDetailsEditorController.$inject = ['$scope', 'VRCommon_CityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function bankDetailsEditorController($scope, VRCommon_CityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var bankDetailEntity;

        var currencyDirectiveApi;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                bankDetailEntity = parameters.bankDetailEntity;
            }
            isEditMode = (bankDetailEntity != undefined);
        }

        function defineScope() {

            $scope.onCurrencyDirectiveReady = function (api) {
                currencyDirectiveApi = api;
                currencyReadyPromiseDeferred.resolve();
            };

            $scope.saveBankDetail = function () {
                if (isEditMode)
                    return updateBankDetails();
                else
                    return insertBankDetails();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.isLoading = true;
            loadAllControls()
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && bankDetailEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(bankDetailEntity.Bank, "Bank Details");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Bank Details");
        }

        function loadStaticData() {

            if (bankDetailEntity == undefined)
                return;
            $scope.bank = bankDetailEntity.Bank;
            $scope.accountCode= bankDetailEntity.AccountCode;
            $scope.accountHolder= bankDetailEntity.AccountHolder;
            $scope.iban= bankDetailEntity.IBAN;
            $scope.address= bankDetailEntity.Address;
            $scope.accountNumber= bankDetailEntity.AccountNumber;
            $scope.swiftCode= bankDetailEntity.SwiftCode;
            $scope.sortCode= bankDetailEntity.SortCode;

        }

        function loadCurrencySelector() {
            var currencyLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            currencyReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: bankDetailEntity != undefined ? bankDetailEntity.CurrencyId : undefined
                    };

                    VRUIUtilsService.callDirectiveLoad(currencyDirectiveApi, directivePayload, currencyLoadPromiseDeferred);
                });
            return currencyLoadPromiseDeferred.promise;
        }

        function buildBankDetailsObjFromScope() {
            var obj = {
                Bank:$scope.bank,
                CurrencyId: currencyDirectiveApi.getSelectedIds(),
                AccountCode: $scope.accountCode,
                AccountHolder: $scope.accountHolder,
                IBAN: $scope.iban,
                Address: $scope.address,
                AccountNumber: $scope.accountNumber,
                SwiftCode:$scope.swiftCode,
                SortCode : $scope.sortCode,
            };
            return obj;
        }

        function insertBankDetails() {
            var bankDetailsObject = buildBankDetailsObjFromScope();
            if ($scope.onBankDetailsAdded != undefined)
                $scope.onBankDetailsAdded(bankDetailsObject);
            $scope.modalContext.closeModal();
        }
        function updateBankDetails() {
            var bankDetailsObject = buildBankDetailsObjFromScope();
            if ($scope.onBankDetailsUpdated != undefined)
                $scope.onBankDetailsUpdated(bankDetailsObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VRCommon_BankDetailsEditorController', bankDetailsEditorController);
})(appControllers);
