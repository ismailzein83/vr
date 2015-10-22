'use strict';
app.directive('vrWhsRoutingOptionfilterRemoveloss', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var routingOptionFilterRemoveLossCtor = new routingOptionFilterRemoveLoss(ctrl, $scope);
                routingOptionFilterRemoveLossCtor.initializeController();

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
                return getRoutingOptionFilterRemoveLossTemplate(attrs);
            }

        };

        function getRoutingOptionFilterRemoveLossTemplate(attrs) {
            return 'Remove Loss';
        }

        function routingOptionFilterRemoveLoss(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.Filters.OptionFilterLoss, TOne.WhS.Routing.Business.RouteRules.Filters"
                    };
                }

                api.setData = function (routeRuleOptionFilterSettings) {

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);