'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancial', [  "UtilsService", "VRUIUtilsService",
    function (  UtilsService, VRUIUtilsService) {
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

        var creditClassSelectorAPI;
        var creditClassReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var paymentMethodDirectiveAPI;
        var paymentMethodReadyPromiseDeferred;


        function initializeController() {
            $scope.scopeModel = {};
        
            $scope.scopeModel.onCreditClassDirectiveReady = function (api) {
                creditClassSelectorAPI = api;
                creditClassReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyPromiseDeferred.resolve();
            };
           
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined && payload.partSettings != undefined)
                {
                }

                var creditClassSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                creditClassReadyPromiseDeferred.promise.then(function () {
                    var directivePayload = (payload != undefined && payload.partSettings != undefined) ? { selectedIds: payload.partSettings.CreditClassId } : undefined;
                    VRUIUtilsService.callDirectiveLoad(creditClassSelectorAPI, directivePayload, creditClassSelectorLoadPromiseDeferred);
                });
                promises.push(creditClassSelectorLoadPromiseDeferred.promise);

                var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                currencySelectorReadyPromiseDeferred.promise.then(function () {
                    var directivePayload = (payload != undefined && payload.partSettings != undefined) ? { selectedIds: payload.partSettings.CurrencyId } : undefined;

                        VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, directivePayload, loadCurrencySelectorPromiseDeferred);
                    });
                promises.push(loadCurrencySelectorPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancial,Retail.BusinessEntity.MainExtensions',
                    CurrencyId: currencySelectorAPI.getSelectedIds(),             
                    CreditClassId: creditClassSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);

