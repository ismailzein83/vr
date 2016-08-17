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
            this.initializeController = initializeController;

            var customerRouteDatabaseConfigurationAPI;
            var customerRouteDatabaseConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var productRouteDatabaseConfigurationAPI;
            var productRouteDatabaseConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var prepareCodePrefixesConfigurationAPI;
            var prepareCodePrefixesConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            $scope.scopeModel = {};

            $scope.scopeModel.onCustomerRouteDatabaseConfigurationReady = function (api) {
                customerRouteDatabaseConfigurationAPI = api;
                customerRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onProductRouteDatabaseConfigurationReady = function (api) {
                productRouteDatabaseConfigurationAPI = api;
                productRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onPrepareCodePrefixesConfigurationReady = function (api) {
                prepareCodePrefixesConfigurationAPI = api;
                prepareCodePrefixesConfigurationReadyPromiseDeferred.resolve();
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
                    var prepareCodePrefixesPayload;

                    if (payload != undefined && payload.data != undefined) {
                        customerRoutePayload = payload.data.RouteDatabasesToKeep.CustomerRouteConfiguration;
                        productRoutePayload = payload.data.RouteDatabasesToKeep.ProductRouteConfiguration;
                        prepareCodePrefixesPayload = payload.data.PrepareCodePrefixes;
                    }


                    //Loading Customer Route Database Configuration
                    var customerRouteDatabaseConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    customerRouteDatabaseConfigurationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(customerRouteDatabaseConfigurationAPI, customerRoutePayload, customerRouteDatabaseConfigurationLoadPromiseDeferred);
                        });
                    promises.push(customerRouteDatabaseConfigurationLoadPromiseDeferred.promise);

                    //Loading Product Route Database Configuration
                    var productRouteDatabaseConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    productRouteDatabaseConfigurationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(productRouteDatabaseConfigurationAPI, productRoutePayload, productRouteDatabaseConfigurationLoadPromiseDeferred);
                        });
                    promises.push(productRouteDatabaseConfigurationLoadPromiseDeferred.promise);

                    //Loading Prepare Code Prefixes Configuration
                    var prepareCodePrefixesConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    prepareCodePrefixesConfigurationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(prepareCodePrefixesConfigurationAPI, prepareCodePrefixesPayload, prepareCodePrefixesConfigurationLoadPromiseDeferred);
                        });
                    promises.push(prepareCodePrefixesConfigurationLoadPromiseDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var routeDatabasesToKeep = { CustomerRouteConfiguration: {}, ProductRouteConfiguration: {} };
                    routeDatabasesToKeep.CustomerRouteConfiguration = customerRouteDatabaseConfigurationAPI.getData();
                    routeDatabasesToKeep.ProductRouteConfiguration = productRouteDatabaseConfigurationAPI.getData();

                    var prepareCodePrefixes = prepareCodePrefixesConfigurationAPI.getData();

                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteSettingsData, TOne.WhS.Routing.Entities",
                        RouteDatabasesToKeep: routeDatabasesToKeep,
                        PrepareCodePrefixes: prepareCodePrefixes
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);