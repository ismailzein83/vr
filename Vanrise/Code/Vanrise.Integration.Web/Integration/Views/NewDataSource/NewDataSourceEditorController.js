(function (appControllers) {
    "use strict";
newDataSourceEditorController.$inject = ['$scope', 'DataSourceAPIService', 'SchedulerTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function newDataSourceEditorController($scope, DataSourceAPIService, SchedulerTaskAPIService, UtilsService, VRUIUtilsService , VRModalService, VRNotificationService, VRNavigationService) {
    var dataSourceObj;
    var editMode;

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

        editMode = (dataSourceId != undefined);
    }

    function defineScope() {
        $scope.SaveDataSource = function () {
            if (editMode) {
                return updateDataSource();
            }
            else {
                return insertDataSource();
            }
        };
        $scope.scopeModel = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.onTaskTriggerDirectiveReady = function (api) {
            taskTriggerDirectiveAPI = api;
            taskTriggerDirectiveReadyPromiseDeferred.resolve();

        }
        $scope.onAdapterTypeDirectiveReady = function (api) {
            adapterTypeDirectiveAPI = api;
            adapterTypeDirectiveReadyPromiseDeferred.resolve();
        }
        $scope.addExecutionFlow = function () {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Add an Execution Flow";

                modalScope.onExecutionFlowAdded = function () {
                    // update the execution flows
                    $scope.isLoading = true;

                    // clear the selection menu
                    $scope.executionFlows = [];

                    loadExecutionFlows()
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        })
                        .finally(function () {
                            $scope.isLoading = false;
                        });
                };
            };

            VRModalService.showModal('/Client/Modules/Integration/Views/DataSourceExecutionFlowEditor.html', null, modalSettings);
        }

        $scope.adapterTypes = [];
        $scope.executionFlows = [];
        $scope.dataSourceAdapter = {};
        $scope.dataSourceAdapter.argument = {};
        $scope.dataSourceAdapter.adapterState = {};

        $scope.schedulerTaskTrigger = {};
        $scope.timeTriggerTemplateURL = undefined;

        $scope.scopeModel.startEffDate = new Date();
        $scope.scopeModel.endEffDate = undefined;
    }

    function load() {
        $scope.isLoading = true;

        if (editMode) {
            getDataSourceToEdit().catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }
        else {
            loadAllControls();
        }
    }

    function getDataSourceToEdit() {
        return DataSourceAPIService.GetDataSource(dataSourceId).then(function (dataSourceResponse) {
            getDataSourceTask(dataSourceResponse).then(function () {
                loadAllControls().finally(function () {
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });;
            
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
            $scope.isLoading = false;
        });
    }
   
    function getDataSourceTask(dataSourceResponse) {
        return SchedulerTaskAPIService.GetTask(dataSourceResponse.TaskId)
               .then(function (taskResponse) {
                   dataSourceObj = { DataSourceData: dataSourceResponse, TaskData: taskResponse };                   
               })
               .catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.isLoading = false;
               });
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadAdapterType, loadExecutionFlows, loadTaskTrigger, setTitle, loadStaticData]).then(function () {
            dataSourceObj = undefined;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        })
        .finally(function () {
            $scope.isLoading = false;
       });
    }

    function loadAdapters() {
        return DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.adapterTypes.push(item);
            });
            if (dataSourceObj != undefined && dataSourceObj.DataSourceData != undefined && dataSourceObj.DataSourceData.AdapterTypeId != undefined)
               $scope.scopeModel.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, dataSourceObj.DataSourceData.AdapterTypeId, "AdapterTypeId");

        });
    }

    function loadExecutionFlows() {
        return DataSourceAPIService.GetExecutionFlows().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.executionFlows.push(item);
            });
            if (dataSourceObj != undefined && dataSourceObj.DataSourceData != undefined && dataSourceObj.DataSourceData.Settings != undefined && dataSourceObj.DataSourceData.Settings.ExecutionFlowId != undefined)
                $scope.scopeModel.selectedExecutionFlow = UtilsService.getItemByVal($scope.executionFlows, dataSourceObj.DataSourceData.Settings.ExecutionFlowId, "ExecutionFlowId");

        });
    }

    function loadAdapterType() {
        var promises = [];        
        var dataSourcePromiseLoad = DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.adapterTypes.push(item);
            });
            if (dataSourceObj != undefined && dataSourceObj.DataSourceData != undefined && dataSourceObj.DataSourceData.AdapterTypeId != undefined)
                $scope.scopeModel.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, dataSourceObj.DataSourceData.AdapterTypeId, "AdapterTypeId");
            else {
                $scope.scopeModel.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, 1, "AdapterTypeId");
            }
        });

        promises.push(dataSourcePromiseLoad);

        var adapterPayload = {};
        if (dataSourceObj != undefined && dataSourceObj.DataSourceData != undefined && dataSourceObj.DataSourceData.Settings != undefined && dataSourceObj.DataSourceData.Settings.AdapterArgument != undefined)
            adapterPayload.adapterArgument = dataSourceObj.DataSourceData.Settings.AdapterArgument;
        if (dataSourceObj != undefined && dataSourceObj.DataSourceData != undefined && dataSourceObj.DataSourceData.AdapterState != undefined && adapterPayload != undefined)
            adapterPayload.adapterState = dataSourceObj.DataSourceData.AdapterState;


        var loadAdapterTypePromiseDeferred = UtilsService.createPromiseDeferred();
        promises.push(loadAdapterTypePromiseDeferred.promise);
        adapterTypeDirectiveReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(adapterTypeDirectiveAPI, adapterPayload, loadAdapterTypePromiseDeferred);
        });
        return UtilsService.waitMultiplePromises(promises);

    }

    function loadTaskTrigger(){
        var triggersPayload;
        if (dataSourceObj != undefined && dataSourceObj.TaskData != undefined && dataSourceObj.TaskData.TaskSettings != undefined) {
            triggersPayload = {
                data: dataSourceObj.TaskData.TaskSettings.TaskTriggerArgument
            };
        }
        var loadTaskTriggerPromiseDeferred = UtilsService.createPromiseDeferred();
        taskTriggerDirectiveReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(taskTriggerDirectiveAPI, triggersPayload, loadTaskTriggerPromiseDeferred);
        });
        return loadTaskTriggerPromiseDeferred.promise;
    }

    function setTitle() {
        if (editMode && dataSourceObj != undefined && dataSourceObj.TaskData != undefined)
            $scope.title = UtilsService.buildTitleForUpdateEditor(dataSourceObj.TaskData.Name, "Data Source");
        else
            $scope.title = UtilsService.buildTitleForAddEditor("Data Source");
    }

    function loadStaticData() {

        if (dataSourceObj == undefined)
            return;

        taskId = dataSourceObj.TaskData.TaskId;

        $scope.scopeModel.dataSourceName = dataSourceObj.DataSourceData.Name;

        $scope.scopeModel.customCode = dataSourceObj.DataSourceData.Settings.MapperCustomCode;
        $scope.scopeModel.isEnabled = dataSourceObj.TaskData.IsEnabled;

        $scope.scopeModel.startEffDate = dataSourceObj.TaskData.TaskSettings.StartEffDate;
        $scope.scopeModel.endEffDate = dataSourceObj.TaskData.TaskSettings.EndEffDate;
    }

    function buildDataSourceObjFromScope() {

        var dataSourceData = {
            DataSourceId: (dataSourceId != null) ? dataSourceId : 0,
            Name: $scope.scopeModel.dataSourceName,
            AdapterTypeId: $scope.scopeModel.selectedAdapterType.AdapterTypeId,
            AdapterState: adapterTypeDirectiveAPI.getStateData(),
            TaskId: taskId,
            Settings: { AdapterArgument: adapterTypeDirectiveAPI.getData(), MapperCustomCode: $scope.scopeModel.customCode, ExecutionFlowId: $scope.scopeModel.selectedExecutionFlow.ExecutionFlowId }
        };

        var taskData = {
            TaskId: taskId,
            Name: 'Data Source Task',
            IsEnabled: $scope.scopeModel.isEnabled,
            TaskType: 0,
            TriggerTypeId: 1,
            ActionTypeId: 2,
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
        $scope.isLoading = true;

        var dataSourceObject = buildDataSourceObjFromScope();
        return DataSourceAPIService.AddDataSource(dataSourceObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Data Source", response)) {
                if ($scope.onDataSourceAdded != undefined)
                    $scope.onDataSourceAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.isLoading = false;
        });

    }

    function updateDataSource() {

        var dataSourceObject = buildDataSourceObjFromScope();
        $scope.isLoading = true;
        DataSourceAPIService.UpdateDataSource(dataSourceObject)
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
            $scope.isLoading = false;
        });
    }
}

appControllers.controller('Integration_NewDataSourceEditorController', newDataSourceEditorController);
})(appControllers);