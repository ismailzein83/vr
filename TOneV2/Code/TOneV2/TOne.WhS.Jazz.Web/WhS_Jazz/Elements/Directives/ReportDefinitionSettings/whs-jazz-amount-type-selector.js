(function (app) {

    'use strict';

    whsJazzAmountTypeSelector.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function whsJazzAmountTypeSelector(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onselectionchanged: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/ReportDefinitionSettings/Templates/AmountCalculationSelector.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var rateCalculationTypeSelectorAPI;
            var rateCalculationTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencySelectorAPI;
            var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onRateCalculationTypeSelectorReady = function (api) {
                    rateCalculationTypeSelectorAPI = api;
                    rateCalculationTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCurrencySelectorReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function loadRateCalculationTypeSelector(payload) {
                var rateCalculationTypeSelectorLoadromiseDeferred = UtilsService.createPromiseDeferred();
                rateCalculationTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(rateCalculationTypeSelectorAPI, payload, rateCalculationTypeSelectorLoadromiseDeferred);
                });
                return rateCalculationTypeSelectorLoadromiseDeferred.promise;
            }
            function loadCurrencySelector(payload) {
                var currencySelectorLoadromiseDeferred = UtilsService.createPromiseDeferred();
                currencySelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, payload, currencySelectorLoadromiseDeferred);
                });
                return currencySelectorLoadromiseDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload)
                    var promises = [];
                    var currencyId = payload != undefined ? payload.CurrencyId : undefined;
                    promises.push(loadRateCalculationTypeSelector(payload));
                    promises.push(loadCurrencySelector({ selectedIds: currencyId }));

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var rateCalculationTypeData = rateCalculationTypeSelectorAPI.getData();
                    return {
                        $type: "TOne.WhS.Jazz.Entities.AmountCalculation,TOne.WhS.Jazz.Entities",
                        RateCalculationType: rateCalculationTypeData.RateCalculationType,
                        RateValue: rateCalculationTypeData.RateValue,
                        Currency: currencySelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzAmountTypeSelector', whsJazzAmountTypeSelector);

})(app);