(function (appControllers) {
    "use strict";
    newDataSourceEditorController.$inject = ['$scope', 'VR_Integration_DataSourceAPIService', 'SchedulerTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function newDataSourceEditorController($scope, VR_Integration_DataSourceAPIService, SchedulerTaskAPIService, UtilsService, VRUIUtilsService, VRModalService, VRNotificationService, VRNavigationService) {
    var dataSourceTask;
    var isEditMode;

    var dataSourceEntity;
    var dataSourceId;
    var taskId = 0;

    var taskTriggerDirectiveAPI;
    var taskTriggerDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var adapterTypeDirectiveAPI;
    var adapterTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    loadParameters();
    defineScope();
    load();
   

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        dataSourceId = undefined;

        if (parameters != undefined && parameters != null) {
            dataSourceId = parameters.dataSourceId;
        }

        isEditMode = (dataSourceId != undefined);
    }

    function defineScope() {
        $scope.scopeModel = {};
        $scope.scopeModel.SaveDataSource = function () {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                return updateDataSource();
            }
            else {
                return insertDataSource();
            }
        };

        $scope.hasSaveDataSourcePermession = function () {
            if (isEditMode) {
                return VR_Integration_DataSourceAPIService.HasUpdateDataSource();
            }
            else {
                return VR_Integration_DataSourceAPIService.HasAddDataSource();
            }
        };

        $scope.scopeModel.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.scopeModel.onTaskTriggerDirectiveReady = function (api) {
            taskTriggerDirectiveAPI = api;
            taskTriggerDirectiveReadyPromiseDeferred.resolve();
        }

        $scope.scopeModel.onAdapterTypeDirectiveReady = function (api) {
            adapterTypeDirectiveAPI = api;
            var setLoader = function (value) { $scope.scopeModel.isLoadingAdapterDirective = value };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, adapterTypeDirectiveAPI, undefined, setLoader, adapterTypeDirectiveReadyPromiseDeferred);
        }

        $scope.scopeModel.addExecutionFlow = function () {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Add an Execution Flow";

                modalScope.onExecutionFlowAdded = function () {
                    // update the execution flows
                    $scope.scopeModel.isLoading = true;

                    // clear the selection menu
                    $scope.scopeModel.executionFlows = [];

                    loadExecutionFlows()
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        })
                        .finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                };
            };

            VRModalService.showModal('/Client/Modules/Integration/Views/DataSourceExecutionFlowEditor.html', null, modalSettings);
        }

        $scope.scopeModel.adapterTypes = [];
        $scope.scopeModel.executionFlows = [];
        $scope.scopeModel.dataSourceAdapter = {};
        $scope.scopeModel.dataSourceAdapter.argument = {};
        $scope.scopeModel.dataSourceAdapter.adapterState = {};
        $scope.scopeModel.schedulerTaskTrigger = {};
        $scope.scopeModel.startEffDate = new Date();
    }

    function load() {
        $scope.scopeModel.isLoading = true;

        if (isEditMode) {
            getDataSource().then(function () {
                getDataSourceTask().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }
        else {
            loadAllControls();
        }
    }

    function getDataSource() {
        return VR_Integration_DataSourceAPIService.GetDataSource(dataSourceId).then(function (dataSourceResponse) {
            dataSourceEntity = dataSourceResponse;
        });
    }
   
    function getDataSourceTask() {
        return SchedulerTaskAPIService.GetTask(dataSourceEntity.Entity.TaskId)
               .then(function (taskResponse) {
                   dataSourceTask = { DataSourceData: dataSourceEntity.Entity, TaskData: taskResponse };
               });
              
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadAdapterType, loadExecutionFlows, loadTaskTrigger, setTitle, loadStaticData]).then(function () {
            dataSourceTask = undefined;
            dataSourceEntity = undefined;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.scopeModel.isLoading = false;
        })
        .finally(function () {
            $scope.scopeModel.isLoading = false;
       });
    }

    function loadAdapters() {
        return VR_Integration_DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.scopeModel.adapterTypes.push(item);
            });
            if (dataSourceTask != undefined && dataSourceEntity != undefined && dataSourceEntity.Entity.AdapterTypeId != undefined)
                $scope.scopeModel.selectedAdapterType = UtilsService.getItemByVal($scope.scopeModel.adapterTypes, dataSourceEntity.Entity.AdapterTypeId, "ExtensionConfigurationId");

        });
    }

    function loadExecutionFlows() {
        return VR_Integration_DataSourceAPIService.GetExecutionFlows().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.scopeModel.executionFlows.push(item);
            });
            if (dataSourceTask != undefined && dataSourceEntity != undefined && dataSourceEntity.Entity != undefined && dataSourceEntity.Entity.Settings != undefined && dataSourceEntity.Entity.Settings.ExecutionFlowId != undefined)
                $scope.scopeModel.selectedExecutionFlow = UtilsService.getItemByVal($scope.scopeModel.executionFlows, dataSourceEntity.Entity.Settings.ExecutionFlowId, "ExecutionFlowId");

        });
    }

    function loadAdapterType() {
        var promises = [];        
        var dataSourcePromiseLoad = VR_Integration_DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.scopeModel.adapterTypes.push(item);
            });
            if (dataSourceTask != undefined && dataSourceEntity != undefined && dataSourceEntity.Entity != undefined && dataSourceEntity.Entity.AdapterTypeId != undefined)
                $scope.scopeModel.selectedAdapterType = UtilsService.getItemByVal($scope.scopeModel.adapterTypes, dataSourceEntity.Entity.AdapterTypeId, "ExtensionConfigurationId");
            else {
                $scope.scopeModel.selectedAdapterType = UtilsService.getItemByVal($scope.scopeModel.adapterTypes, "432aafd8-62c3-4d2b-99a8-9967d198337f", "ExtensionConfigurationId");
            }
        });

        promises.push(dataSourcePromiseLoad);

        var adapterPayload = {};
        if (dataSourceTask != undefined && dataSourceEntity != undefined && dataSourceEntity.Entity != undefined && dataSourceEntity.Entity.Settings != undefined && dataSourceEntity.Entity.Settings.AdapterArgument != undefined)
            adapterPayload.adapterArgument = dataSourceEntity.Entity.Settings.AdapterArgument;
        if (dataSourceTask != undefined && dataSourceEntity != undefined && dataSourceEntity.Entity != undefined && dataSourceEntity.Entity.AdapterState != undefined && adapterPayload != undefined)
            adapterPayload.adapterState = dataSourceEntity.Entity.AdapterState;


        var loadAdapterTypePromiseDeferred = UtilsService.createPromiseDeferred();
        promises.push(loadAdapterTypePromiseDeferred.promise);
        adapterTypeDirectiveReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(adapterTypeDirectiveAPI, adapterPayload, loadAdapterTypePromiseDeferred);
        });
        return UtilsService.waitMultiplePromises(promises);

    }

    function loadTaskTrigger(){
        var triggersPayload;
        if (dataSourceTask != undefined && dataSourceTask.TaskData != undefined && dataSourceTask.TaskData.TaskSettings != undefined) {
            triggersPayload = {
                data: dataSourceTask.TaskData.TaskSettings.TaskTriggerArgument
            };
        }
        var loadTaskTriggerPromiseDeferred = UtilsService.createPromiseDeferred();
        taskTriggerDirectiveReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(taskTriggerDirectiveAPI, triggersPayload, loadTaskTriggerPromiseDeferred);
        });
        return loadTaskTriggerPromiseDeferred.promise;
    }

    function setTitle() {
        if (isEditMode && dataSourceTask != undefined && dataSourceTask.TaskData != undefined)
            $scope.title = UtilsService.buildTitleForUpdateEditor(dataSourceTask.TaskData.Name, "Data Source");
        else
            $scope.title = UtilsService.buildTitleForAddEditor("Data Source");
    }

    function loadStaticData() {

        if (dataSourceTask == undefined)
            return;

        taskId = dataSourceTask.TaskData.TaskId;

        $scope.scopeModel.dataSourceName = dataSourceEntity.Entity.Name;

        $scope.scopeModel.customCode = dataSourceEntity.Entity.Settings.MapperCustomCode;
        $scope.scopeModel.isEnabled = dataSourceTask.TaskData.IsEnabled;

        $scope.scopeModel.startEffDate = dataSourceTask.TaskData.TaskSettings.StartEffDate;
        $scope.scopeModel.endEffDate = dataSourceTask.TaskData.TaskSettings.EndEffDate;
    }

    function buildDataSourceObjFromScope() {

        var dataSourceData = {
            DataSourceId: (dataSourceId != null) ? dataSourceId : 0,
            Name: $scope.scopeModel.dataSourceName,
            AdapterTypeId: $scope.scopeModel.selectedAdapterType.ExtensionConfigurationId,
            AdapterState: adapterTypeDirectiveAPI.getStateData(),
            TaskId: taskId,
            Settings: { AdapterArgument: adapterTypeDirectiveAPI.getData(), MapperCustomCode: $scope.scopeModel.customCode, ExecutionFlowId: $scope.scopeModel.selectedExecutionFlow.ExecutionFlowId }
        };

        var taskData = {
            TaskId: taskId,
            Name: 'Data Source Task',
            IsEnabled: $scope.scopeModel.isEnabled,
            TaskType: 0,
            TaskSettings:
                {
                    TaskTriggerArgument: taskTriggerDirectiveAPI.getData(),
                    StartEffDate: $scope.scopeModel.startEffDate,
                    EndEffDate: $scope.scopeModel.endEffDate
                }
        };

        return { DataSourceData: dataSourceData, TaskData: taskData };
    }
  
    function insertDataSource() {

        var dataSourceObject = buildDataSourceObjFromScope();
        return VR_Integration_DataSourceAPIService.AddDataSource(dataSourceObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Data Source", response)) {
                if ($scope.onDataSourceAdded != undefined)
                    $scope.onDataSourceAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });

    }

    function updateDataSource() {

        var dataSourceObject = buildDataSourceObjFromScope();
        VR_Integration_DataSourceAPIService.UpdateDataSource(dataSourceObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Data Source", response)) {
                if ($scope.onDataSourceUpdated != undefined) {

                    $scope.onDataSourceUpdated(response.UpdatedObject);
                }
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });
    }
}

appControllers.controller('Integration_NewDataSourceEditorController', newDataSourceEditorController);
})(appControllers);