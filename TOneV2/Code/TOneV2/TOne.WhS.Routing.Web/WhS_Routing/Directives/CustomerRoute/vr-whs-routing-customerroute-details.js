'use strict';
app.directive('vrWhsRoutingCustomerrouteDetails', ['WhS_Routing_RouteOptionRuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (WhS_Routing_RouteOptionRuleService, UtilsService, VRUIUtilsService, VRNotificationService) {

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

                $scope.getMenuActions = function (dataItem) {
                    var menuActions = [];

                    if (dataItem.ExecutedRuleId) {
                        menuActions.push({
                            name: "Option Rule",
                            clicked: openRouteOptionRuleEditor,
                        })
                    }

                    function openRouteOptionRuleEditor(dataItem) {
                        WhS_Routing_RouteOptionRuleService.editRouteOptionRule(dataItem.ExecutedRuleId);
                    }

                    return menuActions;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var customerRoute;
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
                    if (routeOptionDetail.Entity != undefined) {
                        serviceViewerPayload = {
                            selectedIds: customerRoute.Entity.CustomerServiceIds
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(routeOptionDetail.serviceViewerAPI, serviceViewerPayload, routeOptionDetail.routeOptionDetailLoadDeferred);
                };
            }
        }

        return directiveDefinitionObject;
    }]);