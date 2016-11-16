'use strict';
app.directive('vrWhsRoutingRoutingoptimizersettingsMargin', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new MarginRoutingOptimizerItemSettings(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/Settings/Extensions/Templates/MarginRoutingOptimizerItemSettingsTemplate.html';
            }

        };

        function MarginRoutingOptimizerItemSettings(ctrl, $scope) {
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                    }

                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Entities.MarginRoutingOptimizerItemSettings, TOne.WhS.Routing.Entities"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);