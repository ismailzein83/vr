(function (appControllers) {

    'use strict';

    DBReplicationDatabaseDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function DBReplicationDatabaseDefinitionEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var databaseDefinitionEntity;
        var isEditMode;
        
        var dbReplicationTableDefinitionGridAPI;
        var dbReplicationTableDefinitionGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                databaseDefinitionEntity = parameters.databaseDefinitionEntity;
            }

            isEditMode = (databaseDefinitionEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDBReplicationTableDefintionGridReady = function (api) {
                dbReplicationTableDefinitionGridAPI = api;
                dbReplicationTableDefinitionGridReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return isEditMode ? updateDatabaseDefinition() : addDatabaseDefinition();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && databaseDefinitionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(databaseDefinitionEntity.Name, 'Database Definition');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Database Definition');
            }

            function loadStaticData() {
                if (databaseDefinitionEntity != undefined) {
                    $scope.scopeModel.name = databaseDefinitionEntity.Name;
                }
            }

            function loadDBReplicationTableDefinitionGrid() {
                var dbReplicationTableDefinitionGridLoadDeferred = UtilsService.createPromiseDeferred();

                dbReplicationTableDefinitionGridReadyDeferred.promise.then(function () {

                    var dbReplicationTableDefinitionGridPayload;
                    if (databaseDefinitionEntity != undefined) {
                        dbReplicationTableDefinitionGridPayload = {
                            dbReplicationTablesDefinitions: databaseDefinitionEntity.Tables
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(dbReplicationTableDefinitionGridAPI, dbReplicationTableDefinitionGridPayload, dbReplicationTableDefinitionGridLoadDeferred);
                });

                return dbReplicationTableDefinitionGridLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDBReplicationTableDefinitionGrid]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function addDatabaseDefinition() {
            var databaseDefinitionObj = buildDatabaseDefinitionObjFromScope();
            if ($scope.onDBReplicationDatabaseDefinitionAdded != undefined) {
                $scope.onDBReplicationDatabaseDefinitionAdded(databaseDefinitionObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateDatabaseDefinition() {
            var databaseDefinitionObj = buildDatabaseDefinitionObjFromScope();
            if ($scope.onDBReplicationDatabaseDefinitionUpdated != undefined) {
                $scope.onDBReplicationDatabaseDefinitionUpdated(databaseDefinitionObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildDatabaseDefinitionObjFromScope() {
            return {
                Name: $scope.scopeModel.name,
                Tables: dbReplicationTableDefinitionGridAPI.getData()
            };
        }
    }

    appControllers.controller('vr-common-dbreplicationdatabasedefinition-editor', DBReplicationDatabaseDefinitionEditorController);
})(appControllers);