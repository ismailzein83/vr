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
                                var roundedConvertedSupplierRate = roundNumber(currentRouteOption.ConvertedSupplierRate);
                                currentRouteOption.title = buildTooltip(currentRouteOption, roundedConvertedSupplierRate);
                                currentRouteOption.titleToDisplay = buildTitleToDisplay(display, currentRouteOption, roundedConvertedSupplierRate);

                                var evaluatedStatus = UtilsService.getItemByVal(routeOptionEvaluatedStatusEnum, currentRouteOption.EvaluatedStatus, "value");
                                if (evaluatedStatus != undefined) {
                                    currentRouteOption.EvaluatedStatusCssClass = evaluatedStatus.cssclass;
                                }

                                if (currentRouteOption.SupplierStatus == WhS_Routing_SupplierStatusEnum.Block.value) {
                                    currentRouteOption.IsBlocked = true;
                                }

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

            function buildTooltip(routeOption, roundedConvertedSupplierRate) {
                var optionOrder = routeOption.OptionOrder + 1;
                var result = buildTitle(routeOption, roundedConvertedSupplierRate);
                return optionOrder + '. ' + result;
            }

            function buildTitle(routeOption, roundedConvertedSupplierRate) {
                var supplierName = routeOption.SupplierName;
                var percentage = routeOption.Percentage;
                var acd = routeOption.ACD;
                var asr = routeOption.ASR;
                var supplierZoneName = routeOption.SupplierZoneName;
                var services = routeOption.SupplierServicesNames;

                var result = percentage ? percentage + '% ' + supplierName : supplierName;
                if (asr) {
                    result = result + ', ASR:' + asr;
                }
                if (acd) {
                    result = result + ', ACD:' + acd;
                }
                if (roundedConvertedSupplierRate) {
                    result = result + ' (' + roundedConvertedSupplierRate + ')';
                }
                if (supplierZoneName) {
                    result = result + ', Zone: ' + supplierZoneName;
                }
                if (services) {
                    result = result + ', Services: ' + services;
                }

                return result;
            }

            function buildTitleToDisplay(display, routeOption, roundedConvertedSupplierRate) {
                switch (display) {
                    case WhS_BE_ZoneRouteOptionsEnum.SupplierRate.value:
                        routeOption.isNumber = true;
                        return routeOption.ConvertedSupplierRate;

                    case WhS_BE_ZoneRouteOptionsEnum.SupplierRateWithNameAndPercentage.value:
                        routeOption.isNumber = false;

                        var totalWhiteSpaceLength = 30;
                        var roundedConvertedSupplierRateLength = roundedConvertedSupplierRate.toString().length + 2; //2 is length of " ()"
                        var remainingWhiteSpaceLength = totalWhiteSpaceLength - roundedConvertedSupplierRateLength;

                        var title = buildTitle(routeOption);
                        if (title.length > remainingWhiteSpaceLength) {
                            title = title.substring(0, remainingWhiteSpaceLength - 2); //2 is length of "..."
                            title += "...";
                        }

                        return title + ' (' + roundedConvertedSupplierRate + ')';

                    default:
                        routeOption.isNumber = true;
                        return routeOption.ConvertedSupplierRate;
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