'use strict';

app.directive('vrCpSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/CPSettings/Templates/CPSettingsTemplate.html"
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
                        $type: "TOne.WhS.CodePreparation.Entities.CPSettingsData, TOne.WhS.CodePreparation.Entities",
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