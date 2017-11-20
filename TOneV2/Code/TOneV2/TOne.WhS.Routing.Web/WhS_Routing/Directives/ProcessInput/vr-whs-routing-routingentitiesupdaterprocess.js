"use strict";

app.directive("vrWhsRoutingRoutingentitiesupdaterprocess", ['UtilsService',
    function (UtilsService) {
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
            templateUrl: "/Client/Modules/WhS_Routing/Directives/ProcessInput/Templates/RoutingEntitiesUpdaterProcessTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: 'TOne.WhS.Routing.BP.Arguments.RoutingEntitiesUpdaterProcessInput, TOne.WhS.Routing.BP.Arguments'
                        }
                    };
                };

                api.load = function (payload) {
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
