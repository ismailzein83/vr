(function (app) {

    'use strict';

    RadiusSQLDataManagerConfig.$inject = ["UtilsService"];

    function RadiusSQLDataManagerConfig(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var radiusSQLDataManagerSetting = new RadiusSQLDataManagerSetting($scope, ctrl, $attrs);
                radiusSQLDataManagerSetting.initializeController();
            },
            controllerAs: "ctrlData",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/RadiusDataManagers/Templates/SingleSQLRadiusSWDataManagerTemplate.html"

        };
        function RadiusSQLDataManagerSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isLoading = false;

                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.connectionString = payload.radiusDataManagersSettings.ConnectionString.ConnectionString;
                    }
                };

                api.getData = getData;

                function getData() {
                    var data = {
                        $type: "TOne.WhS.RouteSync.TelesRadius.SQL.RadiusSQLDataManager, TOne.WhS.RouteSync.TelesRadius.SQL",
                        ConnectionString: { ConnectionString: $scope.scopeModel.connectionString }
                    };
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncTelesradiusSqldatamanager', RadiusSQLDataManagerConfig);

})(app);