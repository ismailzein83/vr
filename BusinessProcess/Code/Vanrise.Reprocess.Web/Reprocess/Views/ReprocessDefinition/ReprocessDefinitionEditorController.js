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

        var stageAPI;
        var stageSelectorReadyDeferred;//= UtilsService.createPromiseDeferred();

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
                 tryLoadFlowStagesOrResolvePromise(onDataRecordStorageSelectionChangedDeferred);
            };

            $scope.scopeModel.onExecutionFlowDefinitionSelectorReady = function (api) {
                executionFlowDefinitionAPI = api;
                executionFlowDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onExecutionFlowDefinitionSelectionChanged = function () {
             
                loadStagesSelector();
                tryLoadFlowStagesOrResolvePromise(onExecutionFlowDefinitionSelectionChangedDeferred);
         
            };

            $scope.scopeModel.onStageSelectorSelectionChanged = function () {
                        
                 tryLoadFlowStagesOrResolvePromise(stageSelectorReadyDeferred);
            };

            $scope.scopeModel.onExecutionFlowStageSelectorReady = function (api) {
                executionFlowStageAPI = api;
                executionFlowStageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onStageSelectorReady = function (api) {
                stageAPI = api;
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

            function loadStagesSelector() {
                var payload = {
                    executionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds()
                };
                if (reprocessDefinitionEntity != undefined && reprocessDefinitionEntity.Settings != undefined)
                    payload.selectedIds = reprocessDefinitionEntity.Settings.StageNames;
                var setLoader = function (value) { $scope.scopeModel.isLoadingSatge = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, stageAPI, payload, setLoader);

            }
            function tryLoadFlowStagesOrResolvePromise(promiseDeferred) {
                if (promiseDeferred != undefined)
                    promiseDeferred.resolve();
                else
                {
                    setTimeout(function () {
                        UtilsService.safeApply($scope, function () {

                            var selectedRecordStorage = $scope.scopeModel.selectedRecordStorage;
                            var selectedExecutionFlowDefinitionId = executionFlowDefinitionAPI.getSelectedIds()!=undefined ?executionFlowDefinitionAPI.getSelectedIds():0;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDataRecordStorageSelector, loadExecutionFlowDefinitionSelector, loadExecutionFlowStageSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
                reprocessDefinitionEntity = undefined
            });

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

                if (stageSelectorReadyDeferred == undefined)
                    stageSelectorReadyDeferred = UtilsService.createPromiseDeferred();
                
                UtilsService.waitMultiplePromises([executionFlowStageSelectorReadyDeferred.promise, onExecutionFlowDefinitionSelectionChangedDeferred.promise, onDataRecordStorageSelectionChangedDeferred.promise, stageSelectorReadyDeferred.promise])
                    .then(function () {
                        setTimeout(function () {
                            UtilsService.safeApply($scope, function () {
                                onExecutionFlowDefinitionSelectionChangedDeferred = undefined;
                                onDataRecordStorageSelectionChangedDeferred = undefined;
                                stageSelectorReadyDeferred = undefined;
                                var executionFlowStageSelectorPayload;
                                executionFlowStageSelectorPayload = {
                                    executionFlowDefinitionId: reprocessDefinitionEntity.Settings.ExecutionFlowDefinitionId,
                                    filter: {
                                        Filters: [buildStageRecordTypeFilter()],
                                        InculdesStageNames:  reprocessDefinitionEntity.Settings.StageNames
                                    },
                                    selectedIds: reprocessDefinitionEntity.Settings.InitiationStageNames
                                };
                                VRUIUtilsService.callDirectiveLoad(executionFlowStageAPI, executionFlowStageSelectorPayload, executionFlowStageSelectorLoadDeferred);
                            });
                        },1000);

                    });

                return executionFlowStageSelectorLoadDeferred.promise;
            }
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
                RecordTypeId:$scope.scopeModel.selectedRecordStorage.DataRecordTypeId 
            };
        }
        function buildReprocessDefinitionObjFromScope() {

            var settings = {
                SourceRecordStorageId: dataRecordStorageAPI.getSelectedIds(),
                ExecutionFlowDefinitionId: executionFlowDefinitionAPI.getSelectedIds(),
                InitiationStageNames: executionFlowStageAPI.getSelectedIds(),
                StageNames: stageAPI.getSelectedIds()
            };

            return {
                ReprocessDefinitionId: reprocessDefinitionId != undefined ? reprocessDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings
            };
        }
    }

    appControllers.controller('Reprocess_ReprocessDefinitionEditorController', ReprocessDefinitionEditorController);

})(appControllers);