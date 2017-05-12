'use strict';

app.directive('vrAccountbalanceBillingtransactionSynchronizer', ['UtilsService', 'VRUIUtilsService', 'VRCommon_VRObjectVariableService', 'VRCommon_VRObjectTypeDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRObjectVariableService, VRCommon_VRObjectTypeDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new billingTransactionSynchronizerEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/MainExtensions/BillingTransaction/Templates/BillingTransactionSynchronizer.html"
        };

        function billingTransactionSynchronizerEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountTypeSelectorApi;
            var accountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var transactionTypeDirectiveAPI;
            var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var invoiceTypeSelectorAPI;
            var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            $scope.scopeModel = {};

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorApi = api;
                accountTypeSelectorPromiseDeferred.resolve();
            };
            $scope.scopeModel.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                invoiceTypeSelectorAPI = api;
                invoiceTypeSelectorReadyDeferred.resolve();
            };
            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceTypeId;
                    if (payload != undefined) {
                        $scope.scopeModel.checkExisting = payload.CheckExisting;
                        $scope.scopeModel.updateInvoicePaidDate = payload.UpdateInvoicePaidDate;
                        invoiceTypeId = payload.InvoiceTypeId;
                    }
                    var promises = [];

                    promises.push(loadAccountTypeSelector());
                    promises.push(loadBillingTransactionTypeSelectorPromise());

                    function loadAccountTypeSelector() {
                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountTypeSelectorPromiseDeferred.promise.then(function () {
                            var payloadSelector = {
                                selectedIds: payload != undefined ? payload.BalanceAccountTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorApi, payloadSelector, accountTypeSelectorLoadDeferred);
                        });
                        return accountTypeSelectorLoadDeferred.promise;
                    }

                    function loadBillingTransactionTypeSelectorPromise() {
                        var billingTransactionTypeLoadDeferred = UtilsService.createPromiseDeferred();
                        transactionTypeDirectiveReadyDeferred.promise.then(function () {
                            var selectorPayload;
                            if (payload != undefined) {
                                selectorPayload = {
                                    selectedIds: payload.BillingTransactionTypeIds
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(transactionTypeDirectiveAPI, selectorPayload, billingTransactionTypeLoadDeferred);
                        });
                        return billingTransactionTypeLoadDeferred.promise;
                    };


                    var invoiceTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(invoiceTypeSelectorPayloadLoadDeferred.promise);

                    invoiceTypeSelectorReadyDeferred.promise.then(function () {
                        var invoiceTypeSelectorPayload;
                        if (invoiceTypeId != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: invoiceTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, invoiceTypeSelectorPayload, invoiceTypeSelectorPayloadLoadDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.AccountBalance.MainExtensions.BillingTransaction.BillingTransactionSynchronizer, Vanrise.AccountBalance.MainExtensions",
                        Name: "Billing Transaction Synchronizer",
                        BalanceAccountTypeId: accountTypeSelectorApi.getSelectedIds(),
                        CheckExisting: $scope.scopeModel.checkExisting,
                        BillingTransactionTypeIds: $scope.scopeModel.checkExisting ? transactionTypeDirectiveAPI.getSelectedIds() : undefined,
                        UpdateInvoicePaidDate: $scope.scopeModel.updateInvoicePaidDate,
                        InvoiceTypeId: $scope.scopeModel.updateInvoicePaidDate ? invoiceTypeSelectorAPI.getSelectedIds() : undefined,

                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);