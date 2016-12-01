'use strict';

app.directive('retailBeAccounttypePartRuntimeFinancial', ["Retail_BE_PaymentMethodEnum", "Retail_BE_AccountTypePartRuntimeFinancialEnum", "UtilsService", "VRUIUtilsService",
    function (Retail_BE_PaymentMethodEnum, Retail_BE_AccountTypePartRuntimeFinancialEnum, UtilsService, VRUIUtilsService) {
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
            $scope.scopeModel.paymentMethods = UtilsService.getArrayEnum(Retail_BE_PaymentMethodEnum);
            $scope.scopeModel.billingCycles = UtilsService.getArrayEnum(Retail_BE_AccountTypePartRuntimeFinancialEnum);
        
            $scope.scopeModel.onCreditClassDirectiveReady = function (api) {
                creditClassSelectorAPI = api;
                creditClassReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onDirectiveReady = function (api) {
                paymentMethodDirectiveAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingPaymentMethod = value };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, paymentMethodDirectiveAPI, payload, setLoader, paymentMethodReadyPromiseDeferred);
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                if (payload != undefined && payload.partSettings != undefined)
                {
                    if (payload.partSettings.PaymentMethod != undefined)
                    {
                        var loadPaymentMethodPromiseDeferred = UtilsService.createPromiseDeferred();
                        paymentMethodReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        paymentMethodReadyPromiseDeferred.promise
                            .then(function () {
                                paymentMethodReadyPromiseDeferred = undefined;
                                var directivePayload;

                                switch(payload.partSettings.PaymentMethod)
                                {
                                    case Retail_BE_PaymentMethodEnum.Postpaid.value:directivePayload = payload.partSettings.PostpaidSettings;break;
                                    case Retail_BE_PaymentMethodEnum.Prepaid.value:directivePayload = payload.partSettings.PrepaidSettings;break;
                                }
                                VRUIUtilsService.callDirectiveLoad(paymentMethodDirectiveAPI, directivePayload, loadPaymentMethodPromiseDeferred);
                            });

                        promises.push(loadPaymentMethodPromiseDeferred.promise);

                        $scope.scopeModel.selectedPaymentMethod = UtilsService.getItemByVal($scope.scopeModel.paymentMethods, payload.partSettings.PaymentMethod, "value");
                        $scope.scopeModel.selectedBillingCycle = UtilsService.getItemByVal($scope.scopeModel.billingCycles, payload.partSettings.BillingCycleId, "value");
                    }
                    $scope.scopeModel.bankDetails = payload.partSettings.BankDetails;
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
                    PaymentMethod: $scope.scopeModel.selectedPaymentMethod.value,
                    CurrencyId: currencySelectorAPI.getSelectedIds(),
                    BankDetails: $scope.scopeModel.bankDetails,
                    PostpaidSettings: $scope.scopeModel.selectedPaymentMethod.value == Retail_BE_PaymentMethodEnum.Postpaid.value ? paymentMethodDirectiveAPI.getData() : undefined,
                    PrepaidSettings: $scope.scopeModel.selectedPaymentMethod.value == Retail_BE_PaymentMethodEnum.Prepaid.value ? paymentMethodDirectiveAPI.getData() : undefined,
                    BillingCycleId: $scope.scopeModel.selectedBillingCycle.value,
                    CreditClassId: creditClassSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);

