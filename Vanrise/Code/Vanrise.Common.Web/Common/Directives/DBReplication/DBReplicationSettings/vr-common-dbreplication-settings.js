(function (appControllers) {

    'use strict';

    DbReplicationSettingsEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VRCommon_DBReplicationDefinitionAPIService'];
    function DbReplicationSettingsEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VRCommon_DBReplicationDefinitionAPIService) {

        var isEditMode;
        var dbReplicationSettingEntity;
        var dbReplicationDefinitionId;
        var selectedDBDefinitions;
        var filter;

        var dbDefinitionSelectorAPI;
        var dbDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                dbReplicationSettingEntity = parameters.dbReplicationSettingEntity;
                dbReplicationDefinitionId = parameters.dbReplicationDefinitionId;
                selectedDBDefinitions = parameters.selectedDBDefinitions;
            }

            isEditMode = (dbReplicationSettingEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDBDefinitionSelectorReady = function (api) {
                dbDefinitionSelectorAPI = api;
                dbDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return isEditMode ? updateDBReplicationSettings() : addDBReplicationSettings();
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
                if (isEditMode && dbReplicationSettingEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor('', 'DB Replication Settings');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('DB Replication Settings');
            }

            function loadStaticData() {
                if (dbReplicationSettingEntity != undefined) {
                    $scope.scopeModel.sourceConnectionStringName = dbReplicationSettingEntity.SourceConnectionStringName;
                    $scope.scopeModel.targetConnectionString = dbReplicationSettingEntity.TargetConnectionString;
                }
            }

            function loadDBDefinitionSelector() {

                var dbDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dbDefinitionSelectorReadyDeferred.promise.then(function () {

                    var dbDefinitionSelectorPayload = {
                        dbReplicationDefinitionId: dbReplicationDefinitionId,
                        filter: { ExcludedDBDefinitionIds: selectedDBDefinitions }
                    };

                    if (dbReplicationSettingEntity != undefined) {
                        var currentSelectedDBDefinitions = [];
                        for (var i = 0; i < dbReplicationSettingEntity.DbDefinitions.length; i++) {
                            var currentDBDefinition = dbReplicationSettingEntity.DbDefinitions[i];
                            currentSelectedDBDefinitions.push(currentDBDefinition.DBDefinitionId);
                        }

                        dbDefinitionSelectorPayload.filter.ForceDBDefinitionIds = currentSelectedDBDefinitions;
                        dbDefinitionSelectorPayload.selectedIds = currentSelectedDBDefinitions;
                    }

                    VRUIUtilsService.callDirectiveLoad(dbDefinitionSelectorAPI, dbDefinitionSelectorPayload, dbDefinitionSelectorLoadDeferred);
                });

                return dbDefinitionSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDBDefinitionSelector]).then(function () {

            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });

        }

        function addDBReplicationSettings() {
            var dbReplicationSettingObj = buildDBReplicationSettingObjFromScope();
            if ($scope.onDBReplicationSettingAdded != undefined) {
                $scope.onDBReplicationSettingAdded(dbReplicationSettingObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateDBReplicationSettings() {
            var dbReplicationSettingObj = buildDBReplicationSettingObjFromScope();
            if ($scope.onDBReplicationSettingUpdated != undefined) {
                $scope.onDBReplicationSettingUpdated(dbReplicationSettingObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildDBReplicationSettingObjFromScope() {
            return {
                SourceConnectionStringName: $scope.scopeModel.sourceConnectionStringName,
                TargetConnectionString: $scope.scopeModel.targetConnectionString,
                Settings: dbDefinitionSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_Common_DbReplicationSettingsEditorController', DbReplicationSettingsEditorController);
})(appControllers);