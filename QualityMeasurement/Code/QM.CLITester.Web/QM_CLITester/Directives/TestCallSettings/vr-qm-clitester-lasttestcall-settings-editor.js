'use strict';

app.directive('vrQmClitesterLasttestcallSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/QM_CLITester/Directives/TestCallSettings/Templates/LastTestCallsSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        ctrl.lastTestCall = payload.data.LastTestCall;
                    }
                }

                api.getData = function () {
                    return {
                        $type: "QM.CLITester.Entities.LastTestCallsSettingsData, QM.CLITester.Entities",
                        LastTestCall: ctrl.lastTestCall
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);