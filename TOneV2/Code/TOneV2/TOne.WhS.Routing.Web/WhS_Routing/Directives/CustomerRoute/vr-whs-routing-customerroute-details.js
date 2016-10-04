'use strict';
app.directive('vrWhsRoutingCustomerrouteDetails', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRNotificationService) {

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
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    console.log(payload.customerRoute);

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
                }

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