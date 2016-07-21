'use strict';

app.directive('vrWhsRoutingRouteSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/RouteSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            var customerRouteDatabaseConfigurationAPI;
            var customerRouteDatabaseConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var productRouteDatabaseConfigurationAPI;
            var productRouteDatabaseConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.onCustomerRouteDatabaseConfigurationReady = function (api) {
                customerRouteDatabaseConfigurationAPI = api;
                customerRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
            }

            $scope.onProductRouteDatabaseConfigurationReady = function (api) {
                productRouteDatabaseConfigurationAPI = api;
                productRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
            }

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var customerRoutePayload;
                    var productRoutePayload;


                    if (payload != undefined && payload.data != undefined) {
                        customerRoutePayload = payload.data.RouteDatabasesToKeep.CustomerRouteConfiguration;
                        productRoutePayload = payload.data.RouteDatabasesToKeep.ProductRouteConfiguration;
                    }

                    var customerRouteDatabaseConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    productRouteDatabaseConfigurationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(customerRouteDatabaseConfigurationAPI, customerRoutePayload, customerRouteDatabaseConfigurationLoadPromiseDeferred);
                        });

                    promises.push(customerRouteDatabaseConfigurationLoadPromiseDeferred.promise);


                    var productRouteDatabaseConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    productRouteDatabaseConfigurationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(productRouteDatabaseConfigurationAPI, productRoutePayload, productRouteDatabaseConfigurationLoadPromiseDeferred);
                        });

                    promises.push(productRouteDatabaseConfigurationLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var routeDatabasesToKeep = { CustomerRouteConfiguration: {}, ProductRouteConfiguration: {} };
                    routeDatabasesToKeep.CustomerRouteConfiguration = customerRouteDatabaseConfigurationAPI.getData();
                    routeDatabasesToKeep.ProductRouteConfiguration = productRouteDatabaseConfigurationAPI.getData();
                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteSettingsData, TOne.WhS.Routing.Entities",
                        RouteDatabasesToKeep: routeDatabasesToKeep
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);