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

            var subProcessSettingsAPI;
            var subProcessSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerRouteSettingsAPI;
            var customerRouteSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var routeOptionRuleConfigurationAPI;
            var routeOptionRuleConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var qualityConfigurationAPI;
            var qualityConfigurationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCustomerRouteDatabaseConfigurationReady = function (api) {
                    customerRouteDatabaseConfigurationAPI = api;
                    customerRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onProductRouteDatabaseConfigurationReady = function (api) {
                    productRouteDatabaseConfigurationAPI = api;
                    productRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSubProcessSettingsReady = function (api) {
                    subProcessSettingsAPI = api;
                    subProcessSettingsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCustomerRouteSettingsReady = function (api) {
                    customerRouteSettingsAPI = api;
                    customerRouteSettingsReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRouteOptionRuleConfigurationReady = function (api) {
                    routeOptionRuleConfigurationAPI = api;
                    routeOptionRuleConfigurationReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onQualityConfigurationReady = function (api) {
                    qualityConfigurationAPI = api;
                    qualityConfigurationReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var customerRoutePayload;
                    var productRoutePayload;
                    var subProcessSettingsPayload;
                    var customerRouteSettingsPayload;
                    var routeOptionRuleConfiguration;
                    var qualityConfiguration;

                    if (payload != undefined && payload.data != undefined) {
                        customerRoutePayload = payload.data.RouteDatabasesToKeep.CustomerRouteConfiguration;
                        productRoutePayload = payload.data.RouteDatabasesToKeep.ProductRouteConfiguration;
                        subProcessSettingsPayload = payload.data.SubProcessSettings;
                        customerRouteSettingsPayload = payload.data.RouteBuildConfiguration;
                        routeOptionRuleConfiguration = payload.data.RouteOptionRuleConfiguration;
                        qualityConfiguration = payload.data.QualityConfiguration;
                    }


                    //Loading Customer Route Database Configuration
                    var customerRouteDatabaseConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    customerRouteDatabaseConfigurationReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(customerRouteDatabaseConfigurationAPI, customerRoutePayload, customerRouteDatabaseConfigurationLoadPromiseDeferred);
                    });
                    promises.push(customerRouteDatabaseConfigurationLoadPromiseDeferred.promise);

                    //Loading Product Route Database Configuration
                    var productRouteDatabaseConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    productRouteDatabaseConfigurationReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(productRouteDatabaseConfigurationAPI, productRoutePayload, productRouteDatabaseConfigurationLoadPromiseDeferred);
                    });
                    promises.push(productRouteDatabaseConfigurationLoadPromiseDeferred.promise);

                    //Loading Sub Process Settings
                    var subProcessSettingsConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    subProcessSettingsReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(subProcessSettingsAPI, subProcessSettingsPayload, subProcessSettingsConfigurationLoadPromiseDeferred);
                    });
                    promises.push(subProcessSettingsConfigurationLoadPromiseDeferred.promise);

                    //Loading Customer Route Settings
                    var customerRouteSettingsConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    customerRouteSettingsReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(customerRouteSettingsAPI, customerRouteSettingsPayload, customerRouteSettingsConfigurationLoadPromiseDeferred);
                    });
                    promises.push(customerRouteSettingsConfigurationLoadPromiseDeferred.promise);

                    //Loading Route Option Rule Configuration
                    var routeOptionRuleConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    routeOptionRuleConfigurationReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(routeOptionRuleConfigurationAPI, routeOptionRuleConfiguration, routeOptionRuleConfigurationLoadPromiseDeferred);
                    });
                    promises.push(routeOptionRuleConfigurationLoadPromiseDeferred.promise);

                    //Loading Route Quality Configuration
                    var qualityConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    qualityConfigurationReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(qualityConfigurationAPI, qualityConfiguration, qualityConfigurationLoadPromiseDeferred);
                    });
                    promises.push(qualityConfigurationLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteSettingsData, TOne.WhS.Routing.Entities",
                        RouteDatabasesToKeep: {
                            CustomerRouteConfiguration: customerRouteDatabaseConfigurationAPI.getData(),
                            ProductRouteConfiguration: productRouteDatabaseConfigurationAPI.getData()
                        },
                        SubProcessSettings: subProcessSettingsAPI.getData(),
                        RouteBuildConfiguration: customerRouteSettingsAPI.getData(),
                        RouteOptionRuleConfiguration: routeOptionRuleConfigurationAPI.getData(),
                        QualityConfiguration: {
                            RouteRuleQualityConfigurationList: qualityConfigurationAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);