'use strict';
app.directive('vrWhsRoutingRouteoptionrulesettingsBlock', ['UtilsService',
    function (UtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new blockCtor(ctrl, $scope);
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
            templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/RouteOptionRuleSettings/Templates/RouteOptionRulesSettingsBlockTemplate.html'
        };

        function blockCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.BlockRouteOptionRule, TOne.WhS.Routing.Business"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);