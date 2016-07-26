(function (appControllers) {

    "use strict";

    ReprocessDefinitionEditorController.$inject = ['$scope', 'Reprocess_ReprocessDefinitionAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function ReprocessDefinitionEditorController($scope, Reprocess_ReprocessDefinitionAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var reprocessDefinitionId;
        var reprocessDefinitionEntity;

        var dataRecordStorageAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var executionFlowAPI;
        var executionFlowSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                reprocessDefinitionId = parameters.reprocessDefinitionId;
            }

            isEditMode = (reprocessDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onExecutionFlowSelectorReady = function (api) {
                executionFlowAPI = api;
                executionFlowSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                GetReprocessDefinition().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }


        function GetReprocessDefinition() {
            return Reprocess_ReprocessDefinitionAPIService.GetReprocessDefinition(reprocessDefinitionId).then(function (response) {
                reprocessDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordStorageSelector, loadExecutionFlowSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var reprocessDefinitionName = (reprocessDefinitionEntity != undefined) ? reprocessDefinitionEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(reprocessDefinitionName, 'ReprocessDefinition');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('ReprocessDefinition');
            }
        }
        function loadStaticData() {
            if (reprocessDefinitionEntity == undefined)
                return;
            $scope.scopeModel.name = reprocessDefinitionEntity.Name;
        }
        function loadDataRecordStorageSelector() {
            var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            dataRecordStorageSelectorReadyDeferred.promise.then(function () {
                var dataRecordStorageSelectorPayload;
                if (isEditMode) {
                    dataRecordStorageSelectorPayload = {
                        selectedIds: reprocessDefinitionEntity.Settings.SourceRecordStorageId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(dataRecordStorageAPI, dataRecordStorageSelectorPayload, dataRecordStorageSelectorLoadDeferred);
            });
            return dataRecordStorageSelectorLoadDeferred.promise;
        }
        function loadExecutionFlowSelector() {
            var executionFlowSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            executionFlowSelectorReadyDeferred.promise.then(function () {
                var executionFlowSelectorPayload;
                if (isEditMode) {
                    executionFlowSelectorPayload = {
                        selectedIds: reprocessDefinitionEntity.Settings.ExecutionFlowDefinitionId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(executionFlowAPI, executionFlowSelectorPayload, executionFlowSelectorLoadDeferred);
            });
            return executionFlowSelectorLoadDeferred.promise;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return Reprocess_ReprocessDefinitionAPIService.AddReprocessDefinition(buildReprocessDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('ReprocessDefinition', response, 'Name')) {
                    if ($scope.onReprocessDefinitionAdded != undefined)
                        $scope.onReprocessDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return Reprocess_ReprocessDefinitionAPIService.UpdateReprocessDefinition(buildReprocessDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('ReprocessDefinition', response, 'Name')) {
                    if ($scope.onReprocessDefinitionUpdated != undefined) {
                        $scope.onReprocessDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildReprocessDefinitionObjFromScope() {

            var settings = {
                SourceRecordStorageId: dataRecordStorageAPI.getSelectedIds(),
                ExecutionFlowDefinitionId: executionFlowAPI.getSelectedIds()
            }

            return {
                ReprocessDefinitionId: reprocessDefinitionEntity != undefined ? reprocessDefinitionEntity.ReprocessDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings
            };
        }
    }

    appControllers.controller('Reprocess_ReprocessDefinitionEditorController', ReprocessDefinitionEditorController);

})(appControllers);