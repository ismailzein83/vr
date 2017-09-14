(function (app) {

    'use strict';

    telesIdbPostgresDataManager.$inject = ["UtilsService"];

    function telesIdbPostgresDataManager(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TelesIdbPostgresDataManagerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlData",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/IdbDataManagers/Templates/TelesPostgresIdbSWDataManagerTemplate.html"
        };

        function TelesIdbPostgresDataManagerCtor($scope, ctrl, $attrs) {
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
                        $scope.scopeModel.connectionString = payload.idbDataManagersSettings.ConnectionString.ConnectionString;
                        $scope.scopeModel.redundantConnectionString = payload.idbDataManagersSettings.RedundantConnectionStrings != undefined ? payload.idbDataManagersSettings.RedundantConnectionStrings[0].ConnectionString : null;
                    }
                };

                api.getData = function getData() {

                    var redundantConnectionStrings;

                    if ($scope.scopeModel.redundantConnectionString != undefined) {
                        redundantConnectionStrings = [];
                        redundantConnectionStrings.push({ ConnectionString: $scope.scopeModel.redundantConnectionString });
                    }

                    var data = {
                        $type: "TOne.WhS.RouteSync.TelesIdb.Postgres.IdbPostgresDataManager, TOne.WhS.RouteSync.TelesIdb.Postgres",
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

    app.directive('whsRoutesyncTelesidbPostgresdatamanager', telesIdbPostgresDataManager);

})(app);