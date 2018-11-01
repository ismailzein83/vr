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
            var counter = 0;
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
                            console.log(dataItem.Id);
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Id, 'Id');
                            $scope.scopeModel.datasource.splice(index, 1); console.log(index);
                        }
                    });
                };

                $scope.scopeModel.addManualRoute = function () {
                    var newManualRoute = {};
                    newManualRoute.onManualRouteActionReady = function (api) {
                        newManualRoute.actionAPI = api;
                        //newManualRoute.actionPromiseDeferred = UtilsService.createPromiseDeferred();
                        var setLoader = function (value) { };
                        var actionPayload = newManualRoute.ManualRouteAction;
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, newManualRoute.actionAPI, actionPayload, setLoader);
                    };

                    //newManualRoute.onManualRouteDestinationsReady = function (api) {
                    //    newManualRoute.destinationsAPI = api;
                    //    //newManualRoute.destinationsPromiseDeferred = UtilsService.createPromiseDeferred();
                    //    var setLoader = function (value) { };
                    //    var destinationsPayload = newManualRoute.ManualRouteDestinations;
                    //    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, newManualRoute.destinationsAPI, destinationsPayload, setLoader);
                    //};

                    newManualRoute.onManualRouteOriginationsReady = function (api) {
                        newManualRoute.originationsAPI = api;
                        //newManualRoute.originationsPromiseDeferred = UtilsService.createPromiseDeferred();
                        var setLoader = function (value) { };
                        var originationsPayload = newManualRoute.ManualRouteOriginations;
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, newManualRoute.originationsAPI, originationsPayload, setLoader);
                    };
                    $scope.scopeModel.datasource.push(newManualRoute);
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    /*if (gridAPI != undefined) {
                        gridAPI.onItemAdded = function (dataItem) {
                            extendManualRoute(dataItem);
                        };
                    }*/
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        if (payload.EricssonManualRoutes != undefined) {
                            for (var i = 0; i < payload.EricssonManualRoutes.length; i++) {
                                extendManualRoute(payload.EricssonManualRoutes[i]);
                                $scope.scopeModel.datasource.push(payload.EricssonManualRoutes[i]);
                            }
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = { EricssonManualRoutes: [] };
                    if ($scope.scopeModel.datasource != undefined) {
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var manualRoute = { Customers: $scope.scopeModel.datasource[i].Customers };
                            if ($scope.scopeModel.datasource[i].actionAPI != undefined)
                                manualRoute.ManualRouteAction = $scope.scopeModel.datasource[i].actionAPI.getData();
                            //if ($scope.scopeModel.datasource[i].destinationsAPI != undefined)
                            //manualRoute.ManualRouteDestinations = $scope.scopeModel.datasource[i].destinationsAPI.getData();
                            if ($scope.scopeModel.datasource[i].originationsAPI != undefined)
                                manualRoute.ManualRouteOriginations = $scope.scopeModel.datasource[i].originationsAPI.getData();
                            data.EricssonManualRoutes.push(manualRoute);
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendManualRoute(manualRoute) {
                manualRoute.Id = counter;
                counter++;
                manualRoute.onManualRouteActionReady = function (api) {
                    manualRoute.actionAPI = api;
                    manualRoute.actionPromiseDeferred = UtilsService.createPromiseDeferred();
                    var payload = { ManualRouteAction: manualRoute.ManualRouteAction };
                    VRUIUtilsService.callDirectiveLoad(manualRoute.actionAPI, payload, manualRoute.actionPromiseDeferred);
                };

                //manualRoute.onManualRouteDestinationsReady = function (api) {
                //    manualRoute.destinationsAPI = api;
                //    manualRoute.destinationsPromiseDeferred = UtilsService.createPromiseDeferred();
                //    var payload = { RouteDestinations: manualRoute.ManualRouteDestinations };
                //    VRUIUtilsService.callDirectiveLoad(manualRoute.destinationsAPI, payload, manualRoute.destinationsPromiseDeferred);
                //};

                manualRoute.onManualRouteOriginationsReady = function (api) {
                    manualRoute.originationsAPI = api;
                    manualRoute.originationsPromiseDeferred = UtilsService.createPromiseDeferred();
                    var payload = { RouteOriginations: manualRoute.ManualRouteOriginations };
                    VRUIUtilsService.callDirectiveLoad(manualRoute.originationsAPI, payload, manualRoute.originationsPromiseDeferred);
                };
            }
        }
        return directiveDefinitionObject;
    }]);