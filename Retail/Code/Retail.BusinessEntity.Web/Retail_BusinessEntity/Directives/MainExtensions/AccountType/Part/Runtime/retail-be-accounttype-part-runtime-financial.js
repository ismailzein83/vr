'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancial', ["Retail_BE_PaymentMethodEnum", "UtilsService", "VRUIUtilsService", function (Retail_BE_PaymentMethodEnum, UtilsService, VRUIUtilsService) {
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


        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.paymentMethods = UtilsService.getArrayEnum(Retail_BE_PaymentMethodEnum);
        
            $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyPromiseDeferred.resolve();
            }

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined && payload.partSettings != undefined)
                {
                    $scope.scopeModel.selectedPaymentMethod = UtilsService.getItemByVal($scope.scopeModel.paymentMethods, payload.partSettings.PaymentMethod, "value");
                    $scope.scopeModel.bankDetails = payload.partSettings.BankDetails;
                    if (payload.partSettings.PostpaidSettings != undefined)
                        $scope.scopeModel.duePeriodInDays = payload.partSettings.PostpaidSettings.DuePeriodInDays;
                }


                var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                currencySelectorReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = (payload != undefined && payload.partSettings != undefined) ? { selectedIds: payload.partSettings.CurrencyId } : undefined

                        VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, directivePayload, loadCurrencySelectorPromiseDeferred);
                    });

                return loadCurrencySelectorPromiseDeferred.promise;


            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancial,Retail.BusinessEntity.MainExtensions',
                    PaymentMethod: $scope.scopeModel.selectedPaymentMethod.value,
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    BankDetails: $scope.scopeModel.bankDetails,
                    PostpaidSettings: { DuePeriodInDays: $scope.scopeModel.duePeriodInDays },
                    PrepardSettings:{},
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);