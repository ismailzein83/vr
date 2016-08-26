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


            $scope.scopeModel = {};

            $scope.scopeModel.onCustomerRouteDatabaseConfigurationReady = function (api) {
                customerRouteDatabaseConfigurationAPI = api;
                customerRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onProductRouteDatabaseConfigurationReady = function (api) {
                productRouteDatabaseConfigurationAPI = api;
                productRouteDatabaseConfigurationReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onSubProcessSettingsReady = function (api) {
                subProcessSettingsAPI = api;
                subProcessSettingsReadyPromiseDeferred.resolve();
            }

            $scope.scopeModel.onCustomerRouteSettingsReady = function (api) {
                customerRouteSettingsAPI = api;
                customerRouteSettingsReadyPromiseDeferred.resolve();
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
                    var subProcessSettingsPayload;
                    var customerRouteSettingsPayload;

                    if (payload != undefined && payload.data != undefined) {
                        customerRoutePayload = payload.data.RouteDatabasesToKeep.CustomerRouteConfiguration;
                        productRoutePayload = payload.data.RouteDatabasesToKeep.ProductRouteConfiguration;
                        subProcessSettingsPayload = payload.data.SubProcessSettings;
                        customerRouteSettingsPayload = payload.data.RouteBuildConfiguration;
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

                    //Loading Sub Process Settings
                    var subProcessSettingsConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    subProcessSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(subProcessSettingsAPI, subProcessSettingsPayload, subProcessSettingsConfigurationLoadPromiseDeferred);
                        });
                    promises.push(subProcessSettingsConfigurationLoadPromiseDeferred.promise);

                    //Loading Customer Route Settings
                    var customerRouteSettingsConfigurationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    customerRouteSettingsReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(customerRouteSettingsAPI, customerRouteSettingsPayload, customerRouteSettingsConfigurationLoadPromiseDeferred);
                        });
                    promises.push(customerRouteSettingsConfigurationLoadPromiseDeferred.promise);


                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteSettingsData, TOne.WhS.Routing.Entities",
                        RouteDatabasesToKeep: {
                            CustomerRouteConfiguration: customerRouteDatabaseConfigurationAPI.getData(),
                            ProductRouteConfiguration: productRouteDatabaseConfigurationAPI.getData()
                        },
                        SubProcessSettings: subProcessSettingsAPI.getData(),
                        RouteBuildConfiguration: customerRouteSettingsAPI.getData()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);