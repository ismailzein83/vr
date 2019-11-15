(function (app) {

    'use strict';

    CataleyaPostgresDataManager.$inject = ['UtilsService', 'VRUIUtilsService'];

    function CataleyaPostgresDataManager(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CataleyaPostgresDataManagerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Whs_RouteSync/Directives/MainExtensions/Cataleya/DataManagers/Templates/CataleyaPostgresDataManagerTemplate.html'
        };

        function CataleyaPostgresDataManagerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dbConnectionSelectorAPI;
            var dbConnectionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDBConnectionSelectorReady = function (api) {
                    dbConnectionSelectorAPI = api;
                    dbConnectionSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var databaseConnection;

                    if (payload != undefined) {
                        var cataleyaDataManager = payload.cataleyaDataManager;
                        if (cataleyaDataManager != undefined && cataleyaDataManager.DatabaseConnection != undefined) {
                            databaseConnection = cataleyaDataManager.DatabaseConnection;
                            $scope.scopeModel.schemaName = databaseConnection.SchemaName;
                        }
                    }

                    var promises = [];

                    var loadDBConnectionSelectorPromise = getLoadDBConnectionSelectorPromise();
                    promises.push(loadDBConnectionSelectorPromise);

                    function getLoadDBConnectionSelectorPromise() {
                        var dbConnectionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dbConnectionSelectorReadyPromiseDeferred.promise.then(function () {

                            var dbConnectionSelectorPayload = {
                                filter: {
                                    ConnectionTypeIds: ['8224B27C-C128-4150-A4E4-5E2034BB3A36'] // VRSQLConnectionFilter
                                }
                            };
                            if (databaseConnection != undefined) {
                                dbConnectionSelectorPayload.selectedIds = databaseConnection.DBConnectionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(dbConnectionSelectorAPI, dbConnectionSelectorPayload, dbConnectionSelectorLoadPromiseDeferred);
                        });

                        return dbConnectionSelectorLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Cataleya.Data.Postgres.CataleyaPostgresDataManager, TOne.WhS.RouteSync.Cataleya",
                        DatabaseConnection: {
                            SchemaName: $scope.scopeModel.schemaName,
                            DBConnectionId: dbConnectionSelectorAPI.getSelectedIds()
                        }
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncCataleyapostgresDatamanager', CataleyaPostgresDataManager);
})(app);