DataSourceEditorController.$inject = ['$scope', 'DataSourceAPIService', 'SchedulerTaskAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DataSourceEditorController($scope, DataSourceAPIService, SchedulerTaskAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    var dataSourceId;
    var taskId;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        dataSourceId = undefined;
        taskId = undefined;

        if (parameters != undefined && parameters != null) {
            dataSourceId = parameters.dataSourceId;
            taskId = parameters.taskId;
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

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.addExecutionFlow = function () {

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Add an Execution Flow";

                modalScope.onExecutionFlowAdded = function () {
                    // update the execution flows
                    $scope.isGettingData = true;

                    // clear the selection menu
                    $scope.executionFlows = [];

                    loadExecutionFlows()
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        })
                        .finally(function () {
                            $scope.isGettingData = false;
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

        $scope.startEffDate = new Date();
        $scope.endEffDate = undefined;
    }

    function load() {
        $scope.isGettingData = true;
        return UtilsService.waitMultipleAsyncOperations([loadAdapters, loadExecutionFlows]).then(function () {
            if (editMode) {
                getDataSourceToEdit();
            }
            else {
                $scope.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, 1, "AdapterTypeId");
                $scope.isGettingData = false;
            }

        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
            $scope.isGettingData = false;
        });
    }

    function getDataSourceToEdit() {
        return DataSourceAPIService.GetDataSource(dataSourceId).then(function (dataSourceResponse) {

            getDataSourceTask(dataSourceResponse);
            
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
            $scope.isGettingData = false;
        });
    }

    function getDataSourceTask(dataSourceResponse) {
        return SchedulerTaskAPIService.GetTask(dataSourceResponse.TaskId)
               .then(function (taskResponse) {
                   var dataSourceObj = { DataSourceData: dataSourceResponse, TaskData: taskResponse }
                   fillScopeFromDataSourceObj(dataSourceObj);
               })
               .catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               }).finally(function () {
                   $scope.isGettingData = false;
               });
    }

    function loadAdapters() {
        return DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.adapterTypes.push(item);
            });
        });
    }

    function loadExecutionFlows() {
        return DataSourceAPIService.GetExecutionFlows().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.executionFlows.push(item);
            });
        });
    }

    function buildDataSourceObjFromScope() {

        var dataSourceData = {
            DataSourceId: (dataSourceId != null) ? dataSourceId : 0,
            Name: $scope.dataSourceName,
            AdapterTypeId: $scope.selectedAdapterType.AdapterTypeId,
            AdapterState: $scope.dataSourceAdapter.adapterState.getData(),
            TaskId: (taskId != null) ? taskId : 0,
            Settings: { AdapterArgument: $scope.dataSourceAdapter.argument.getData(), MapperCustomCode: $scope.customCode, ExecutionFlowId: $scope.selectedExecutionFlow.ExecutionFlowId }
        };

        var taskData = {
            TaskId: (taskId != null) ? taskId : 0,
            Name: 'Data Source Task',
            IsEnabled: $scope.isEnabled,
            TaskType: 0,
            TriggerTypeId: 1,
            ActionTypeId: 2,
            TaskSettings:
                {
                    TaskTriggerArgument: $scope.schedulerTaskTrigger.getData(),
                    StartEffDate: $scope.startEffDate,
                    EndEffDate: $scope.endEffDate
                }
        };

        return { DataSourceData: dataSourceData, TaskData: taskData };
    }

    function fillScopeFromDataSourceObj(dataSourceObj) {
        $scope.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, dataSourceObj.DataSourceData.AdapterTypeId, "AdapterTypeId");
        $scope.dataSourceName = dataSourceObj.DataSourceData.Name;

        $scope.dataSourceAdapter.adapterState.data = dataSourceObj.DataSourceData.AdapterState;
        $scope.dataSourceAdapter.argument.data = dataSourceObj.DataSourceData.Settings.AdapterArgument;
        
        if ($scope.dataSourceAdapter.loadTemplateData != undefined)
            $scope.dataSourceAdapter.loadTemplateData();

        $scope.customCode = dataSourceObj.DataSourceData.Settings.MapperCustomCode;
        $scope.isEnabled = dataSourceObj.TaskData.IsEnabled;

        $scope.startEffDate = dataSourceObj.TaskData.TaskSettings.StartEffDate;
        $scope.endEffDate = dataSourceObj.TaskData.TaskSettings.EndEffDate;

        $scope.schedulerTaskTrigger.data = dataSourceObj.TaskData.TaskSettings.TaskTriggerArgument;
        if ($scope.schedulerTaskTrigger.loadTemplateData != undefined)
            $scope.schedulerTaskTrigger.loadTemplateData();

        $scope.selectedExecutionFlow = UtilsService.getItemByVal($scope.executionFlows, dataSourceObj.DataSourceData.Settings.ExecutionFlowId, "ExecutionFlowId");
    }

    function insertDataSource() {
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
        });

    }

    function updateDataSource() {
        var dataSourceObject = buildDataSourceObjFromScope();
        DataSourceAPIService.UpdateDataSource(dataSourceObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Data Source", response)) {
                if ($scope.onDataSourceUpdated != undefined)
                    $scope.onDataSourceUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}
appControllers.controller('Integration_DataSourceEditorController', DataSourceEditorController);
