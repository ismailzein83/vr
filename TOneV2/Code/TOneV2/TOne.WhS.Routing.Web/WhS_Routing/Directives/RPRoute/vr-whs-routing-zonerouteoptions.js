"use strict";

app.directive("vrWhsRoutingZonerouteoptions", ["UtilsService", "UISettingsService", "WhS_Routing_RPRouteService", "WhS_Routing_SupplierStatusEnum", "WhS_BE_ZoneRouteOptionsEnum", "WhS_Routing_RouteOptionEvaluatedStatusEnum",
    function (UtilsService, UISettingsService, WhS_Routing_RPRouteService, WhS_Routing_SupplierStatusEnum, WhS_BE_ZoneRouteOptionsEnum, WhS_Routing_RouteOptionEvaluatedStatusEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var zoneRouteOptions = new ZoneRouteOptions(ctrl, $scope);
                zoneRouteOptions.initCtrl();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/ZoneRouteOptionsTemplate.html"
        };

        function ZoneRouteOptions(ctrl, $scope) {
            this.initCtrl = initCtrl;

            var routingDatabaseId;
            var routingProductId;
            var saleZoneId;
            var currencyId;
            var display;
            var saleRate;
            var routeOptionEvaluatedStatusEnum = UtilsService.getArrayEnum(WhS_Routing_RouteOptionEvaluatedStatusEnum);
            

            function initCtrl() {
                ctrl.routeOptions = [];

                ctrl.longPrecision = UISettingsService.getUIParameterValue('LongPrecision');

                ctrl.viewSupplier = function (routeOption) {
                    WhS_Routing_RPRouteService.viewRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, routeOption.SupplierId, currencyId, saleRate);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.routeOptions = [];

                    if (payload != undefined) {
                        routingDatabaseId = payload.RoutingDatabaseId;
                        routingProductId = payload.RoutingProductId;
                        saleZoneId = payload.SaleZoneId;
                        display = payload.display;
                        saleRate = payload.saleRate;

                        currencyId = payload.CurrencyId;
                        if (currencyId == undefined)
                            currencyId = payload.currencyId;

                        if (payload.RouteOptions) {
                            for (var i = 0; i < payload.RouteOptions.length; i++) {
                                var currentRouteOption = payload.RouteOptions[i];
                                currentRouteOption.title = buildTitle(currentRouteOption.SupplierName, currentRouteOption.Percentage, currentRouteOption.ACD, currentRouteOption.ASR);
                                currentRouteOption.titleToDisplay = buildTitleToDisplay(currentRouteOption, display);

                                var evaluatedStatus = UtilsService.getItemByVal(routeOptionEvaluatedStatusEnum, currentRouteOption.EvaluatedStatus, "value");
                                if (evaluatedStatus != undefined) {
                                    currentRouteOption.EvaluatedStatusCssClass = evaluatedStatus.cssclass;
                                }

                                //if (currentItem.SupplierStatus == WhS_Routing_SupplierStatusEnum.Block.value) {
                                //    currentItem.IsBlocked = true;
                                //    currentItem.Color = 'Red';
                                //}

                                if (currentRouteOption.Color == undefined)
                                    currentRouteOption.Color = '#616F77';

                                ctrl.routeOptions.push(currentRouteOption);
                            }
                        }
                    }
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function buildTitle(supplierName, percentage, acd, asr) {
                var result = supplierName;
                if (percentage) {
                    result = percentage + '%' + ' ' + result;
                }
                if (asr) {
                    result = result + ', ASR:' + asr;
                }
                if (acd) {
                    result = result + ', ACD:' + acd;
                }
                return result;
            }

            function buildTitleToDisplay(currentItem, display) {
                switch (display) {
                    case WhS_BE_ZoneRouteOptionsEnum.SupplierRate.value:
                        currentItem.isNumber = true;
                        return currentItem.ConvertedSupplierRate;

                    case WhS_BE_ZoneRouteOptionsEnum.SupplierRateWithNameAndPercentage.value:
                        currentItem.isNumber = false;
                        return currentItem.title + ' (' + roundNumber(currentItem.ConvertedSupplierRate) + ')';

                    default:
                        currentItem.isNumber = true;
                        return currentItem.ConvertedSupplierRate;
                }
            }

            function roundNumber(number) {
                var precisionNumber = Math.pow(10, ctrl.longPrecision);
                return Math.round(number * precisionNumber) / precisionNumber;
            }

            //function getRowStyle(optionStatus) {
            //    if (optionStatus == WhS_Routing_SupplierStatusEnum.PartialActive.value)
            //        return '#d0c89e'; //'BurlyWood';
            //    if (optionStatus == WhS_Routing_SupplierStatusEnum.Block.value)
            //        return '#efa2a2'; //'Red';
            //}
        }
    }]);