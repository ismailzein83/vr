'use strict';

app.directive('retailZajilPaymentConvertorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailZajilPaymentConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Zajil/Directives/MainExtensions/Payment/Templates/PaymentConvertorEditor.html"
        };

        function retailZajilPaymentConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var transactionTypeDirectiveAPI;
            var transactionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencySelectorAPI;
            var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var companyProfileDefinitionSelectorAPI;
            var companyProfileDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onCompanyProfileDefinitionSelectorReady = function (api) {
                companyProfileDefinitionSelectorAPI = api;
                companyProfileDefinitionSelectorReadyDeferred.resolve();
            };
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.amountColumn = payload.AmountColumn;
                        $scope.scopeModel.timeColumn = payload.TimeColumn;
                        $scope.scopeModel.accountColumn = payload.SourceAccountIdColumn;
                        $scope.scopeModel.sourceIdColumn = payload.SourceIdColumn;
                        $scope.scopeModel.invoiceSourceIdColumn = payload.InvoiceSourceIdColumn;
                    }

                    var promises = [];

                    var loadTransactionTypePromise = getBillingTransactionTypeSelectorPromise();
                    promises.push(loadTransactionTypePromise);

                    var loadAccountDefinitionTypePromise = getAccountDefinitionSelectorLoadPromise();
                    promises.push(loadAccountDefinitionTypePromise);

                    promises.push(loadCurrencySelector());

                    promises.push(loadCompanyProfileDefinitionSelector());

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

                    function loadCurrencySelector() {
                        var currencyLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload;
                        if (payload != undefined) {
                            selectorPayload = {
                                selectedIds: payload.CurrencyId
                            }
                        }
                        currencySelectorReadyDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, selectorPayload, currencyLoadPromiseDeferred);
                            });
                        return currencyLoadPromiseDeferred.promise;
                    };

                    function loadCompanyProfileDefinitionSelector() {
                        var companyProfileSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        companyProfileDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('F6630722-4E85-4DF2-915F-F9942074743C')
                                },
                                selectedIds: payload != undefined ? payload.ZajilCompanyExtendedInfoId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(companyProfileDefinitionSelectorAPI, selectorPayload, companyProfileSelectorLoadDeferred);
                        });
                        return companyProfileSelectorLoadDeferred.promise;
                    };


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.Zajil.MainExtensions.Convertors.PaymentConvertor, Retail.Zajil.MainExtensions",
                        Name: "Zajil Payment Convertor",
                        TransactionTypeId: transactionTypeDirectiveAPI.getSelectedIds(),
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        CurrencyId: currencySelectorAPI.getSelectedIds(),
                        AmountColumn: $scope.scopeModel.amountColumn,
                        TimeColumn: $scope.scopeModel.timeColumn,
                        SourceAccountIdColumn: $scope.scopeModel.accountColumn,
                        SourceIdColumn: $scope.scopeModel.sourceIdColumn,
                        InvoiceSourceIdColumn: $scope.scopeModel.invoiceSourceIdColumn,
                        ZajilCompanyExtendedInfoId: companyProfileDefinitionSelectorAPI.getSelectedIds()
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