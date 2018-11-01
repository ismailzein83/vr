(function (app) {
    'use strict';
    ManualRouteDestinationSelective.$inject = ["UtilsService", 'VRUIUtilsService', 'WhS_RouteSync_EricssonManualRoutesAPIService'];
    function ManualRouteDestinationSelective(UtilsService, VRUIUtilsService, WhS_RouteSync_EricssonManualRoutesAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ManualRouteDestinationSelectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/ManualRoutes/Templates/EricssonManualRouteDestinationSelectiveTemplate.html"
        };

        function ManualRouteDestinationSelectiveCtor($scope, ctrl, $attrs) {
            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var routeDestinations;

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
                        RouteDestinations: routeDestinations
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
                        routeDestinations = payload.RouteDestinations;
                    }

                    var loadRouteDestinationsTypeExtensionCofigsPromise = loadRouteDestinationsTypeExtensionCofigs();

                    promises.push(loadRouteDestinationsTypeExtensionCofigsPromise);

                    function loadRouteDestinationsTypeExtensionCofigs() {
                        return WhS_RouteSync_EricssonManualRoutesAPIService.GetManualRouteDestinationsTypeExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.extensionConfigs.push(response[i]);
                                }
                                if (routeDestinations != undefined) {
                                    $scope.scopeModel.selectedExtensionConfig = UtilsService.getItemByVal($scope.scopeModel.extensionConfigs, routeDestinations.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    //return loadRouteDestinationsTypeExtensionCofigsPromise;
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

    app.directive('whsRoutesyncEricssonManualrouteDestinationSelective', ManualRouteDestinationSelective);

})(app);