'use strict';
app.directive('vrWhsRoutingRouterulesettingsPercentageOptimization', ['UtilsService','WhS_Routing_RoutRuleSettingsAPIService',
    function (UtilsService, WhS_Routing_RoutRuleSettingsAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new percentageOptimizationCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Percentage/Templates/OptionPercentageOptimizationDirective.html';
            }

        };

        function percentageOptimizationCtor(ctrl, $scope) {

            function initializeController() {
                ctrl.dataSource = [];
                ctrl.routingOptimizers = [];
                ctrl.selectedRoutingOptimizers = [];
                ctrl.onSelectedRoutingOptimizer = function (routingOptimizer) {
                    var dataItem = {
                        ID: routingOptimizer.ExtensionConfigurationId,
                        Title: routingOptimizer.Title,
                    };
                    ctrl.dataSource.push(dataItem);
                };

                ctrl.onDeSelectedRoutingOptimizer = function (routingOptimizer) {
                    var index = UtilsService.getItemIndexByVal(ctrl.dataSource, routingOptimizer.ExtensionConfigurationId, "ID");
                    ctrl.dataSource.splice(index, 1);
                };
                ctrl.removeRoutingOptimizer = function (routingOptimizer) {
                    ctrl.dataSource.splice(ctrl.dataSource.indexOf(routingOptimizer), 1);
                    ctrl.selectedRoutingOptimizers.splice(UtilsService.getItemIndexByVal(ctrl.selectedRoutingOptimizers, routingOptimizer.ID, "ExtensionConfigurationId"), 1);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {

                    }

                    WhS_Routing_RoutRuleSettingsAPIService.GetRoutingOptimizerSettingsConfigs().then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length ; i++) {
                                var item = response[i];
                                ctrl.routingOptimizers.push(item);
                            }
                            if (payload != undefined && payload.Items != undefined) {
                                for (var i = 0; i < payload.Items.length; i++) {
                                    var item = payload.Items[i];
                                    var routingOptimizer = UtilsService.getItemByVal(ctrl.routingOptimizers, item.RoutingOptimizerItemConfigId, "ExtensionConfigurationId");
                                    var dataItem = {
                                        ID: routingOptimizer.ExtensionConfigurationId,
                                        Title: routingOptimizer.Title,
                                        PercentageFactor: item.PercentageFactor
                                    };
                                    ctrl.selectedRoutingOptimizers.push(routingOptimizer);
                                    ctrl.dataSource.push(dataItem);
                                }
                            }
                        }
                    });

                };

                api.getData = function () {
                    var items;
                    if (ctrl.dataSource.length > 0) {
                        items = [];
                        for (var i = 0; i < ctrl.dataSource.length; i++) {
                            var item = ctrl.dataSource[i];
                            items.push({
                                RoutingOptimizerItemConfigId: item.ID,
                                PercentageFactor: item.PercentageFactor
                            });
                        }
                    }
                    return {
                        $type: "TOne.WhS.Routing.Business.RoutingOptimizationOptionPercentage, TOne.WhS.Routing.Business",
                        Items: items,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);