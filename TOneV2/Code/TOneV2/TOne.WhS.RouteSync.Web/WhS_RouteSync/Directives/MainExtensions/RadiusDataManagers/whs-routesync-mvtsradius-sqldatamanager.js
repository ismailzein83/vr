﻿(function (app) {

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
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/RadiusDataManagers/Templates/DoubleSQLRadiusSWDataManagerTemplate.html"

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
                        $scope.scopeModel.redundantConnectionString = payload.radiusDataManagersSettings.RedundantConnectionStrings != undefined ? payload.radiusDataManagersSettings.RedundantConnectionStrings[0].ConnectionString : null;
                    }
                };

                api.getData = getData;

                function getData() {
                    var redundantConnectionStrings;
                    if ($scope.scopeModel.redundantConnectionString != undefined) {
                        redundantConnectionStrings = [];
                        redundantConnectionStrings.push({ ConnectionString: $scope.scopeModel.redundantConnectionString });
                    }

                    var data = {
                        $type: "TOne.WhS.RouteSync.MVTSRadius.SQL.RadiusSQLDataManager, TOne.WhS.RouteSync.MVTSRadius.SQL",
                        ConnectionString: { ConnectionString: $scope.scopeModel.connectionString },
                        RedundantConnectionStrings: redundantConnectionStrings
                    };
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncMvtsradiusSqldatamanager', RadiusSQLDataManagerConfig);

})(app);