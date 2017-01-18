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

            $scope.scopeModel = {};

            $scope.scopeModel.onBillingTransactionTypeReady = function (api) {
                transactionTypeDirectiveAPI = api;
                transactionTypeDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var loadTransactionTypePromise = getBillingTransactionTypeSelectorPromise();
                    promises.push(loadTransactionTypePromise);

                    var loadAccountDefinitionTypePromise = getAccountDefinitionSelectorLoadPromise();
                    promises.push(loadAccountDefinitionTypePromise);

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
                    }

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
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.Zajil.MainExtensions.Convertors.PaymentConvertor, Retail.Zajil.MainExtensions",
                        Name: "Zajil Payment Convertor",
                        TransactionTypeId: transactionTypeDirectiveAPI.getSelectedIds(),
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            
        }

        return directiveDefinitionObject;
    }]);