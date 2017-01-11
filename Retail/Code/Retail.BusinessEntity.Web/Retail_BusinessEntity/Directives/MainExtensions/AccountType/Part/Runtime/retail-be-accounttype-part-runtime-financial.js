'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancial', ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
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

            var creditClassSelectorAPI;
            var creditClassReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var productSelectorAPI;
            var productSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCreditClassDirectiveReady = function (api) {
                    creditClassSelectorAPI = api;
                    creditClassReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onProductDirectiveReady = function (api) {
                    productSelectorAPI = api;
                    productSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;
                    var partSettings;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        partSettings = payload.partSettings;
                    }

                    //Loading Currency Selector
                    var currencySelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    currencySelectorReadyPromiseDeferred.promise.then(function () {
                        var currencySelectorPayload = partSettings != undefined ? { selectedIds: partSettings.CurrencyId } : undefined;
                        VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadPromiseDeferred);
                    });
                    promises.push(currencySelectorLoadPromiseDeferred.promise);

                    //Loading CreditClass Selector
                    var creditClassSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    creditClassReadyPromiseDeferred.promise.then(function () {
                        var creditClassSelectorPayload = partSettings != undefined ? { selectedIds: partSettings.CreditClassId } : undefined;
                        VRUIUtilsService.callDirectiveLoad(creditClassSelectorAPI, creditClassSelectorPayload, creditClassSelectorLoadPromiseDeferred);
                    });
                    promises.push(creditClassSelectorLoadPromiseDeferred.promise);

                    //Loading Product Selector
                    var productSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    productSelectorReadyPromiseDeferred.promise.then(function () {
                        var productSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountProductFilter, Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId
                                }]
                            }
                        };
                        if (partSettings != undefined) {
                            productSelectorPayload.selectedIds = partSettings.ProductId;
                        }
                        VRUIUtilsService.callDirectiveLoad(productSelectorAPI, productSelectorPayload, productSelectorLoadPromiseDeferred);
                    });
                    promises.push(productSelectorLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancial,Retail.BusinessEntity.MainExtensions',
                        CurrencyId: currencySelectorAPI.getSelectedIds(),
                        CreditClassId: creditClassSelectorAPI.getSelectedIds(),
                        ProductId: productSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

