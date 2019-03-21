'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancial', ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountTypeFinancialPartRuntime = new AccountTypeFinancialPartRuntime($scope, ctrl, $attrs);
                accountTypeFinancialPartRuntime.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeFinancialPartRuntimeTemplate.html'
        };

        function AccountTypeFinancialPartRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var currencySelectorAPI;
            var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var productSelectorAPI;
            var productSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var operatorBanksAPI;
            var operatorBanksReadyPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onProductDirectiveReady = function (api) {
                    productSelectorAPI = api;
                    productSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onOperatorBanksReady = function (api) {
                    operatorBanksAPI = api;
                    var setLoader = function (value) { $scope.scopeModel.isLoadingOperatorBanks = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, operatorBanksAPI, undefined, setLoader, operatorBanksReadyPromiseDeferred);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var accountBEDefinitionId;
                    var accountId;
                    var partSettings;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountId = payload.accountId;
                        partSettings = payload.partSettings;

                        if (payload.partDefinition != undefined && payload.partDefinition.Settings != undefined) {
                            $scope.scopeModel.hideProductSelector = payload.partDefinition.Settings.HideProductSelector;
                            $scope.scopeModel.showOperatorBanks = payload.partDefinition.Settings.ShowOperatorBanks;
                            if ($scope.scopeModel.showOperatorBanks) {
                                operatorBanksReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                            }
                        }
                    }

                    //Loading Currency Selector
                    function loadCurrencySelector() {
                        var currencySelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        currencySelectorReadyPromiseDeferred.promise.then(function () {
                            var currencySelectorPayload = partSettings != undefined ? { selectedIds: partSettings.CurrencyId } : undefined;
                            VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadPromiseDeferred);
                        });
                        return currencySelectorLoadPromiseDeferred.promise;
                    }
                

                    //Loading Product Selector
                    function loadProductSelector() {
                        var productSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        productSelectorReadyPromiseDeferred.promise.then(function () {
                            var productSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountDefinitionProductFilter, Retail.BusinessEntity.Business",
                                        AccountBEDefinitionId: accountBEDefinitionId,
                                        AccountId: accountId
                                    }]
                                }
                            };
                            if (partSettings != undefined) {
                                productSelectorPayload.selectedIds = partSettings.ProductId;
                            }
                            VRUIUtilsService.callDirectiveLoad(productSelectorAPI, productSelectorPayload, productSelectorLoadPromiseDeferred);
                        });
                        return productSelectorLoadPromiseDeferred.promise;
                    }

                    function loadOperatorBanks() {
                        var operatorBanksLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        operatorBanksReadyPromiseDeferred.promise.then(function () {
                            operatorBanksReadyPromiseDeferred = undefined;
                            var operatorBanksPayload;
                            if (partSettings != undefined) {
                                operatorBanksPayload = {
                                    BankDetails: partSettings.OperatorBanks
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(operatorBanksAPI, operatorBanksPayload, operatorBanksLoadPromiseDeferred);
                        });
                        return operatorBanksLoadPromiseDeferred.promise;
                    }
                    var promises = [loadCurrencySelector(), loadProductSelector()];

                    if ($scope.scopeModel.showOperatorBanks)
                        promises.push(loadOperatorBanks());
                    var rootPromiseNode = {
                        promises: promises
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancial,Retail.BusinessEntity.MainExtensions',
                        CurrencyId: currencySelectorAPI.getSelectedIds(),
                        ProductId: productSelectorAPI.getSelectedIds(),
                        OperatorBanks: $scope.scopeModel.showOperatorBanks && operatorBanksAPI != undefined ? operatorBanksAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

