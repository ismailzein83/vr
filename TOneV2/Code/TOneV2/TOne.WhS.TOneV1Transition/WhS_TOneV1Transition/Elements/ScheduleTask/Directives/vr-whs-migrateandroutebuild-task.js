"use strict";

app.directive("vrWhsMigrateandroutebuildTask", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MigrateAndRouteBuildTaskDirectiveConstructor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_TOneV1Transition/Elements/ScheduleTask/Directives/Templates/MigrateAndRouteBuildTaskTemplate.html"
        };

        function MigrateAndRouteBuildTaskDirectiveConstructor($scope, ctrl) {
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
                        $type: "TOne.WhS.TOneV1Transition.BP.Arguments.MigrateAndRouteBuildInput, TOne.WhS.TOneV1Transition.BP.Arguments"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
