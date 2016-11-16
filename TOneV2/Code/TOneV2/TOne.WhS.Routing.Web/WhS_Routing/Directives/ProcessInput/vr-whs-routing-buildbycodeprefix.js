"use strict";

app.directive("vrWhsRoutingBuildbycodeprefix", [ function () {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/ProcessInput/Templates/BuildRoutesByCodePrefixTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            
            var api = {};
            api.getData = function () {
                return {
                    InputArguments: {
                        $type: "TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput, TOne.WhS.Routing.BP.Arguments",
                        CodePrefix: $scope.codePrefix,
                        EffectiveOn: $scope.effectiveOn,
                        IsFuture: $scope.isFuture
                    }
                };
            };

            api.load = function (payload) {

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
