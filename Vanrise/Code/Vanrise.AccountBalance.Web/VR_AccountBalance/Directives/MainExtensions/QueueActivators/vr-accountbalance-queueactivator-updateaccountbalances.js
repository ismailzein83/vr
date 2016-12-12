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

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.ongridReady = function (api) {
                    gridAPI = api;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountTypeId;
                    var accountId;
                    var effectiveOn;
                    var amount;
                    var currencyId;
                    var dataRecordTypeId;

                    if (payload != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;

                        if (payload.QueueActivator != undefined) {
                            accountTypeId = payload.QueueActivator.AccountTypeId;
                            accountId = payload.QueueActivator.AccountId;
                            effectiveOn = payload.QueueActivator.EffectiveOn;
                            amount = payload.QueueActivator.Amount;
                            currencyId = payload.QueueActivator.CurrencyId;
                        }
                    }

                    var accountTypeSelectorLoadPromise = getAccountTypeSelectorLoadPromise();
                    promises.push(accountTypeSelectorLoadPromise);

                    var gridLoadPromise = getGridLoadPromise();
                    promises.push(gridLoadPromise);


                    function getAccountTypeSelectorLoadPromise() {
                        var accountTypeLoadDeferred = UtilsService.createPromiseDeferred();

                        accountTypeSelectorReadyDeferred.promise.then(function () {

                            $scope.scopeModel.isAccountTypeSelectorLoading = true;

                            var accountTypeSelectorPayload;
                            if (accountTypeId != undefined) {
                                accountTypeSelectorPayload = {
                                    selectedIds: accountTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeLoadDeferred);
                        });

                        return accountTypeLoadDeferred.promise.then(function () {

                            $scope.scopeModel.isAccountTypeSelectorLoading = false;
                        });;
                    }
                    function getGridLoadPromise() {
                        var promises = [];

                        $scope.scopeModel.isGridLoading = true;

                        $scope.scopeModel.dataRecordFields = [
                            { RecordName: "AccountId", RecordValue: accountId },
                            { RecordName: "EffectiveOn", RecordValue: effectiveOn },
                            { RecordName: "Amount", RecordValue: amount },
                            { RecordName: "CurrencyId", RecordValue: currencyId }
                        ];

                        for (var index = 0; index < $scope.scopeModel.dataRecordFields.length; index++) {
                            var currentItem = $scope.scopeModel.dataRecordFields[index]
                            extendDataRecordFieldObject(currentItem);
                            promises.push(currentItem.dataRecordFieldSelectorLoadDeferred.promise);
                        }

                        return UtilsService.waitMultiplePromises(promises).then(function () {

                            $scope.scopeModel.isGridLoading = false;
                        });
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

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var accountIdEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "AccountId", 'RecordName');
                    var accountId = accountIdEntity && accountIdEntity.dataRecordFieldSelectorAPI ? accountIdEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var effeciveOnEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "EffectiveOn", 'RecordName');
                    var effeciveOn = effeciveOnEntity && effeciveOnEntity.dataRecordFieldSelectorAPI ? effeciveOnEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var amountEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "Amount", 'RecordName');
                    var amount = amountEntity && amountEntity.dataRecordFieldSelectorAPI ? amountEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    var currencyIdEntity = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, "CurrencyId", 'RecordName');
                    var currencyId = currencyIdEntity && currencyIdEntity.dataRecordFieldSelectorAPI ? currencyIdEntity.dataRecordFieldSelectorAPI.getSelectedIds() : undefined;

                    return {
                        $type: 'Vanrise.AccountBalance.MainExtensions.QueueActivators.UpdateAccountBalancesQueueActivator, Vanrise.AccountBalance.MainExtensions',
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        AccountId: accountId,
                        EffectiveOn: effeciveOn,
                        Amount: amount,
                        CurrencyId: currencyId
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrAccountbalanceQueueactivatorUpdateaccountbalances', QueueActivatorUpdateAccountBalances);

})(app);