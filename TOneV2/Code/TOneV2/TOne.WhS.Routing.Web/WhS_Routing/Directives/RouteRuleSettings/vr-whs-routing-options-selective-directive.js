'use strict';
app.directive('vrWhsRoutingOptionsSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var routingSelectiveOptionsCtor = new routingSelectiveOptions(ctrl, $scope);
                routingSelectiveOptionsCtor.initializeController();

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
                return getRoutingSelectiveOptionsTemplate(attrs);
            }

        };

        function getRoutingSelectiveOptionsTemplate(attrs) {
            return '/Client/Modules/WhS_Routing/Directives/RouteRuleSettings/Templates/SelectiveOptionDirectiveTemplate.html';
        }

        function routingSelectiveOptions(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {

                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups.SelectiveOptions, TOne.WhS.Routing.Business",
                        Options: null
                    };
                }

                api.setData = function (routeOptionSettingsGroup) {

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);