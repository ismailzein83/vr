(function (app) {

    'use strict';

    QueueActivatorUpdateAccountBalances.$inject = ['UtilsService', 'VRUIUtilsService'];

    function QueueActivatorUpdateAccountBalances(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new QueueActivatorUpdateAccountBalancesCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/QueueActivators/Templates/QueueActivatorUpdateAccountBalancesTemplate.html';
            }
        };

        function QueueActivatorUpdateAccountBalancesCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var balanceAccountTypeSelectorAPI;
            var balanceAccountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var billingTransactionTypeSelectorAPI;
            var billingTransactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var updateAccountBalanceSettingsDirectiveAPI;
            var updateAccountBalanceSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isBalanceAccountTypeSelectorRequired = false;

                $scope.scopeModel.onBalanceAccountTypeSelectorReady = function (api) {
                    balanceAccountTypeSelectorAPI = api;
                    balanceAccountTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onBillingTransactionTypeSelectorReady = function (api) {
                    billingTransactionTypeSelectorAPI = api;
                    billingTransactionTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onUpdateAccountBalanceSettingsDirectiveReady = function (api) {
                    updateAccountBalanceSettingsDirectiveAPI = api;
                    updateAccountBalanceSettingsDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.isDataRecordFieldRequired = function (dataItem) {

                    if (dataItem.RecordName == "BalanceAccountTypeId" && $scope.scopeModel.selectedBalanceAccountType != undefined)
                        return false;

                    if (dataItem.RecordName == "TransactionTypeId" && $scope.scopeModel.selectedBillingTransactionType != undefined)
                        return false;

                    return true;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var dataRecordTypeId;
                    var balanceAccountTypeId;
                    var transactionTypeId;
                    var accountIdFieldName;
                    var effectiveOnFieldName;
                    var amountFieldName;
                    var currencyIdFieldName;
                    var balanceAccountTypeIdFieldName;
                    var transactionTypeIdFieldName;
                    var updateAccountBalanceSettings;

                    if (payload != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;

                        if (payload.QueueActivator != undefined) {
                            balanceAccountTypeId = payload.QueueActivator.BalanceAccountTypeId;
                            transactionTypeId = payload.QueueActivator.TransactionTypeId;
                            accountIdFieldName = payload.QueueActivator.AccountIdFieldName;
                            effectiveOnFieldName = payload.QueueActivator.EffectiveOnFieldName;
                            amountFieldName = payload.QueueActivator.AmountFieldName;
                            currencyIdFieldName = payload.QueueActivator.CurrencyIdFieldName;
                            balanceAccountTypeIdFieldName = payload.QueueActivator.BalanceAccountTypeIdFieldName;
                            transactionTypeIdFieldName = payload.QueueActivator.TransactionTypeIdFieldName;
                            updateAccountBalanceSettings = payload.QueueActivator.UpdateAccountBalanceSettings;
                        }
                    }

                    var balanceAccountTypeSelectorLoadPromise = getBalanceAccountTypeSelectorLoadPromise();
                    promises.push(balanceAccountTypeSelectorLoadPromise);

                    var billingTransactionTypeSelectorLoadPromise = getBillingTransactionTypeSelectorLoadPromise();
                    promises.push(billingTransactionTypeSelectorLoadPromise);

                    var gridLoadPromise = getGridLoadPromise();
                    promises.push(gridLoadPromise);

                    var updateAccountBalanceSettingsDirectiveLoadPromise = getUpdateAccountBalanceSettingsDirectiveLoadPromise();
                    promises.push(updateAccountBalanceSettingsDirectiveLoadPromise);


                    function getBalanceAccountTypeSelectorLoadPromise() {
                        var balanceAccountTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        balanceAccountTypeSelectorReadyDeferred.promise.then(function () {

                            $scope.scopeModel.isAccountTypeSelectorLoading = true;

                            var balanceAccountTypeSelectorPayload;
                            if (balanceAccountTypeId != undefined) {
                                balanceAccountTypeSelectorPayload = {
                                    selectedIds: balanceAccountTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(balanceAccountTypeSelectorAPI, balanceAccountTypeSelectorPayload, balanceAccountTypeLoadDeferred);
                        });

                        return balanceAccountTypeLoadDeferred.promise.then(function () {
                            $scope.scopeModel.isAccountTypeSelectorLoading = false;
                        });
                    }

                    function getBillingTransactionTypeSelectorLoadPromise() {
                        var billingTransactionTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        billingTransactionTypeSelectorReadyDeferred.promise.then(function () {

                            var billingTransactionTypeSelectorPayload;
                            if (transactionTypeId != undefined) {
                                billingTransactionTypeSelectorPayload = {
                                    selectedIds: transactionTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(billingTransactionTypeSelectorAPI, billingTransactionTypeSelectorPayload, billingTransactionTypeLoadDeferred);
                        });

                        return billingTransactionTypeLoadDeferred.promise;
                    }

                    function getGridLoadPromise() {
                        var promises = [];

                        $scope.scopeModel.isGridLoading = true;

                        $scope.scopeModel.dataRecordFields = [
                         { RecordName: "AccountId", RecordValue: accountIdFieldName },
                         { RecordName: "EffectiveOn", RecordValue: effectiveOnFieldName },
                         { RecordName: "Amount", RecordValue: amountFieldName },
                         { RecordName: "CurrencyId", RecordValue: currencyIdFieldName },
                         { RecordName: "BalanceAccountTypeId", RecordValue: balanceAccountTypeIdFieldName },
                         { RecordName: "TransactionTypeId", RecordValue: transactionTypeIdFieldName }
                        ];

                        for (var index = 0; index < $scope.scopeModel.dataRecordFields.length; index++) {
                            var currentItem = $scope.scopeModel.dataRecordFields[index];
                            extendDataRecordFieldObject(currentItem);
                            promises.push(currentItem.dataRecordFieldSelectorLoadDeferred.promise);
                        }

                        function extendDataRecordFieldObject(dataRecordField) {

                            dataRecordField.dataRecordFieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            dataRecordField.onDataRecordFieldSelectorReady = function (api) {
                                dataRecordField.dataRecordFieldSelectorAPI = api;

                                var dataRecordFieldPayload;
                                if (dataRecordField != undefined) {
                                    dataRecordFieldPayload = {
                                        selectedIds: dataRecordField.RecordValue,
                                        dataRecordTypeId: dataRecordTypeId
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(dataRecordField.dataRecordFieldSelectorAPI, dataRecordFieldPayload, dataRecordField.dataRecordFieldSelectorLoadDeferred);
                            };
                        }

                        return UtilsService.waitMultiplePromises(promises).then(function () {
                            $scope.scopeModel.isGridLoading = false;
                        });
                    }

                    function getUpdateAccountBalanceSettingsDirectiveLoadPromise() {
                        var updateAccountBalanceSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        updateAccountBalanceSettingsDirectiveReadyDeferred.promise.then(function () {

                            var payload;
                            if (updateAccountBalanceSettings != undefined) {
                                payload = {
                                    updateAccountBalanceSettings: updateAccountBalanceSettings
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(updateAccountBalanceSettingsDirectiveAPI, payload, updateAccountBalanceSettingsDirectiveLoadDeferred);
                        });

                        return updateAccountBalanceSettingsDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var accountIdEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "AccountId", 'RecordName');
                    var accountIdFieldName = accountIdEntity && accountIdEntity.dataRecordFieldSelectorAPI ? accountIdEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var effeciveOnEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "EffectiveOn", 'RecordName');
                    var effeciveOnFieldName = effeciveOnEntity && effeciveOnEntity.dataRecordFieldSelectorAPI ? effeciveOnEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var amountEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "Amount", 'RecordName');
                    var amountFieldName = amountEntity && amountEntity.dataRecordFieldSelectorAPI ? amountEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var currencyIdEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "CurrencyId", 'RecordName');
                    var currencyIdFieldName = currencyIdEntity && currencyIdEntity.dataRecordFieldSelectorAPI ? currencyIdEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var balanceAccountTypeIdEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "BalanceAccountTypeId", 'RecordName');
                    var balanceAccountTypeIdFieldName = balanceAccountTypeIdEntity && balanceAccountTypeIdEntity.dataRecordFieldSelectorAPI ? balanceAccountTypeIdEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var transactionTypeIdEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "TransactionTypeId", 'RecordName');
                    var transactionTypeIdFieldName = transactionTypeIdEntity && transactionTypeIdEntity.dataRecordFieldSelectorAPI ? transactionTypeIdEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var obj = {
                        $type: 'Vanrise.AccountBalance.MainExtensions.QueueActivators.UpdateAccountBalancesQueueActivator, Vanrise.AccountBalance.MainExtensions',
                        BalanceAccountTypeId: balanceAccountTypeSelectorAPI.getSelectedIds(),
                        TransactionTypeId: billingTransactionTypeSelectorAPI.getSelectedIds(),
                        AccountIdFieldName: accountIdFieldName,
                        EffectiveOnFieldName: effeciveOnFieldName,
                        AmountFieldName: amountFieldName,
                        CurrencyIdFieldName: currencyIdFieldName,
                        BalanceAccountTypeIdFieldName: balanceAccountTypeIdFieldName,
                        TransactionTypeIdFieldName: transactionTypeIdFieldName,
                        UpdateAccountBalanceSettings: updateAccountBalanceSettingsDirectiveAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrAccountbalanceQueueactivatorUpdateaccountbalances', QueueActivatorUpdateAccountBalances);

})(app);