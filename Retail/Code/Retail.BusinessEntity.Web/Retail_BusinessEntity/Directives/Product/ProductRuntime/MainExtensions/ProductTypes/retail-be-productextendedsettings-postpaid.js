(function (app) {

    'use strict';

    PostpaidSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function PostpaidSettingsDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PostpaidSettingsCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/MainExtensions/ProductTypes/Templates/PostPaidSettingsTemplate.html'
        };

        function PostpaidSettingsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var currencySelectorAPI;
            var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCurrencySelectorReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var currencyId;

                    if (payload != undefined && payload.extendedSettings) {
                        $scope.scopeModel.creditLimit = payload.extendedSettings.CreditLimit;
                        currencyId = payload.extendedSettings.CurrencyId;
                    }

                    //Loading Currency Selector
                    var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    currencySelectorReadyDeferred.promise.then(function () {

                        var currencySelectorPayload = {};
                        if (currencyId) {
                            currencySelectorPayload.selectedIds = currencyId;
                        }
                        else {
                            currencySelectorPayload.selectSystemCurrency = true;
                        }

                        VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadDeferred);
                    });

                    return currencySelectorLoadDeferred.promise;
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.ProductTypes.PostPaid.PostPaidSettings, Retail.BusinessEntity.MainExtensions",
                        CreditLimit: $scope.scopeModel.creditLimit,
                        CurrencyId: currencySelectorAPI.getSelectedIds()
                    };

                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeProductextendedsettingsPostpaid', PostpaidSettingsDirective);

})(app);