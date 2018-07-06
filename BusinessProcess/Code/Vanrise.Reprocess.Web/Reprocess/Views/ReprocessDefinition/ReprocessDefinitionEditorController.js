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

        var reprocessDefinitionSelectorAPI;
        var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var reprocessFilterDefinitionSettingsAPI;
        var reprocessFilterDefinitionSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var stageAPI;
        var stageSelectorReadyDeferred;

        var stagesToHoldAPI;
        var stagesToProcessAPI;

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
            $scope.scopeModel.recordCountPerTransaction = 50000;

            $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageAPI = api;
                dataRecordStorageSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onDataRecordStorageSelectionChanged = function () {
                tryLoadFlowStagesOrResolvePromise(onDataRecordStorageSelectionChangedDeferred);
            };

            $scope.scopeModel.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionAPI = api;
                executionFlowDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onExecutionFlowDefinitionSelectionChanged = function () {
                loadStagesSelector();
                loadStagesToHoldSelector();
                loadStagesToProcessSelector();
                tryLoadFlowStagesOrResolvePromise(onExecutionFlowDefinitionSelectionChangedDeferred);
            };

            $scope.scopeModel.onExecutionFlowStageSelectorReady = function (api) {
                executionFlowStageAPI = api;
                executionFlowStageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onReprocessDefinitionSelectorReady = function (api) {
                reprocessDefinitionSelectorAPI = api;
                reprocessDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onReprocessFilterDefinitionSettingsReady = function (api) {
                reprocessFilterDefinitionSettingsAPI = api;
                reprocessFilterDefinitionSettingsReadyDeferred.resolve();
            };

            $scope.scopeModel.onStageSelectorReady = function (api) {
                stageAPI = api;
            };
            $scope.scopeModel.onStageSelectorSelectionChanged = function () {
                tryLoadFlowStagesOrResolvePromise(stageSelectorReadyDeferred);
            };

            $scope.scopeModel.onStagesToProcessSelectorReady = function (api) {
                stagesToProcessAPI = api;
            };

            $scope.scopeModel.onStagesToHoldSelectorReady = function (api) {
                stagesToHoldAPI = api;
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
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.validateDataRecordStorageSelector = function () {

                var recordTypeIds = UtilsService.getPropValuesFromArray($scope.scopeModel.selectedRecordStorage, "DataRecordTypeId");
                if (recordTypeIds == undefined || recordTypeIds.length <= 1)
                    return null;

                var firstDataRecordTypeId = recordTypeIds[0];
                for (var index = 1; index < recordTypeIds.length; index++) {
                    var currentDataRecordTypeId = recordTypeIds[index];
                    if (firstDataRecordTypeId != currentDataRecordTypeId)
                        return "All Record Storages should be of the same Record Type";
                }

                return null;
            };

            function loadStagesSelector() {
                var payload = {
                    executionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds()
                };
                if (reprocessDefinitionEntity != undefined && reprocessDefinitionEntity.Settings != undefined)
                    payload.selectedIds = reprocessDefinitionEntity.Settings.StageNames;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSatge = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stageAPI, payload, setLoader);
            };

            function loadStagesToHoldSelector() {
                var stagesToHoldPayload = {
                    executionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds()
                };
                if (reprocessDefinitionEntity != undefined && reprocessDefinitionEntity.Settings != undefined)
                    stagesToHoldPayload.selectedIds = reprocessDefinitionEntity.Settings.StagesToHoldNames;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSatgesToHold = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stagesToHoldAPI, stagesToHoldPayload, setLoader);
            };

            function loadStagesToProcessSelector() {
                var stagesToProcessPayload = {
                    executionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds()
                };
                if (reprocessDefinitionEntity != undefined && reprocessDefinitionEntity.Settings != undefined)
                    stagesToProcessPayload.selectedIds = reprocessDefinitionEntity.Settings.StagesToProcessNames;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSatgesToProcess = value; };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stagesToProcessAPI, stagesToProcessPayload, setLoader);
            };

            function tryLoadFlowStagesOrResolvePromise(promiseDeferred) {
                if (promiseDeferred != undefined)
                    promiseDeferred.resolve();
                else {
                    setTimeout(function () {
                        UtilsService.safeApply($scope, function () {

                            var selectedRecordStorage = $scope.scopeModel.selectedRecordStorage;
                            var selectedExecutionFlowDefinitionId = executionFlowDefinitionAPI.getSelectedIds() != undefined ? executionFlowDefinitionAPI.getSelectedIds() : 0;
                            var stageNames = stageAPI.getSelectedIds() != undefined ? stageAPI.getSelectedIds() : [];

                            var payload = {
                                executionFlowDefinitionId: selectedExecutionFlowDefinitionId,
                                filter: {
                                    Filters: selectedRecordStorage != undefined ? [buildStageRecordTypeFilter()] : undefined,
                                    InculdesStageNames: stageNames
                                }
                            };
                            var setExecutionFlowStagesLoader = function (value) {
                                $scope.scopeModel.isLoadingExecutionFlowStages = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, executionFlowStageAPI, payload, setExecutionFlowStagesLoader, promiseDeferred);
                        });
                    });
                }
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getReprocessDefinition().then(function () {
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

        function getReprocessDefinition() {
            return Reprocess_ReprocessDefinitionAPIService.GetReprocessDefinition(reprocessDefinitionId).then(function (response) {
                reprocessDefinitionEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode) {
                    var reprocessDefinitionName = (reprocessDefinitionEntity != undefined) ? reprocessDefinitionEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(reprocessDefinitionName, 'Reprocess Definition');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Reprocess Definition');
                }
            }

            function loadStaticData() {
                if (reprocessDefinitionEntity == undefined)
                    return;

                $scope.scopeModel.name = reprocessDefinitionEntity.Name;

                var reprocessDefinitionSettings = reprocessDefinitionEntity.Settings;
                if (reprocessDefinitionSettings != undefined) {
                    $scope.scopeModel.forceUseTempStorage = reprocessDefinitionSettings.ForceUseTempStorage;
                    $scope.scopeModel.cannotBeTriggeredManually = reprocessDefinitionSettings.CannotBeTriggeredManually;
                    if (reprocessDefinitionSettings.RecordCountPerTransaction > 0) {
                        $scope.scopeModel.recordCountPerTransaction = reprocessDefinitionSettings.RecordCountPerTransaction;
                    }
                }
            }

            function loadDataRecordStorageSelector() {
                var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordStorageSelectorReadyDeferred.promise.then(function () {
                    var dataRecordStorageSelectorPayload;
                    if (isEditMode) {
                        dataRecordStorageSelectorPayload = {
                            selectedIds: reprocessDefinitionEntity.Settings.SourceRecordStorageIds
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

                if (stageSelectorReadyDeferred == undefined)
                    stageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([executionFlowStageSelectorReadyDeferred.promise, onExecutionFlowDefinitionSelectionChangedDeferred.promise, onDataRecordStorageSelectionChangedDeferred.promise, stageSelectorReadyDeferred.promise])
                    .then(function () {
                        setTimeout(function () {
                            UtilsService.safeApply($scope, function () {
                                onExecutionFlowDefinitionSelectionChangedDeferred = undefined;
                                onDataRecordStorageSelectionChangedDeferred = undefined;
                                stageSelectorReadyDeferred = undefined;

                                var executionFlowStageSelectorPayload = {
                                    executionFlowDefinitionId: reprocessDefinitionEntity.Settings.ExecutionFlowDefinitionId,
                                    filter: {
                                        Filters: [buildStageRecordTypeFilter()],
                                        InculdesStageNames: reprocessDefinitionEntity.Settings.StageNames
                                    },
                                    selectedIds: reprocessDefinitionEntity.Settings.InitiationStageNames
                                };
                                VRUIUtilsService.callDirectiveLoad(executionFlowStageAPI, executionFlowStageSelectorPayload, executionFlowStageSelectorLoadDeferred);
                            });
                        }, 1000);
                    });

                return executionFlowStageSelectorLoadDeferred.promise;
            }

            function loadReprocessDefinitionSelector() {
                var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                reprocessDefinitionSelectorReadyDeferred.promise.then(function () {
                    var reprocessDefinitionSelectorPayload;
                    if (isEditMode) {
                        var selectedIds;
                        if (reprocessDefinitionEntity != undefined && reprocessDefinitionEntity.Settings != undefined && reprocessDefinitionEntity.Settings.PostExecution != undefined && reprocessDefinitionEntity.Settings.PostExecution.ReprocessDefinitionIds != undefined) {
                            selectedIds = reprocessDefinitionEntity.Settings.PostExecution.ReprocessDefinitionIds;
                        }

                        reprocessDefinitionSelectorPayload = {
                            filter: { ExcludedReprocessDefinitionIds: [reprocessDefinitionId] },
                            selectedIds: selectedIds
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, reprocessDefinitionSelectorPayload, reprocessDefinitionSelectorLoadDeferred);
                });

                return reprocessDefinitionSelectorLoadDeferred.promise;
            }

            function loadReprocessFilterDefinitionSettings() {
                var reprocessFilterDefinitionSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                reprocessFilterDefinitionSettingsReadyDeferred.promise.then(function () {
                    var reprocessFilterDefinitionSettingsPayload;
                    if (isEditMode) {
                        var selectedIds;
                        if (reprocessDefinitionEntity != undefined && reprocessDefinitionEntity.Settings != undefined && reprocessDefinitionEntity.Settings.FilterDefinition != undefined) {
                            reprocessFilterDefinitionSettingsPayload = { filterDefinition: reprocessDefinitionEntity.Settings.FilterDefinition };
                        }
                    }

                    VRUIUtilsService.callDirectiveLoad(reprocessFilterDefinitionSettingsAPI, reprocessFilterDefinitionSettingsPayload, reprocessFilterDefinitionSettingsLoadDeferred);
                });

                return reprocessFilterDefinitionSettingsLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordStorageSelector, loadExecutionFlowDefinitionSelector, loadExecutionFlowStageSelector, loadReprocessDefinitionSelector, loadReprocessFilterDefinitionSettings]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                    reprocessDefinitionEntity = undefined;
                });
        };

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
        };

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
        };

        function buildStageRecordTypeFilter() {

            var recordTypeIds = UtilsService.getPropValuesFromArray($scope.scopeModel.selectedRecordStorage, "DataRecordTypeId");

            return {
                '$type': 'Vanrise.GenericData.QueueActivators.RecordTypeStageFilter, Vanrise.GenericData.QueueActivators',
                RecordTypeId: recordTypeIds != undefined ? recordTypeIds[0] : undefined
            };
        };

        function buildReprocessDefinitionObjFromScope() {

            var settings = {
                SourceRecordStorageIds: dataRecordStorageAPI.getSelectedIds(),
                ExecutionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds(),
                InitiationStageNames: executionFlowStageAPI.getSelectedIds(),
                StageNames: stageAPI.getSelectedIds(),
                StagesToHoldNames: stagesToHoldAPI.getSelectedIds(),
                StagesToProcessNames: stagesToProcessAPI.getSelectedIds(),
                RecordCountPerTransaction: $scope.scopeModel.recordCountPerTransaction,
                ForceUseTempStorage: $scope.scopeModel.forceUseTempStorage,
                CannotBeTriggeredManually: $scope.scopeModel.cannotBeTriggeredManually,
                PostExecution: { ReprocessDefinitionIds: reprocessDefinitionSelectorAPI.getSelectedIds() },
                FilterDefinition: reprocessFilterDefinitionSettingsAPI != undefined ? reprocessFilterDefinitionSettingsAPI.getData() : undefined
            };

            return {
                ReprocessDefinitionId: reprocessDefinitionId != undefined ? reprocessDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings
            };
        };
    }

    appControllers.controller('Reprocess_ReprocessDefinitionEditorController', ReprocessDefinitionEditorController);

})(appControllers);