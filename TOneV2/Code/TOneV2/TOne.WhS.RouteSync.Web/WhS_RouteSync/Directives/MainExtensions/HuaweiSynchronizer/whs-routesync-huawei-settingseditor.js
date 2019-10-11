(function (app) {

    'use strict';

    HuaweiSWSyncSettingsEditor.$inject = ["UtilsService"];

    function HuaweiSWSyncSettingsEditor(UtilsService) {

        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSWSyncSettingsEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSynchronizer/Templates/HuaweiRouteSyncSetting.html"
        };

        function HuaweiSWSyncSettingsEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.numberOfRetries = 1;

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.numberOfTries = payload.settings.NumberOfTries;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Huawei.Entities.HuaweiSwitchRouteSynchronizerSettings,TOne.WhS.RouteSync.Huawei",
                        NumberOfTries: $scope.scopeModel.numberOfTries
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncHuaweiSettingseditor', HuaweiSWSyncSettingsEditor);
})(app);