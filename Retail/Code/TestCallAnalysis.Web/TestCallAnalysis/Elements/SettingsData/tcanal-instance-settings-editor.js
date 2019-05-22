'use strict';

app.directive('tcanalInstanceSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/TestCallAnalysis/Elements/SettingsData/Directives/Templates/NewInstanceSettingsData.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.timeMargin = payload.data.TimeMargin;
                        $scope.scopeModel.timeOutMargin = payload.data.TimeOutMargin;
                    }

                };

                api.getData = function () {
                    return {
                        $type: "TestCallAnalysis.Entities.TCAnalSettingsData, TestCallAnalysis.Entities",
                        TimeMargin: $scope.scopeModel.timeMargin,
                        TimeOutMargin: $scope.scopeModel.timeOutMargin,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);