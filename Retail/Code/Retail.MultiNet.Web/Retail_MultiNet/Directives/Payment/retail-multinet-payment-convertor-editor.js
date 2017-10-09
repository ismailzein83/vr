'use strict';

app.directive('retailMultinetPaymentConvertorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailMultiNetPaymentConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_MultiNet/Directives/Payment/Templates/PaymentConvertorEditor.html"
        };

        function retailMultiNetPaymentConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var transactionTypeDirectiveAPI;
            var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencySelectorAPI;
            var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var invoiceTypeIdSelectorApi;
            var invoiceTypeIdSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                invoiceTypeIdSelectorApi = api;
                invoiceTypeIdSelectorPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.amountColumn = payload.AmountColumn;
                        $scope.scopeModel.timeColumn = payload.PaymentDateColumn;
                        $scope.scopeModel.accountColumn = payload.SourceAccountIdColumn;
                        $scope.scopeModel.sourceIdColumn = payload.SourceIdColumnName;
                        $scope.scopeModel.invoiceSourceIdColumn = payload.InvoiceSourceIdColumn;
                        $scope.scopeModel.currencyColumn = payload.CurrencyColumnName;
                        $scope.scopeModel.referenceColumnName = payload.ReferenceColumnName;
                        $scope.scopeModel.invoiceTypeId = payload.InvoiceTypeId;
                    }

                    var promises = [];

                    var loadTransactionTypePromise = getBillingTransactionTypeSelectorPromise();
                    promises.push(loadTransactionTypePromise);

                    var loadAccountDefinitionTypePromise = getAccountDefinitionSelectorLoadPromise();
                    promises.push(loadAccountDefinitionTypePromise);

                    var loadInvoiceTypePromise = getInvoiceTypeSelectorPromise();
                    promises.push(loadInvoiceTypePromise);


                    function getAccountDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    function getBillingTransactionTypeSelectorPromise() {
                        var billingTransactionTypeLoadDeferred = UtilsService.createPromiseDeferred();
                        transactionTypeDirectiveReadyDeferred.promise.then(function () {
                            var selectorPayload;
                            if (payload != undefined) {
                                selectorPayload = {
                                    selectedIds: payload.TransactionTypeId
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(transactionTypeDirectiveAPI, selectorPayload, billingTransactionTypeLoadDeferred);
                        });
                        return billingTransactionTypeLoadDeferred.promise;
                    };

                    function getInvoiceTypeSelectorPromise() {
                        var invoiceTypeLoadDeferred = UtilsService.createPromiseDeferred();
                        invoiceTypeIdSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload;
                            if (payload != undefined) {
                                selectorPayload = {
                                    selectedIds: payload.InvoiceTypeId
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(invoiceTypeIdSelectorApi, selectorPayload, invoiceTypeLoadDeferred);
                        });
                        return invoiceTypeLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.MultiNet.Business.Convertors.PaymentConvertor, Retail.MultiNet.Business",
                        Name: "MultiNet Payment Convertor",
                        TransactionTypeId: transactionTypeDirectiveAPI.getSelectedIds(),
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        CurrencyColumnName: $scope.scopeModel.currencyColumn,
                        AmountColumn: $scope.scopeModel.amountColumn,
                        PaymentDateColumn: $scope.scopeModel.timeColumn,
                        SourceAccountIdColumn: $scope.scopeModel.accountColumn,
                        SourceIdColumnName: $scope.scopeModel.sourceIdColumn,
                        InvoiceSourceIdColumn: $scope.scopeModel.invoiceSourceIdColumn,
                        ReferenceColumnName: $scope.scopeModel.referenceColumnName,
                        InvoiceTypeId: invoiceTypeIdSelectorApi.getSelectedIds(),
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getAccountPartDefinitionIds(id) {
                var partDefinitionIds = [];
                partDefinitionIds.push(id);
                return partDefinitionIds;
            }

        }

        return directiveDefinitionObject;
    }]);