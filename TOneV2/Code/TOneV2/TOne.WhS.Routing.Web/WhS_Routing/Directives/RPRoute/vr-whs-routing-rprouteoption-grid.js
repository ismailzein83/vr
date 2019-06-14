﻿'use strict';

app.directive('vrWhsRoutingRprouteoptionGrid', ['WhS_Routing_RPRouteAPIService', 'WhS_Routing_RouteOptionRuleService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_Routing_RouteOptionEvaluatedStatusEnum',
    function (WhS_Routing_RPRouteAPIService, WhS_Routing_RouteOptionRuleService, UtilsService, VRNotificationService, VRUIUtilsService, WhS_Routing_RouteOptionEvaluatedStatusEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var rpRouteOptionGrid = new RpRouteOptionGrid($scope, ctrl, $attrs);
                rpRouteOptionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteOptionGridTemplate.html'
        };

        function RpRouteOptionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var routingProductId;
            var saleZoneId;
            var supplierId;
            var routingDatabaseId;
            var currencyId;
            var saleRate;
            var routeOptionEvaluatedStatusEnum = UtilsService.getArrayEnum(WhS_Routing_RouteOptionEvaluatedStatusEnum);

            var gridAPI;

            function initializeController() {
                $scope.supplierZones = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.getMenuActions = function (dataItem) {
                    var menuActions = [];

                    if (dataItem.Entity.ExecutedRuleId) {
                        menuActions.push({
                            name: "Option Rule",
                            clicked: openRouteOptionRuleEditor
                        });
                    }

                    function openRouteOptionRuleEditor(dataItem) {
                        WhS_Routing_RouteOptionRuleService.editRouteOptionRule(dataItem.Entity.ExecutedRuleId);
                    }

                    return menuActions;
                };

                //$scope.getRowStyle = function (dataItem) {
                //    var rowStyle;
                //    if (dataItem.Entity.IsBlocked) {
                //        rowStyle = { CssClass: "bg-danger" };
                //    }
                //    else if (dataItem.Entity.ExecutedRuleId) {
                //        rowStyle = { CssClass: "bg-success" };
                //    }
                //    return rowStyle;
                //};
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rpRouteOptionSupplierInput;
                    if (payload != undefined) {
                        rpRouteOptionSupplierInput = {
                            RoutingDatabaseId: payload.routingDatabaseId,
                            RoutingProductId: payload.routingProductId,
                            SaleZoneId: payload.saleZoneId,
                            SupplierId: payload.supplierId,
                            CurrencyId: payload.currencyId,
                            SaleRate: payload.saleRate
                        };
                    }

                    return WhS_Routing_RPRouteAPIService.GetRPRouteOptionSupplier(rpRouteOptionSupplierInput).then(function (response) {
                        if (response) {
                            var _supplierZoneServiceViewerPromises = [];

                            for (var i = 0; i < response.SupplierZones.length; i++) {
                                var supplierZone = response.SupplierZones[i];
                                $scope.supplierZones.push(supplierZone);
                                extendSupplierZoneObject(supplierZone);
                                _supplierZoneServiceViewerPromises.push(supplierZone.supplierZoneLoadDeferred.promise);
                            }

                            UtilsService.waitMultiplePromises(_supplierZoneServiceViewerPromises).then(function () {
                            }).catch(function (error) {
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                            });
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendSupplierZoneObject(supplierZone) {
                supplierZone.supplierZoneLoadDeferred = UtilsService.createPromiseDeferred();
                supplierZone.onServiceViewerReady = function (api) {
                    supplierZone.serviceViewerAPI = api;

                    var serviceViewerPayload;
                    if (supplierZone.Entity != undefined) {
                        serviceViewerPayload = { selectedIds: supplierZone.Entity.ExactSupplierServiceIds };
                    }
                    VRUIUtilsService.callDirectiveLoad(supplierZone.serviceViewerAPI, serviceViewerPayload, supplierZone.supplierZoneLoadDeferred);
                };

                var evaluatedStatus = UtilsService.getItemByVal(routeOptionEvaluatedStatusEnum, supplierZone.EvaluatedStatus, "value");
                if (evaluatedStatus != undefined) {
                    supplierZone.EvaluatedStatusCssClass = evaluatedStatus.cssclass;
                }
            }
        }
    }]);