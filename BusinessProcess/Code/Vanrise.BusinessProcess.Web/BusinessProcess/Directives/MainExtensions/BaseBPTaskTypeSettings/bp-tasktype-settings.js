(function (app) {

    'use strict';
    TaskTypeSettings.$inject = [ 'UtilsService', 'VRUIUtilsService'];

    function TaskTypeSettings(UtilsService, VRUIUtilsService ) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/BaseBPTaskTypeSettings/Templates/BPTaskTypeSettingsTemplate.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.editor = payload.Editor;
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.BusinessProcess.Entities.BPTaskTypeSettings, Vanrise.BusinessProcess.Entities",
                        Editor:$scope.scopeModel.editor
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('bpTasktypeSettings', TaskTypeSettings);

})(app);
