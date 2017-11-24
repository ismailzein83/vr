'use strict';
app.directive('vrWhsRoutingCustomerrouteDetails', ['WhS_Routing_RouteOptionRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService','WhS_Routing_RouteOptionRuleAPIService',
    function (WhS_Routing_RouteOptionRuleService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RouteOptionRuleAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new customerRouteDetailsCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteDetailTemplate.html"
        };

        function customerRouteDetailsCtor(ctrl, $scope) {
            this.initializeController = initializeController;
            var customerRoute;
            var gridAPI;
            function initializeController() {
                $scope.routeOptionDetails = [];

                $scope.getRowStyle = function (dataItem) {

                    var rowStyle;

                    if (dataItem.IsBlocked) {
                        rowStyle = { CssClass: "bg-danger" };
                    }
                    else if (dataItem.ExecutedRuleId) {
                        rowStyle = { CssClass: "bg-success" };
                    }

                    return rowStyle
                };
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                };
                $scope.getMenuActions = function (dataItem) {
                    var menuActions = [];

                    if (dataItem.ExecutedRuleId) {
                        menuActions.push({
                            name: "Matching Rule",
                            clicked: viewRouteOptionRuleEditor,
                        })
                    }

                    if (dataItem.LinkedRouteOptionRuleIds != null && dataItem.LinkedRouteOptionRuleIds.length > 0) {
                        if (dataItem.LinkedRouteOptionRuleIds.length == 1) {
                            menuActions.push({
                                name: "Edit Rule",
                                clicked: editLinkedRouteOptionRule,
                                haspermission: hasUpdateRulePermission
                            });
                        }
                        else {
                            menuActions.push({
                                name: "Linked Rules",
                                clicked: viewLinkedRouteOptionRules,
                            });
                        }
                    }
                    else {
                        menuActions.push({
                            name: "Add Rule",
                            clicked: addRouteOptionRuleEditor,
                            haspermission: hasAddRulePermission
                        });
                    }

                    return menuActions;
                };


                function hasUpdateRulePermission() {
                    return WhS_Routing_RouteOptionRuleAPIService.HasUpdateRulePermission();
                };

                function hasAddRulePermission() {
                    return WhS_Routing_RouteOptionRuleAPIService.HasAddRulePermission();
                };

                function viewLinkedRouteOptionRules(dataItem) {
                    WhS_Routing_RouteOptionRuleService.viewLinkedRouteOptionRules(dataItem.LinkedRouteOptionRuleIds, customerRoute.Entity.Code);
                };

                function editLinkedRouteOptionRule(dataItem) {
                    WhS_Routing_RouteOptionRuleService.editLinkedRouteOptionRule(dataItem.LinkedRouteOptionRuleIds[0], customerRoute.Entity.Code);
                };

                function viewRouteOptionRuleEditor(dataItem) {
                    WhS_Routing_RouteOptionRuleService.viewRouteOptionRule(dataItem.ExecutedRuleId);
                }

                function addRouteOptionRuleEditor(dataItem) {
                    var onRouteOptionRuleAdded = function (addedItem) {
                        var newDataItem =
                            {
                                CustomerRouteOptionDetailId: dataItem.CustomerRouteOptionDetailId,
                                RouteOption: dataItem.RouteOption,
                                SupplierName: dataItem.SupplierName,
                                SupplierCode: dataItem.SupplierCode,
                                SupplierZoneName: dataItem.SupplierZoneName,
                                SupplierRate: dataItem.SupplierRate,
                                Percentage: dataItem.Percentage,
                                IsBlocked: dataItem.IsBlocked,
                                ExactSupplierServiceIds: dataItem.ExactSupplierServiceIds,
                                ExecutedRuleId: dataItem.ExecutedRuleId,
                                LinkedRouteOptionRuleIds: [],
                                LinkedRouteOptionRuleCount: 1
                            };
                        newDataItem.LinkedRouteOptionRuleIds.push(addedItem.Entity.RuleId);
                        extendRouteOptionDetailObject(newDataItem);
                        gridAPI.itemUpdated(newDataItem);
                    };

                    var linkedRouteOptionRuleInput = { RuleId: dataItem.Entity.ExecutedRuleId, CustomerId: customerRoute.Entity.CustomerId, Code: customerRoute.Entity.Code, SupplierId: dataItem.Entity.SupplierId, SupplierZoneId: dataItem.Entity.SupplierZoneId };
                    WhS_Routing_RouteOptionRuleService.addLinkedRouteOptionRule(linkedRouteOptionRuleInput, customerRoute.Entity.Code, onRouteOptionRuleAdded);
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var _routeOptionDetailServiceViewerPromises = [];

                    if (payload != undefined)
                        customerRoute = payload.customerRoute;

                    if (customerRoute != undefined && customerRoute.RouteOptionDetails != null) {
                        for (var i = 0; i < customerRoute.RouteOptionDetails.length; i++) {
                            var routeOptionDetail = customerRoute.RouteOptionDetails[i];
                            extendRouteOptionDetailObject(routeOptionDetail);
                            _routeOptionDetailServiceViewerPromises.push(routeOptionDetail.routeOptionDetailLoadDeferred.promise);
                            $scope.routeOptionDetails.push(routeOptionDetail);
                        }
                    }

                    return UtilsService.waitMultiplePromises(_routeOptionDetailServiceViewerPromises).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendRouteOptionDetailObject(routeOptionDetail) {
                routeOptionDetail.routeOptionDetailLoadDeferred = UtilsService.createPromiseDeferred();
                routeOptionDetail.onServiceViewerReady = function (api) {
                    routeOptionDetail.serviceViewerAPI = api;

                    var serviceViewerPayload = {
                        selectedIds: routeOptionDetail.ExactSupplierServiceIds
                    };

                    VRUIUtilsService.callDirectiveLoad(routeOptionDetail.serviceViewerAPI, serviceViewerPayload, routeOptionDetail.routeOptionDetailLoadDeferred);
                };
            }
        }

        return directiveDefinitionObject;
    }]);