(function (app) {

    'use strict';

    TelesRadiusSWSync.$inject = ["UtilsService", 'VRUIUtilsService'];

    function TelesRadiusSWSync(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var telesRadiusSWSyncronizer = new TelesRadiusSWSyncronizer($scope, ctrl, $attrs);
                telesRadiusSWSyncronizer.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/TelesSynchronizer/Templates/TelesRadiusSWSyncTemplate.html"

        };
        function TelesRadiusSWSyncronizer($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var telesRadiusSWSynSettings;

                    if (payload != undefined) {
                        telesRadiusSWSynSettings = payload.switchSynchronizerSettings;
                    }

                    if (telesRadiusSWSynSettings != undefined) {
                        $scope.scopeModel.connectionString = telesRadiusSWSynSettings.ConnectionString;
                    }
                };

                api.getData = getData;

                function getData() {
                    var data = {
                        $type: "TOne.WhS.RouteSync.TelesRadius.TelesRadiusSWSync, TOne.WhS.RouteSync.TelesRadius",
                        ConnectionString: $scope.scopeModel.connectionString
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncTelesradiusSwsync', TelesRadiusSWSync);

})(app);