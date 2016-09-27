(function (app) {

    'use strict';

    MarketPriceManagementDirective.$inject = ['WhS_Routing_MarketPriceService', 'UtilsService', 'VRNotificationService'];

    function MarketPriceManagementDirective(WhS_Routing_MarketPriceService, UtilsService, VRNotificationService) {
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

            var settingsEditorRuntime;

            function initializeController() {
                ctrl.marketPrices = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

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

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    ctrl.marketPrices = [];
                    var marketPrices;

                    if (payload != undefined) {
                        settingsEditorRuntime = payload.SettingsEditorRuntime;

                        if (payload.RouteOptionRuleSettings != undefined)
                            marketPrices = payload.RouteOptionRuleSettings.MarketPrices;
                    }

                    if (marketPrices) {
                        for (var index = 0; index < marketPrices.length; index++) {
                            var marketPrice = marketPrices[index];
                            extendObject(marketPrice);
                            ctrl.marketPrices.push(marketPrice);
                        }
                    }
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.Routing.Business.MarketPriceRouteOptionRule, TOne.WhS.Routing.Business",
                        MarketPrices: ctrl.marketPrices
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function extendObject(marketPrice) {
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

                //Editing Currency
                if (settingsEditorRuntime.Currency != undefined) {
                    marketPrice.CurrencyName = settingsEditorRuntime.Currency[marketPrice.CurrencyId];
                }
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