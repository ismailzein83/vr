(function (app) {

    'use strict';

    DbReplicationDefinitionSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VRCommon_DBReplicationService'];

    function DbReplicationDefinitionSettings(UtilsService, VRUIUtilsService, VRCommon_DBReplicationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DbReplicationDefinitionSettingsCtor(ctrl, $scope, $attrs);
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
            templateUrl: '/Client/Modules/Common/Directives/DBReplication/DBReplicationDefinition/Templates/DBReplicationDefinitionSettingsTemplate.html'
        };

        function DbReplicationDefinitionSettingsCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var databaseDefinitions;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.addDBReplicationDatabaseDefinition = function () {
                    var onDBReplicationDatabaseDefinitionAdded = function (addedDatabaseDefinition) {
                        var addedObject = {
                            ID: UtilsService.guid(),
                            Name: addedDatabaseDefinition.Name,
                            Tables: addedDatabaseDefinition.Tables
                        };
                        $scope.scopeModel.datasource.push(addedObject);
                    };

                    VRCommon_DBReplicationService.addDBReplicationDatabaseDefinition(onDBReplicationDatabaseDefinitionAdded);
                };

                $scope.scopeModel.removeDBReplicationDatabaseDefinition = function (dataItem) {
                    var index = $scope.scopeModel.datasource.indexOf(dataItem);
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                $scope.scopeModel.isGridValid = function () {
                    if ($scope.scopeModel.datasource.length == 0) {
                        return 'At least one item should be added';
                    }
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var currentItem = $scope.scopeModel.datasource[i];
                        var currentItemDBDefinitionName = currentItem.Name.toLowerCase();

                        for (var j = i + 1; j < $scope.scopeModel.datasource.length; j++) {
                            var dataItem = $scope.scopeModel.datasource[j];
                            var dataItemDBDefinitionName = dataItem.Name.toLowerCase();

                            if (currentItemDBDefinitionName == dataItemDBDefinitionName) {
                                return 'Same Database Definition Name is added more than once';
                            }
                        }
                    }
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.componentType != undefined) {
                        $scope.scopeModel.name = payload.componentType.Name;
                        if (payload.componentType.Settings != undefined) {
                            databaseDefinitions = payload.componentType.Settings.DatabaseDefinitions;

                            angular.forEach(databaseDefinitions, function (val, key) {
                                if (val != "$type" && val.Tables != undefined) {
                                    $scope.scopeModel.datasource.push({
                                        ID: key,
                                        Name: val.Name,
                                        Tables: val.Tables
                                    });
                                }
                            });
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var databaseDefinitions;
                    if ($scope.scopeModel.datasource != undefined) {
                        databaseDefinitions = {};
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentItem = $scope.scopeModel.datasource[i];
                            buildDatabaseDefinitionObj(databaseDefinitions, currentItem);
                        }
                    }
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.Entities.DBReplicationDefinitionSettings, Vanrise.Entities",
                            DatabaseDefinitions: databaseDefinitions
                        }
                    };
                };

                function buildDatabaseDefinitionObj(databaseDefinitions, currentItem) {
                    databaseDefinitions[currentItem.ID] = {
                        Name: currentItem.Name,
                        Tables: currentItem.Tables
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }

            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editDBReplicationDatabaseDefinition
                }];

                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editDBReplicationDatabaseDefinition(dbDefinitionSettingObj) {
                var onDBReplicationDatabaseDefinitionUpdated = function (updatedSetting) {
                    var index = $scope.scopeModel.datasource.indexOf(dbDefinitionSettingObj);
                    updatedSetting.ID = dbDefinitionSettingObj.ID;
                    $scope.scopeModel.datasource[index] = updatedSetting;
                };

                VRCommon_DBReplicationService.editDBReplicationDatabaseDefinition(onDBReplicationDatabaseDefinitionUpdated, dbDefinitionSettingObj);
            }

        }
        return directiveDefinitionObject;
    }
    app.directive('vrCommonDbreplicationdefinitionSettings', DbReplicationDefinitionSettings);

})(app);
