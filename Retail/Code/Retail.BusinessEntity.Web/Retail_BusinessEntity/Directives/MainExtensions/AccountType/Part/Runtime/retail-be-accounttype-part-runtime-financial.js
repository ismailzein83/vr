'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancial', ["Retail_BE_PaymentMethodEnum", "UtilsService", function (Retail_BE_PaymentMethodEnum, UtilsService) {
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
                if(payload !=undefined)
                {
                    $scope.scopeModel.selectedPaymentMethod = UtilsService.getItemByVal($scope.scopeModel.paymentMethods, payload.PaymentMethod,"value");
                    $scope.scopeModal.bankDetails = payload.BankDetails;
                    if (payload.PostpaidSettings != undefined)
                      $scope.scopeModal.duePeriodInDays = payload.PostpaidSettings.DuePeriodInDays;
                }


                var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                currencySelectorReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = (payload != undefined) ? { selectedIds: payload.CurrencyId } : undefined

                        VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, directivePayload, loadCurrencySelectorPromiseDeferred);
                    });

                return loadCurrencySelectorPromiseDeferred.promise;


            };

            api.getData = function () {
                return {
                    $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartFinancialRuntime,Retail.BusinessEntity.MainExtensions',
                    PaymentMethod: $scope.scopeModel.selectedPaymentMethod.value,
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    BankDetails:$scope.scopeModal.bankDetails,
                    PostpaidSettings:{DuePeriodInDays:$scope.scopeModal.duePeriodInDays},
                    PrepardSettings:{},
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);