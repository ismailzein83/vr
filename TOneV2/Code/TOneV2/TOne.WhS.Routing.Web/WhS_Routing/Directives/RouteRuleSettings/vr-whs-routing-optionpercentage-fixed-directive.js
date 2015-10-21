'use strict';
app.directive('vrWhsRoutingOptionpercentageFixed', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var routingOptionPercentageFixedCtor = new routingOptionPercentageFixed(ctrl, $scope);
                routingOptionPercentageFixedCtor.initializeController();

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
                return getRoutingOptionPercentageFixedTemplate(attrs);
            }

        };

        function getRoutingOptionPercentageFixedTemplate(attrs) {
            return '/Client/Modules/WhS_Routing/Directives/RouteRuleSettings/Templates/OptionPercentageFixedDirectiveTemplate.html';
        }

        function routingOptionPercentageFixed(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.Percentages.FixedOptionPercentage, TOne.WhS.Routing.Business.RouteRules.Percentages"
                    };
                }

                api.setData = function (RouteRuleOptionPercentageSettings) {

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);