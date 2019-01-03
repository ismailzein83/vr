﻿'use strict';

app.directive('vrCommonDbreplicationSettingsGrid', ['UtilsService', 'VRUIUtilsService', 'VRCommon_DBReplicationService', 'VRCommon_VRComponentTypeAPIService', 'VRCommon_VRConnectionAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_DBReplicationService, VRCommon_VRComponentTypeAPIService, VRCommon_VRConnectionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DBReplicationSettingsGridDirectiveCtor(ctrl, $scope, $attrs);
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
            templateUrl: "/Client/Modules/Common/Directives/DBReplication/DBReplicationSettings/Templates/DBReplicationSettingsGridTemplate.html"
        };

        function DBReplicationSettingsGridDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var dbReplicationDefinitionId;
            var dbDefinitions;
            var dbTargetConnectionInfos = [];
            var selectedDbDefinitions = [];

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.disableAdd = false;

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0) {
                        $scope.scopeModel.disableAdd = false;
                        return 'At least one item should exist';
                    }

                    var selectedDbDefintionsLength = 0;
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var currentDbConnection = $scope.scopeModel.datasource[i];
                        if (currentDbConnection.DbDefinitions != undefined)
                            selectedDbDefintionsLength += currentDbConnection.DbDefinitions.length;
                    }

                    if (dbDefinitions != undefined && dbDefinitions.length != selectedDbDefintionsLength) {
                        $scope.scopeModel.disableAdd = false;
                        return 'All Database Definitions should be selected';
                    }

                    $scope.scopeModel.disableAdd = true;
                    return null;
                };

                $scope.scopeModel.addDBReplicationSetting = function () {
                    var onDBReplicationSettingAdded = function (addedSetting) {

                        if (addedSetting != undefined && addedSetting.Settings != undefined) {
                            addedSetting.DbDefinitions = [];

                            for (var i = 0; i < addedSetting.Settings.length; i++) {
                                if (dbDefinitions != undefined) {
                                    var selectedDbDefinition = UtilsService.getItemByVal(dbDefinitions, addedSetting.Settings[i].DatabaseDefinitionId, 'DBDefinitionId');
                                    if (selectedDbDefinition != undefined) {
                                        addedSetting.DbDefinitions.push({ DBDefinitionId: addedSetting.Settings[i].DatabaseDefinitionId, name: selectedDbDefinition.Name });
                                    }
                                }
                            }
                        }
                        addedSetting.id = UtilsService.guid();
                        $scope.scopeModel.datasource.push(addedSetting);
                    };

                    selectedDbDefinitions = [];
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var entity = $scope.scopeModel.datasource[i];
                        for (var j = 0; j < entity.DbDefinitions.length; j++) {
                            selectedDbDefinitions.push(entity.DbDefinitions[j].DBDefinitionId);
                        }
                    }

                    VRCommon_DBReplicationService.addDBReplicationSetting(onDBReplicationSettingAdded, dbReplicationDefinitionId, selectedDbDefinitions);
                };

                $scope.scopeModel.removeDBReplicationSetting = function (dataItem) {
                    var index = $scope.scopeModel.datasource.indexOf(dataItem);
                    for (var i = 0; i < dataItem.DbDefinitions.length; i++) {
                        selectedDbDefinitions.pop(dataItem.DbDefinitions[i].DatabaseDefinitionId);
                    }
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.datasource = [];

                    var dbReplicationSettings;

                    if (payload != undefined) {
                        dbReplicationDefinitionId = payload.dbReplicationDefinitionId;
                        dbDefinitions = payload.dbDefinitions;
                        dbReplicationSettings = payload.dbReplicationSettings;
                    }

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    getConnectionInfos().then(function () {
                        var promises = [];
                        if (dbReplicationSettings != undefined) {
                            var dbConnections = dbReplicationSettings.DBConnections;
                            if (dbConnections != undefined) {
                                for (var i = 0; i < dbConnections.length; i++) {
                                    var currentDBConnection = dbConnections[i];
                                    var addItemPromise = addItem(currentDBConnection);
                                    promises.push(addItemPromise);
                                }
                            }
                        }
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            directiveLoadDeferred.resolve();
                        });
                    });

                    function getConnectionInfos() {
                        var filter = { Filters: [{ $type: "Vanrise.Common.Business.SQLConnectionFilter ,Vanrise.Common.Business" }] };

                        return VRCommon_VRConnectionAPIService.GetVRConnectionInfos(UtilsService.serializetoJson(filter)).then(function (response) {
                            if (response != null) {
                                dbTargetConnectionInfos = response;
                            }
                        });
                    }

                    function addItem(currentDBConnection) {
                        var loadReadyDeferred = UtilsService.createPromiseDeferred();
                        setTimeout(function () {
                            var vrConnection = UtilsService.getItemByVal(dbTargetConnectionInfos, currentDBConnection.TargetConnectionId, 'VRConnectionId');
                            if (currentDBConnection != undefined && currentDBConnection.Settings != undefined) {
                                var newItem = {
                                    id: UtilsService.guid(),
                                    SourceConnectionStringName: currentDBConnection.SourceConnectionStringName,
                                    TargetConnectionStringName: vrConnection.Name,
                                    TargetConnectionId: currentDBConnection.TargetConnectionId,
                                    Settings: [],
                                    DbDefinitions: []
                                };

                                for (var i = 0; i < currentDBConnection.Settings.length; i++) {
                                    if (dbDefinitions != undefined) {
                                        var selectedDbDefinition = UtilsService.getItemByVal(dbDefinitions, currentDBConnection.Settings[i].DatabaseDefinitionId, 'DBDefinitionId');

                                        if (selectedDbDefinition != undefined) {
                                            newItem.Settings.push({ DatabaseDefinitionId: currentDBConnection.Settings[i].DatabaseDefinitionId });
                                            newItem.DbDefinitions.push({ DBDefinitionId: currentDBConnection.Settings[i].DatabaseDefinitionId, name: selectedDbDefinition.Name });
                                        }
                                    }
                                }

                                $scope.scopeModel.datasource.push(newItem);
                            }
                            loadReadyDeferred.resolve();
                        });
                        return loadReadyDeferred.promise;
                    }

                    return directiveLoadDeferred.promise;

                };

                api.getData = function () {
                    var dbReplicationSettings;
                    if ($scope.scopeModel.datasource != undefined) {
                        dbReplicationSettings = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            dbReplicationSettings.push($scope.scopeModel.datasource[i]);
                        }
                    }
                    return { DBConnections: dbReplicationSettings };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editDBReplicationSetting
                }];

                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editDBReplicationSetting(settingObj) {
                var onDBReplicationSettingUpdated = function (updatedSetting) {

                    if (updatedSetting != undefined && updatedSetting.Settings != undefined) {
                        updatedSetting.DbDefinitions = [];

                        for (var i = 0; i < updatedSetting.Settings.length; i++) {
                            if (dbDefinitions != undefined) {
                                var selectedDbDefinition = UtilsService.getItemByVal(dbDefinitions, updatedSetting.Settings[i].DatabaseDefinitionId, 'DBDefinitionId');
                                if (selectedDbDefinition != undefined) {
                                    updatedSetting.DbDefinitions.push({ DBDefinitionId: updatedSetting.Settings[i].DatabaseDefinitionId, name: selectedDbDefinition.Name });
                                }
                            }
                        }
                    }

                    var index = $scope.scopeModel.datasource.indexOf(settingObj);
                    $scope.scopeModel.datasource[index] = updatedSetting;
                };

                selectedDbDefinitions = [];
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var entity = $scope.scopeModel.datasource[i];
                    for (var j = 0; j < entity.DbDefinitions.length; j++) {
                        var currentSetting = entity.DbDefinitions[j];
                        selectedDbDefinitions.push(currentSetting.DBDefinitionId);
                    }
                }
                VRCommon_DBReplicationService.editDBReplicationSetting(onDBReplicationSettingUpdated, settingObj, dbReplicationDefinitionId, selectedDbDefinitions);
            }
        }

        return directiveDefinitionObject;
    }]);