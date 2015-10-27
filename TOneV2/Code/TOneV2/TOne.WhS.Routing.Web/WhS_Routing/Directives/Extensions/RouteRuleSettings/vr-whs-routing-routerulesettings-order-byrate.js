﻿'use strict';
app.directive('vrWhsRoutingRouterulesettingsOrderByrate', ['UtilsService',
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
            template: function (element, attrs) {
                return getRoutingOptionOrderByRateTemplate(attrs);
            }

        };

        function getRoutingOptionOrderByRateTemplate(attrs) {
            return '';
        }

        function routingOptionOrderByRate(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (routeRuleOptionOrderSettings) {
                    
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.Orders.OptionOrderByRate, TOne.WhS.Routing.Business"
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);