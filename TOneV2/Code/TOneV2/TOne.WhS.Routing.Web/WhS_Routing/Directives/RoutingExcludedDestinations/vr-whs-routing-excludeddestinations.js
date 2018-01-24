(function (app) {

    'use strict';

    RoutingExcludedDestinationsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutingConfigurationAPIService'];

    function RoutingExcludedDestinationsDirective(UtilsService, VRUIUtilsService, WhS_Routing_RoutingConfigurationAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RoutingExcludedDestinationsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RoutingExcludedDestinations/Templates/RoutingExcludedDestinationsTemplate.html"
        };

        function RoutingExcludedDestinationsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var excludedDestinations;
            var isLinkedRouteRule;
            var linkedCode;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var directivePayload = {
                        isLinkedRouteRule: isLinkedRouteRule,
                        linkedCode: linkedCode
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    if (payload != undefined) {
                        excludedDestinations = payload.excludedDestinations;
                        isLinkedRouteRule = payload.isLinkedRouteRule;
                        linkedCode = payload.linkedCode;
                    }

                    var getRoutingExcludedDestinationsTemplateConfigs = getRoutingExcludedDestinationsTemplateConfigs();
                    promises.push(getRoutingExcludedDestinationsTemplateConfigs);

                    if (excludedDestinations != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    function getRoutingExcludedDestinationsTemplateConfigs() {
                        return WhS_Routing_RoutingConfigurationAPIService.GetRoutingExcludedDestinationsTemplateConfigs().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (excludedDestinations != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, excludedDestinations.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                excludedDestinations: excludedDestinations,
                                isLinkedRouteRule: payload.isLinkedRouteRule,
                                linkedCode: payload.linkedCode
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsRoutingExcludeddestinations', RoutingExcludedDestinationsDirective);

})(app);