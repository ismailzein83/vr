(function (app) {

    'use strict';

    MarketPriceManagementDirective.$inject = ['WhS_Routing_MarketPriceService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function MarketPriceManagementDirective(WhS_Routing_MarketPriceService, UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var marketPrice = new MarketPriceCtor($scope, ctrl);
                marketPrice.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/RouteOptionRuleSettings/Templates/RouteOptionRulesSettingsMarketPriceTemplate.html'
        };

        function MarketPriceCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();

            var currencySelectorAPI;
            var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var settingsEditorRuntime;

            function initializeController() {
                var _promises = [gridReadyDeferred.promise, currencySelectorReadyDeferred.promise];

                ctrl.marketPrices = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };

                ctrl.onCurrencySelectorReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyDeferred.resolve();
                }

                ctrl.onAddMarketPrice = function () {
                    var onMarketPriceAdded = function (addedMarketPrice) {
                        ctrl.marketPrices.push(addedMarketPrice);
                    };

                    WhS_Routing_MarketPriceService.addMarketPrice(ctrl.marketPrices, onMarketPriceAdded);
                };
                ctrl.onDeleteMarketPrice = function (marketPrice) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.marketPrices, marketPrice.ServiceNames, 'ServiceNames');
                            ctrl.marketPrices.splice(index, 1);
                        }
                    });
                }

                UtilsService.waitMultiplePromises(_promises).then(function () {
                    defineAPI();
                });

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    ctrl.marketPrices = [];

                    var marketPrices;
                    var currency;

                    if (payload != undefined) {
                        settingsEditorRuntime = payload.SettingsEditorRuntime;

                        if (payload.RouteOptionRuleSettings != undefined) {
                            marketPrices = payload.RouteOptionRuleSettings.MarketPrices;
                            currency = payload.RouteOptionRuleSettings.CurrencyId;
                        }
                    }

                    //Loading Grid
                    if (marketPrices) {
                        for (var key in marketPrices) {
                            if (key != '$type') {
                                var marketPrice = marketPrices[key];
                                extendMarketPriceObject(marketPrice);
                                ctrl.marketPrices.push(marketPrice);
                            }
                        }
                    }

                    //Loading Currency Selector
                    var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    var currencySelectorPayload = {
                        selectedIds: currency
                    };
                    VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadDeferred);

                    return currencySelectorLoadDeferred.promise;
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.Routing.Business.MarketPriceRouteOptionRule, TOne.WhS.Routing.Business",
                        MarketPrices: buildMarketPricesDictionary(ctrl.marketPrices),
                        CurrencyId: currencySelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function extendMarketPriceObject(marketPrice) {
                if (settingsEditorRuntime == undefined)
                    return;

                var _serviceNames = [];

                //Editing Services
                if (marketPrice.ServiceIds != undefined && settingsEditorRuntime.Services != undefined) {
                    for (var index = 0; index < marketPrice.ServiceIds.length; index++) {
                        _serviceNames.push(settingsEditorRuntime.Services[marketPrice.ServiceIds[index]]);
                    }
                }
                marketPrice.ServiceNames = _serviceNames.join(", ");
            }
            function buildMarketPricesDictionary(marketPrices) {
                var marketPricesDictionary = {};
                var _serviceIds = [];
                var marketPrice;

                for (var index = 0 ; index < marketPrices.length; index++) {
                    marketPrice = marketPrices[index];
                    _serviceIds = marketPrice.ServiceIds.sort().join(",");
                    marketPricesDictionary[_serviceIds] = marketPrice;
                }

                return marketPricesDictionary;
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editMarketPrice
                }];
            }
            function editMarketPrice(marketPrice) {
                var onMarketPriceUpdated = function (updatedMarketPrice) {
                    var index = UtilsService.getItemIndexByVal(ctrl.marketPrices, marketPrice.ServiceNames, 'ServiceNames');
                    ctrl.marketPrices[index] = updatedMarketPrice;
                };

                WhS_Routing_MarketPriceService.editMarketPrice(marketPrice, ctrl.marketPrices, onMarketPriceUpdated);
            }
        }
    }

    app.directive('vrWhsRoutingRouteoptionrulesettingsMarketprice', MarketPriceManagementDirective);

})(app);