'use strict';
app.directive('vrWhsBeRouteruleBlock', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var beRouteRuleBlockCtor = new beRouteRuleBlock(ctrl, $scope);
                beRouteRuleBlockCtor.initializeController();

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
                return getBeRouteRuleBlockTemplate(attrs);
            }

        };

        function getBeRouteRuleBlockTemplate(attrs) {
            return '';
        }

        function beRouteRuleBlock(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.BlockRouteRule, TOne.WhS.Routing.Business.RouteRules"
                    };
                }

                api.setData = function (routeRuleSettings) {
                    
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);