'use strict';

app.directive('whsRoutesyncEricssonManualroutes', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RoutesyncManualRoutesEricssonCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonManualRoutesTemplate.html"
        };

        function RoutesyncManualRoutesEricssonCtor(ctrl, $scope, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.isValid = function () {

                    return null;
                };

                $scope.scopeModel.removerow = function (dataItem) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Id, 'Id');
                            $scope.scopeModel.datasource.splice(index, 1);
                        }
                    });
                };

                $scope.scopeModel.addManualRoute = function () {
                    var newManualRoute = {};
                    extendManualRoute(newManualRoute);

                    $scope.scopeModel.datasource.push(newManualRoute);
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.manualRoutes != undefined) {
                        for (var i = 0; i < payload.manualRoutes.length; i++) {
                            var currentManualRoute = payload.manualRoutes[i];
                            promises.push(extendManualRoute(currentManualRoute));
                            $scope.scopeModel.datasource.push(currentManualRoute);
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var ericssonManualRoutes = [];
                    if ($scope.scopeModel.datasource != undefined) {
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentDataSource = $scope.scopeModel.datasource[i];
                            var manualRoute = { Customers: currentDataSource.Customers };

                            if (currentDataSource.actionAPI != undefined)
                                manualRoute.ManualRouteAction = currentDataSource.actionAPI.getData();

                            if (currentDataSource.originationsAPI != undefined)
                                manualRoute.ManualRouteOriginations = currentDataSource.originationsAPI.getData();

                            ericssonManualRoutes.push(manualRoute);
                        }
                    }
                    return ericssonManualRoutes;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendManualRoute(manualRoute) {
                var routePromises = [];
                manualRoute.Id = UtilsService.guid();
                var actionPromiseDeferred = UtilsService.createPromiseDeferred();
                var originationsPromiseDeferred = UtilsService.createPromiseDeferred();
                routePromises.push(actionPromiseDeferred.promise);
                routePromises.push(originationsPromiseDeferred.promise);
                manualRoute.onManualRouteActionReady = function (api) {
                    manualRoute.actionAPI = api;
                    //manualRoute.actionPromiseDeferred = UtilsService.createPromiseDeferred();
                    var payload = { ManualRouteAction: manualRoute.ManualRouteAction };
                    VRUIUtilsService.callDirectiveLoad(manualRoute.actionAPI, payload, actionPromiseDeferred);
                };

                //manualRoute.onManualRouteDestinationsReady = function (api) {
                //    manualRoute.destinationsAPI = api;
                //    manualRoute.destinationsPromiseDeferred = UtilsService.createPromiseDeferred();
                //    var payload = { RouteDestinations: manualRoute.ManualRouteDestinations };
                //    VRUIUtilsService.callDirectiveLoad(manualRoute.destinationsAPI, payload, manualRoute.destinationsPromiseDeferred);
                //};

                manualRoute.onManualRouteOriginationsReady = function (api) {
                    manualRoute.originationsAPI = api;
                    //manualRoute.originationsPromiseDeferred = UtilsService.createPromiseDeferred();
                    var payload = { RouteOriginations: manualRoute.ManualRouteOriginations };
                    VRUIUtilsService.callDirectiveLoad(manualRoute.originationsAPI, payload, originationsPromiseDeferred);
                };
                return UtilsService.waitMultiplePromises(routePromises);
            }
        }
        return directiveDefinitionObject;
    }]);