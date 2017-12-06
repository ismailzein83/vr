(function (app) {

    'use strict';

    CDRComparisonSettingsEditorDirective.$inject = [];

    function CDRComparisonSettingsEditorDirective() {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cdrComparisonSettingsEditor = new CDRComparisonSettingsEditor($scope, ctrl, $attrs);
                cdrComparisonSettingsEditor.initializeController();
            },
            controllerAs: 'cdrComparisonSettingsCtrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CDRComparison/Directives/Templates/CDRComparisonSettingsTemplate.html'
        };

        function CDRComparisonSettingsEditor($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.taskTimeoutInSeconds = payload.data.TaskTimeoutInSeconds;
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'CDRComparison.Entities.CDRComparisonSettingData, CDRComparison.Entities',
                        TaskTimeoutInSeconds: $scope.scopeModel.taskTimeoutInSeconds
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('cdrcomparisonCdrcomparisonsettingsEditor', CDRComparisonSettingsEditorDirective);

})(app);