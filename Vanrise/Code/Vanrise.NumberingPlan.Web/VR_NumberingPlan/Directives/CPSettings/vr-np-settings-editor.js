'use strict';

app.directive('vrNpSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_NumberingPlan/Directives/CPSettings/Templates/CPSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        ctrl.effectiveDateOffset = payload.data.EffectiveDateOffset;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.NumberingPlan.Entities.CPSettingsData, Vanrise.NumberingPlan.Entities",
                        EffectiveDateOffset: ctrl.effectiveDateOffset
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);