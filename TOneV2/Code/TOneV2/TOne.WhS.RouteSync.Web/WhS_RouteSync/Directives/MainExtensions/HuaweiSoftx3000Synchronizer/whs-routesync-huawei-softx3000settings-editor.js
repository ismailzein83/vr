(function (app) {

    'use strict';

    HuaweiSoftX3000SWSyncHuaweiSettings.$inject = ['UtilsService'];

    function HuaweiSoftX3000SWSyncHuaweiSettings(UtilsService) {

        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSoftX3000SWSyncHuaweiSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSoftX3000Synchronizer/Templates/HuaweiSoftX3000RouteSyncSetting.html"
        };

        function HuaweiSoftX3000SWSyncHuaweiSettingsCtor($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.RouteSync.Huawei.SoftX3000.Entities.HuaweiSwitchRouteSynchronizerSettings, TOne.WhS.RouteSync.Huawei",
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

    app.directive('whsRoutesyncHuaweisoftx3000settingsEditor', HuaweiSoftX3000SWSyncHuaweiSettings);
})(app);