'use strict';
app.directive('vrWhsSalesSellingrulesettingsThresholdFixed', ['$compile', 'VRUIUtilsService','UtilsService',
    function ($compile, VRUIUtilsService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new sellingRuleFixedConstructor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/SellingRuleSettings/Fixed/Templates/FixedThresholdSellingRule.html"

        };


        function sellingRuleFixedConstructor(ctrl, $scope, $attrs) {

            var currencyDirectiveAPI;
            var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                defineAPI();

                ctrl.onCurrencySelectReady = function (api) {
                    currencyDirectiveAPI = api;
                    currencyDirectiveReadyPromiseDeferred.resolve();
                };

            }
            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.Sales.MainExtensions.FixedRateThreshold,TOne.WhS.Sales.MainExtensions",
                        CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                        Rate: ctrl.rate
                    };
                    return obj;
                };

                api.load = function (payload) {
                    if (payload != undefined && payload.threshold != undefined) {

                        var promises = [];
                        ctrl.rate = payload.threshold.Rate;

                        var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        var currencyPayload;

                        if (payload.threshold.CurrencyId > 0) {
                            currencyPayload = { selectedIds: payload.threshold.CurrencyId };
                        }
                        else {
                            currencyPayload = { selectSystemCurrency: true };
                        }

                        currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred);

                        });
                        promises.push(loadCurrencySelectorPromiseDeferred.promise);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);