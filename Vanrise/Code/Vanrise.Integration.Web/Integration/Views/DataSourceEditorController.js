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

        $scope.schedulerTaskTrigger = {};
        $scope.timeTriggerTemplateURL = undefined;
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
        return UtilsService.waitMultipleAsyncOperations([getDataSource]).then(function () {
            getDataSourceTask();

        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
            $scope.isGettingData = false;
        });
    }
    
    var dataSourceObjToEdit;
    function getDataSource() {
        return DataSourceAPIService.GetDataSource(dataSourceId)
           .then(function (dataSourceResponse) {
               dataSourceObjToEdit = dataSourceResponse;
           });
    }

    function getDataSourceTask() {
        return SchedulerTaskAPIService.GetTask(dataSourceObjToEdit.TaskId)
               .then(function (taskResponse) {
                   var dataSourceObj = { DataSourceData: dataSourceObjToEdit, TaskData: taskResponse }
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
            Settings: { AdapterArgument: $scope.dataSourceAdapter.getData(), MapperCustomCode: $scope.customCode, ExecutionFlowId: $scope.selectedExecutionFlow.ExecutionFlowId }
        };

        var taskData = {
            TaskId: (taskId != null) ? taskId : 0,
            Name: 'Data Source Task',
            IsEnabled: $scope.isEnabled,
            TaskType: 0,
            TriggerTypeId: 1,
            TaskTrigger: $scope.schedulerTaskTrigger.getData(),
            ActionTypeId: 2,
            TaskAction: { $type: "Vanrise.Integration.Business.DSSchedulerTaskAction, Vanrise.Integration.Business" }
        };

        return { DataSourceData: dataSourceData, TaskData: taskData };
    }

    function fillScopeFromDataSourceObj(dataSourceObject) {
        $scope.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, dataSourceObject.DataSourceData.AdapterTypeId, "AdapterTypeId");
        $scope.dataSourceName = dataSourceObject.DataSourceData.Name;

        $scope.dataSourceAdapter.data = dataSourceObject.DataSourceData.Settings.AdapterArgument;
        if ($scope.dataSourceAdapter.loadTemplateData != undefined)
            $scope.dataSourceAdapter.loadTemplateData();

        $scope.customCode = dataSourceObject.DataSourceData.Settings.MapperCustomCode;
        $scope.isEnabled = dataSourceObject.TaskData.IsEnabled;
        

        $scope.schedulerTaskTrigger.data = dataSourceObject.TaskData.TaskTrigger;
        if ($scope.schedulerTaskTrigger.loadTemplateData != undefined)
            $scope.schedulerTaskTrigger.loadTemplateData();

        $scope.selectedExecutionFlow = UtilsService.getItemByVal($scope.executionFlows, dataSourceObject.DataSourceData.Settings.ExecutionFlowId, "ExecutionFlowId");
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
