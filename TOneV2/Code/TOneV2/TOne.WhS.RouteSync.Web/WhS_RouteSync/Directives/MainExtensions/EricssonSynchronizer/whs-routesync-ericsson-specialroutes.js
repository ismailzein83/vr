'use strict';

app.directive('whsRoutesyncEricssonSpecialroutes', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RoutesyncSpecialRoutesEricssonCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonSpecialRoutesTemplate.html"
        };

        function RoutesyncSpecialRoutesEricssonCtor(ctrl, $scope, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.isValid = function () {
                    var datasource = $scope.scopeModel.datasource;
                    if (datasource == undefined)
                        return null;

                    var datasourceLength = $scope.scopeModel.datasource.length;
                    if (datasourceLength <= 1)
                        return null;

                    for (var i = 0; i < datasourceLength - 1; i++) {
                        var currentDataSource = datasource[i];
                        for (var j = i + 1; j < datasourceLength; j++) {
                            {
                                var tempDataSource = datasource[j];
                                if (currentDataSource.TargetBO == tempDataSource.TargetBO)
                                    return 'Target BO should be added once';
                            }
                        }
                    }
                    return null;
                };

                $scope.scopeModel.validateBOs = function (dataItem) {
                    if (dataItem != undefined && dataItem.TargetBO != undefined && dataItem.SourceBO != undefined && dataItem.TargetBO == dataItem.SourceBO)
                        return 'Source and Target BOs should be different';

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

                $scope.scopeModel.addSpecialRoute = function () {
                    var newSpecialRoute = {};
                    extendSpecialRoute(newSpecialRoute);

                    $scope.scopeModel.datasource.push(newSpecialRoute);
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
                    if (payload != undefined && payload.specialRoutes != undefined) {
                        for (var i = 0; i < payload.specialRoutes.length; i++) {
                            var currentSpecialRoute = payload.specialRoutes[i];
                            promises.push(extendSpecialRoute(currentSpecialRoute));
                            $scope.scopeModel.datasource.push(currentSpecialRoute);
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var ericssonSpecialRoutes = [];
                    if ($scope.scopeModel.datasource != undefined) {
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentDatasource = $scope.scopeModel.datasource[i];
                            var specialRoute = {
                                TargetBO: currentDatasource.TargetBO,
                                SourceBO: currentDatasource.SourceBO
                            };

                            if (currentDatasource.typeAPI != undefined)
                                specialRoute.Settings = currentDatasource.typeAPI.getData();

                            ericssonSpecialRoutes.push(specialRoute);
                        }
                    }
                    return ericssonSpecialRoutes;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function extendSpecialRoute(specialRoute) {
                var routePromises = [];
                specialRoute.Id = UtilsService.guid();
                var typePromiseDeferred = UtilsService.createPromiseDeferred();
                routePromises.push(typePromiseDeferred.promise);

                specialRoute.onSpecialRouteTypeReady = function (api) {
                    specialRoute.typeAPI = api;
                    var payload = { Settings: specialRoute.Settings };
                    VRUIUtilsService.callDirectiveLoad(specialRoute.typeAPI, payload, typePromiseDeferred);
                };
                return UtilsService.waitMultiplePromises(routePromises);
            }
        }
        return directiveDefinitionObject;
    }]);