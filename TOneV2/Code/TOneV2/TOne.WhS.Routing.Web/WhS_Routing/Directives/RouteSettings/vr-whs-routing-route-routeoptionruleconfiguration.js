'use strict';

app.directive('vrWhsRoutingRouteRouteoptionruleconfiguration', ['UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutingProcessTypeEnum',
function (UtilsService, VRUIUtilsService, WhS_Routing_RoutingProcessTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new settingsCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/RouteOptionRuleConfigurationTemplate.html'
    };


    function settingsCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        var customerRouteGridAPI;
        var customerRouteGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var productRouteGridAPI;
        var productRouteGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onCustomerRouteGridReady = function (api) {
                customerRouteGridAPI = api;
                customerRouteGridReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onProductRouteGridReady = function (api) {
                productRouteGridAPI = api;
                productRouteGridReadyPromiseDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([customerRouteGridReadyPromiseDeferred.promise, productRouteGridReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var customerRouteOptionRuleTypeConfiguration;
                var productRouteOptionRuleTypeConfiguration;

                if (payload) {
                    customerRouteOptionRuleTypeConfiguration = payload.CustomerRouteOptionRuleTypeConfiguration;
                    productRouteOptionRuleTypeConfiguration = payload.ProductRouteOptionRuleTypeConfiguration;
                }

                //Loading Customer Route Grid
                var customerRouteGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                var customerRoutePayload = {
                    routeOptionRuleTypeConfiguration: customerRouteOptionRuleTypeConfiguration ? customerRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration : undefined,
                    routingProcessType: WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value
                }
                VRUIUtilsService.callDirectiveLoad(customerRouteGridAPI, customerRoutePayload, customerRouteGridLoadPromiseDeferred);
                promises.push(customerRouteGridLoadPromiseDeferred.promise);

                //Loading Product Route Grid
                var productRouteGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                var productRoutePayload = {
                    routeOptionRuleTypeConfiguration: productRouteOptionRuleTypeConfiguration? productRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration : undefined,
                    routingProcessType: WhS_Routing_RoutingProcessTypeEnum.RoutingProductRoute.value
                }
                VRUIUtilsService.callDirectiveLoad(productRouteGridAPI, productRoutePayload, productRouteGridLoadPromiseDeferred);
                promises.push(productRouteGridLoadPromiseDeferred.promise);


                return UtilsService.waitMultiplePromises(promises);
            }

            api.getData = function () {

                var obj = {
                    $type: "TOne.WhS.Routing.Entities.RouteOptionRuleConfiguration, TOne.WhS.Routing.Entities",
                    CustomerRouteOptionRuleTypeConfiguration: {
                        $type: "TOne.WhS.Routing.Entities.CustomerRouteOptionRuleTypeConfiguration, TOne.WhS.Routing.Entities",
                        RouteOptionRuleTypeConfiguration: customerRouteGridAPI.getData()
                    },
                    ProductRouteOptionRuleTypeConfiguration: {
                        $type: "TOne.WhS.Routing.Entities.ProductRouteOptionRuleTypeConfiguration, TOne.WhS.Routing.Entities",
                        RouteOptionRuleTypeConfiguration: productRouteGridAPI.getData()
                    }
                }

                return obj;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);