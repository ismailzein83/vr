'use strict';
app.directive('vrWhsRoutingOptionrorderByrate', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var routingOptionOrderByRateCtor = new routingOptionOrderByRate(ctrl, $scope);
                routingOptionOrderByRateCtor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return getRoutingOptionOrderByRateTemplate(attrs);
            }

        };

        function getRoutingOptionOrderByRateTemplate(attrs) {
            return '/Client/Modules/WhS_Routing/Directives/RouteRuleSettings/Templates/OptionOrderByRuleDirectiveTemplate.html';
        }

        function routingOptionOrderByRate(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.Orders.OptionOrderByRate, TOne.WhS.Routing.Business.RouteRules.Orders"
                    };
                }

                api.setData = function (routeRuleOptionOrderSettings) {

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);