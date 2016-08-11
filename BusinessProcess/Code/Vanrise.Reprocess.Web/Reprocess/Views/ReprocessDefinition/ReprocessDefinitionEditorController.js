(function (appControllers) {

    "use strict";

    ReprocessDefinitionEditorController.$inject = ['$scope', 'Reprocess_ReprocessDefinitionAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function ReprocessDefinitionEditorController($scope, Reprocess_ReprocessDefinitionAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var reprocessDefinitionId;
        var reprocessDefinitionEntity;

        var dataRecordStorageAPI;
        var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var onDataRecordStorageSelectionChangedDeferred;

        var executionFlowDefinitionAPI;
        var executionFlowDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var onExecutionFlowDefinitionSelectionChangedDeferred;

        var executionFlowStageAPI;
        var executionFlowStageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.onDataRecordStorageSelectionChanged = function () {
                var selectedRecordStorageId = dataRecordStorageAPI.getSelectedIds();
                if (selectedRecordStorageId != undefined) {
                    tryLoadFlowStagesOrResolvePromise(onDataRecordStorageSelectionChangedDeferred);
                }
            };

            $scope.scopeModel.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionAPI = api;
                executionFlowDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onExecutionFlowDefinitionSelectionChanged = function () {
                var selectedExecutionFlowDefinitionId = executionFlowDefinitionAPI.getSelectedIds();
                if (selectedExecutionFlowDefinitionId != undefined)
                {
                    tryLoadFlowStagesOrResolvePromise(onExecutionFlowDefinitionSelectionChangedDeferred);
                }
            };

            $scope.scopeModel.onExecutionFlowStageSelectorReady = function (api) {
                executionFlowStageAPI = api;
                executionFlowStageSelectorReadyDeferred.resolve();
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

            function tryLoadFlowStagesOrResolvePromise(promiseDeferred) {
                if (promiseDeferred != undefined)
                    promiseDeferred.resolve();
                else
                {
                    setTimeout(function () {
                        UtilsService.safeApply($scope, function () {

                            var selectedRecordStorage = $scope.scopeModel.selectedRecordStorage;
                            var selectedExecutionFlowDefinitionId = executionFlowDefinitionAPI.getSelectedIds();
                            if (selectedExecutionFlowDefinitionId == undefined || selectedRecordStorage == undefined)
                                return;
                            var payload = {
                                executionFlowDefinitionId: selectedExecutionFlowDefinitionId,
                                filter: {
                                    Filters: [buildStageRecordTypeFilter()]
                                }
                            }
                            var setExecutionFlowStagesLoader = function (value) {
                                $scope.scopeModel.isLoadingExecutionFlowStages = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, executionFlowStageAPI, payload, setExecutionFlowStagesLoader, promiseDeferred);
                        });                        
                    }, 1000);      
                 } 
            }
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordStorageSelector, loadExecutionFlowDefinitionSelector, loadExecutionFlowStageSelector]).catch(function (error) {
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
        function loadExecutionFlowDefinitionSelector() {
            var executionFlowDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            executionFlowDefinitionSelectorReadyDeferred.promise.then(function () {
                var executionFlowDefinitionSelectorPayload;
                if (isEditMode) {
                    executionFlowDefinitionSelectorPayload = {
                        selectedIds: reprocessDefinitionEntity.Settings.ExecutionFlowDefinitionId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(executionFlowDefinitionAPI, executionFlowDefinitionSelectorPayload, executionFlowDefinitionSelectorLoadDeferred);
            });
            return executionFlowDefinitionSelectorLoadDeferred.promise;
        }
        function loadExecutionFlowStageSelector() {
            if (reprocessDefinitionEntity == undefined || reprocessDefinitionEntity.Settings.ExecutionFlowDefinitionId == undefined)
                return;

            var executionFlowStageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            if (onExecutionFlowDefinitionSelectionChangedDeferred == undefined)
                onExecutionFlowDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
            if (onDataRecordStorageSelectionChangedDeferred == undefined)
                onDataRecordStorageSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([executionFlowStageSelectorReadyDeferred.promise, onExecutionFlowDefinitionSelectionChangedDeferred.promise, onDataRecordStorageSelectionChangedDeferred.promise])
                .then(function () {
                    setTimeout(function () {
                        UtilsService.safeApply($scope, function () {
                            onExecutionFlowDefinitionSelectionChangedDeferred = undefined;
                            onDataRecordStorageSelectionChangedDeferred = undefined;
                            var executionFlowStageSelectorPayload;
                            executionFlowStageSelectorPayload = {
                                executionFlowDefinitionId: reprocessDefinitionEntity.Settings.ExecutionFlowDefinitionId,
                                filter: { Filters: [buildStageRecordTypeFilter()] },
                                selectedIds: reprocessDefinitionEntity.Settings.InitiationStageNames
                            };
                            VRUIUtilsService.callDirectiveLoad(executionFlowStageAPI, executionFlowStageSelectorPayload, executionFlowStageSelectorLoadDeferred);
                        });
                    });
                   
                });

            return executionFlowStageSelectorLoadDeferred.promise;
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


        function buildStageRecordTypeFilter() {
            return {
                '$type': 'Vanrise.GenericData.QueueActivators.RecordTypeStageFilter, Vanrise.GenericData.QueueActivators',
                RecordTypeId: $scope.scopeModel.selectedRecordStorage.DataRecordTypeId
            };
        }
        function buildReprocessDefinitionObjFromScope() {

            var settings = {
                SourceRecordStorageId: dataRecordStorageAPI.getSelectedIds(),
                ExecutionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds(),
                InitiationStageNames: executionFlowStageAPI.getSelectedIds()
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