﻿(function (appControllers) {

    "use strict";

    bankDetailsEditorController.$inject = ['$scope', 'VRCommon_CityAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_BankDetailAPIService'];

    function bankDetailsEditorController($scope, VRCommon_CityAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRCommon_BankDetailAPIService) {

        var isEditMode;
        var bankDetailEntity;
        var isSingleInsert;

        var currencyDirectiveApi;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                bankDetailEntity = parameters.bankDetailEntity;
                isSingleInsert = parameters.isSingleInsert;

            }
            isEditMode = (bankDetailEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.secondaryAccounts = [];

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

            $scope.scopeModel.addSecondaryAccount = function () {

                var dataItem = {
                    currencyReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                    currencyLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                };

                extendSecondaryAccountDataItem(dataItem);
            };

            $scope.scopeModel.removeSecondaryAccount = function (dataItem) {
                var index = $scope.scopeModel.secondaryAccounts.indexOf(dataItem);
                $scope.scopeModel.secondaryAccounts.splice(index, 1);
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector, loadSecondaryAccounts])
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
            $scope.bankDetailId = bankDetailEntity.BankDetailId;
            $scope.bank = bankDetailEntity.Bank;
            $scope.accountCode = bankDetailEntity.AccountCode;
            $scope.accountHolder = bankDetailEntity.AccountHolder;
            $scope.iban = bankDetailEntity.IBAN;
            $scope.address = bankDetailEntity.Address;
            $scope.accountNumber = bankDetailEntity.AccountNumber;
            $scope.swiftCode = bankDetailEntity.SwiftCode;
            $scope.sortCode = bankDetailEntity.SortCode;
            $scope.channelName = bankDetailEntity.ChannelName;
            $scope.correspondentBank = bankDetailEntity.CorrespondentBank;
            $scope.correspondentBankSwiftCode = bankDetailEntity.CorrespondentBankSwiftCode;
            $scope.ach = bankDetailEntity.ACH;
            $scope.abaRoutingNumber = bankDetailEntity.ABARoutingNumber;
            $scope.moreInfo = bankDetailEntity.MoreInfo;
        }
        function loadSecondaryAccounts() {

            if (bankDetailEntity == undefined || bankDetailEntity.SecondaryAccounts == undefined || bankDetailEntity.SecondaryAccounts.length == 0)
                return;

            var secondaryAccounts = bankDetailEntity.SecondaryAccounts;
            var promises = [];

            for (var j = 0; j < secondaryAccounts.length; j++) {
                var dataItem = {
                    currencyReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                    currencyLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                };
                promises.push(dataItem.currencyLoadPromiseDeferred.promise);

                var secondaryAccountEntity = secondaryAccounts[j];

                extendSecondaryAccountDataItem(dataItem, secondaryAccountEntity);
            }

            return UtilsService.waitMultiplePromises(promises);

        }

        function extendSecondaryAccountDataItem(dataItem, payloadEntity) {
            var selectedCurrencyId;

            if (payloadEntity != undefined) {
                selectedCurrencyId = payloadEntity.CurrencyId;
                dataItem.AccountNumber = payloadEntity.AccountNumber;
            };

            dataItem.onCurrencyDirectiveReady = function (api) {
                dataItem.currencyDirectiveApi = api;
                dataItem.currencyReadyPromiseDeferred.resolve();
            };

            dataItem.currencyReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: selectedCurrencyId
                    };

                    VRUIUtilsService.callDirectiveLoad(dataItem.currencyDirectiveApi, directivePayload, dataItem.currencyLoadPromiseDeferred);
                });

            $scope.scopeModel.secondaryAccounts.push(dataItem);
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
                Bank: $scope.bank,
                CurrencyId: currencyDirectiveApi.getSelectedIds(),
                AccountCode: $scope.accountCode,
                AccountHolder: $scope.accountHolder,
                IBAN: $scope.iban,
                Address: $scope.address,
                AccountNumber: $scope.accountNumber,
                SwiftCode: $scope.swiftCode,
                SortCode: $scope.sortCode,
                ChannelName: $scope.channelName,
                CorrespondentBank: $scope.correspondentBank,
                CorrespondentBankSwiftCode: $scope.correspondentBankSwiftCode,
                ACH: $scope.ach,
                ABARoutingNumber: $scope.abaRoutingNumber,
                MoreInfo: $scope.moreInfo
            };

            var secondaryAccounts;
            if ($scope.scopeModel.secondaryAccounts != undefined && $scope.scopeModel.secondaryAccounts.length > 0) {
                secondaryAccounts = [];

                for (var i = 0; i < $scope.scopeModel.secondaryAccounts.length; i++) {
                    var secondaryAccount = $scope.scopeModel.secondaryAccounts[i];

                    secondaryAccounts.push({
                        CurrencyId: secondaryAccount.currencyDirectiveApi != undefined ? secondaryAccount.currencyDirectiveApi.getSelectedIds() : undefined,
                        AccountNumber: secondaryAccount.AccountNumber
                    });
                }
            }

            obj.SecondaryAccounts = secondaryAccounts;
            return obj;
        }

        function insertBankDetails() {
            var bankDetailsObject = buildBankDetailsObjFromScope();
            bankDetailsObject.BankDetailId = UtilsService.guid();
            if (isSingleInsert == true) {
                return VRCommon_BankDetailAPIService.AddBank(bankDetailsObject)
                    .then(function (response) {
                        if (VRNotificationService.notifyOnItemAdded("Bank", response, "Bank")) {
                            if ($scope.onBankDetailsAdded != undefined)
                                $scope.onBankDetailsAdded(bankDetailsObject);
                            $scope.modalContext.closeModal();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
            }
            else {
                if ($scope.onBankDetailsAdded != undefined)
                    $scope.onBankDetailsAdded(bankDetailsObject);
                $scope.modalContext.closeModal();
            }

        }
        function updateBankDetails() {
            var bankDetailsObject = buildBankDetailsObjFromScope();
            bankDetailsObject.BankDetailId = $scope.bankDetailId;
            if ($scope.onBankDetailsUpdated != undefined)
                $scope.onBankDetailsUpdated(bankDetailsObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VRCommon_BankDetailsEditorController', bankDetailsEditorController);
})(appControllers);
