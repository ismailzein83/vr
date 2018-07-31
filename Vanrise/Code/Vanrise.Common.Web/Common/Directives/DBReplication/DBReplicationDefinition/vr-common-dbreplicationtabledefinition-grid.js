'use strict';

app.directive('vrCommonDbreplicationtabledefinitionGrid', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new DBReplicationTableDefinitionGridDirectiveCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/Common/Directives/DBReplication/DBReplicationDefinition/Templates/DBReplicationTableDefinitionGridTemplate.html"
    };

    function DBReplicationTableDefinitionGridDirectiveCtor(ctrl, $scope, attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.datasource = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.addDBReplicationTableDefinition = function () {
                var dbReplicationTableDefinition = {
                    id: UtilsService.guid()
                };

                dbReplicationTableDefinition.onPreInsertSelectorReady = function (api) {
                    dbReplicationTableDefinition.preInsertSelectorAPI = api;
                    var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dbReplicationTableDefinition.preInsertSelectorAPI, undefined, setLoader);
                };

                $scope.scopeModel.datasource.push(dbReplicationTableDefinition);
            };

            $scope.scopeModel.removeDBReplicationTableDefinition = function (dataItem) {
                var index = $scope.scopeModel.datasource.indexOf(dataItem);
                $scope.scopeModel.datasource.splice(index, 1);
            };

            $scope.scopeModel.isDataItemValid = function (dataItem) {
                if (dataItem.FilterDateTimeColumn != undefined && dataItem.IdColumn != undefined) {
                    return 'Either Filter Date Time Column or Id Column should be filled , but not both';
                }
                return null;
            };

            $scope.scopeModel.isGridValid = function () {
                if ($scope.scopeModel.datasource.length == 0) {
                    return 'At least one item should be added';
                }
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var currentItem = $scope.scopeModel.datasource[i];
                    if (currentItem.TableName == undefined || currentItem.TableSchema == undefined)
                        continue;

                    var currentItemTableName = currentItem.TableName.toLowerCase();
                    var currentItemTableSchema = currentItem.TableSchema.toLowerCase();

                    for (var j = i + 1; j < $scope.scopeModel.datasource.length; j++) {
                        var dataItem = $scope.scopeModel.datasource[j];

                        if (dataItem.TableName == undefined || dataItem.TableSchema == undefined)
                            continue;

                        var dataItemTableName = dataItem.TableName.toLowerCase();
                        var dataItemTableSchema = dataItem.TableSchema.toLowerCase();

                        if (currentItemTableName == dataItemTableName && currentItemTableSchema == dataItemTableSchema) {
                            return 'Same Table (Name and Schema) is added more than once';
                        }
                    }
                }
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                $scope.scopeModel.datasource = [];

                if (payload != undefined && payload.dbReplicationTablesDefinitions != undefined) {
                    for (var x = 0; x < payload.dbReplicationTablesDefinitions.length; x++) {
                        var currentItem = payload.dbReplicationTablesDefinitions[x];
                        var gridItem = {
                            TableName: currentItem.TableName,
                            TableSchema: currentItem.TableSchema,
                            FilterDateTimeColumn: currentItem.FilterDateTimeColumn,
                            IdColumn: currentItem.IdColumn,
                            ChunkSize: currentItem.ChunkSize,
                            readyPreInsertSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPreInsertSelector: UtilsService.createPromiseDeferred()
                        };
                        gridItem.payload = currentItem.DBReplicationPreInsert;

                        addItemtoGrid(gridItem);
                    }
                }

                function addItemtoGrid(gridItem) {
                    promises.push(gridItem.loadPreInsertSelector.promise);

                    gridItem.onPreInsertSelectorReady = function (api) {
                        gridItem.preInsertSelectorAPI = api;
                        gridItem.readyPreInsertSelectorPromiseDeferred.resolve();
                    };
                    UtilsService.waitMultiplePromises([gridItem.readyPreInsertSelectorPromiseDeferred.promise]).then(function () {
                        var preInsertSelectorPayload = {
                            DBReplicationPreInsert: gridItem.payload
                        };
                        VRUIUtilsService.callDirectiveLoad(gridItem.preInsertSelectorAPI, preInsertSelectorPayload, gridItem.loadPreInsertSelector);
                    });

                    $scope.scopeModel.datasource.push(gridItem);
                }

                return UtilsService.waitMultiplePromises(promises);
            };


            api.getData = function () {
                var dbReplicationTableDefinitions;

                if ($scope.scopeModel.datasource != undefined) {
                    dbReplicationTableDefinitions = [];

                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var currentItem = $scope.scopeModel.datasource[i];
                        var dbReplicationTableDefinition = {
                            TableName: currentItem.TableName,
                            TableSchema: currentItem.TableSchema,
                            FilterDateTimeColumn: currentItem.FilterDateTimeColumn,
                            IdColumn: currentItem.IdColumn,
                            ChunkSize: currentItem.ChunkSize,
                            DBReplicationPreInsert: currentItem.preInsertSelectorAPI.getData()
                        };

                        dbReplicationTableDefinitions.push(dbReplicationTableDefinition);
                    }
                }
                return dbReplicationTableDefinitions;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
    return directiveDefinitionObject;
}]);