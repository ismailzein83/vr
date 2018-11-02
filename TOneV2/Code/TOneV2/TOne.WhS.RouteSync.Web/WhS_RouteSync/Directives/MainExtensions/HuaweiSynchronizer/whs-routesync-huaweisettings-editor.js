(function (app) {

    'use strict';

    HuaweiSWSyncHuaweiSettings.$inject = ["UtilsService", 'VRUIUtilsService'];

    function HuaweiSWSyncHuaweiSettings(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new HuaweiSWSyncHuaweiSettingsrCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/HuaweiSynchronizer/Templates/HuaweiRouteSyncSetting.html"
        };

        function HuaweiSWSyncHuaweiSettingsrCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.numberOfRetries = 1;
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        $scope.scopeModel.numberOfTries = payload.settings.NumberOfTries;
                    }
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

    app.directive('whsRoutesyncHuaweisettingsEditor', HuaweiSWSyncHuaweiSettings);

})(app);