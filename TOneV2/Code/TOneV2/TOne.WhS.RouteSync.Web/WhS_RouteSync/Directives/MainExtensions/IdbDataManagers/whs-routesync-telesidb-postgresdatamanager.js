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
                $scope.scopeModel.redundantConnectionStrings = [];
                $scope.scopeModel.isLoading = false;

                $scope.scopeModel.addRedundantConnectionString = function () {
                    $scope.scopeModel.redundantConnectionStrings.push({ ConnectionString: undefined });
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.connectionString = payload.idbDataManagersSettings.ConnectionString.ConnectionString;
                        $scope.scopeModel.schemaName = payload.idbDataManagersSettings.ConnectionString.SchemaName;

                        if (payload.idbDataManagersSettings.RedundantConnectionStrings != undefined)
                            $scope.scopeModel.redundantConnectionStrings = payload.idbDataManagersSettings.RedundantConnectionStrings;
                    }
                };

                api.getData = function getData() {

                    var data = {
                        $type: "TOne.WhS.RouteSync.TelesIdb.Postgres.IdbPostgresDataManager, TOne.WhS.RouteSync.TelesIdb.Postgres",
                        ConnectionString: { ConnectionString: $scope.scopeModel.connectionString, SchemaName: $scope.scopeModel.schemaName },
                        RedundantConnectionStrings: $scope.scopeModel.redundantConnectionStrings
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncTelesidbPostgresdatamanager', telesIdbPostgresDataManager);

})(app);