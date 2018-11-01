(function (app) {
    'use strict';
    ManualRouteOriginationSelective.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonManualRoutesAPIService'];
    function ManualRouteOriginationSelective(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonManualRoutesAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualRouteOriginationSelectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/ManualRoutes/Templates/EricssonManualRouteOriginationSelectiveTemplate.html"
        };

        function ManualRouteOriginationSelectiveCtor($scope, ctrl, $attrs) {
            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var routeOriginations;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.extensionConfigs = [];
                $scope.scopeModel.selectedExtensionConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var directivePayload = {
                        RouteOriginations: routeOriginations
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        routeOriginations = payload.RouteOriginations;
                    }

                    var loadRouteOriginationsTypeExtensionCofigsPromise = loadRouteOriginationsTypeExtensionCofigs();

                    promises.push(loadRouteOriginationsTypeExtensionCofigsPromise);

                    function loadRouteOriginationsTypeExtensionCofigs() {
                        return WhS_RouteSync_EricssonManualRoutesAPIService.GetManualRouteOriginationsTypeExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.extensionConfigs.push(response[i]);
                                }
                                if (routeOriginations != undefined) {
                                    $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, routeOriginations.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    //return loadRouteOriginationsTypeExtensionCofigsPromise;
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedExtensionConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                    }
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonManualrouteOriginationSelective', ManualRouteOriginationSelective);

})(app);