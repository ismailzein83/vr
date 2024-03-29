﻿"use strict";

app.directive("vrWhsMigrateandroutebuildProcess", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MigrateAndRouteBuildProcessDirectiveConstructor($scope, ctrl);
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
            templateUrl: "/Client/Modules/WhS_TOneV1Transition/Elements/ProcessInput/Directives/Templates/MigrateAndRouteBuildProcessTemplate.html"
        };

        function MigrateAndRouteBuildProcessDirectiveConstructor($scope, ctrl) {
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
                        InputArguments: {
                            $type: "TOne.WhS.TOneV1Transition.BP.Arguments.MigrateAndRouteBuildInput, TOne.WhS.TOneV1Transition.BP.Arguments"
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
