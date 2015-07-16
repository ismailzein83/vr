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

        if (dataSourceId != undefined)
            editMode = true;
        else
            editMode = false;
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

        $scope.adapterTypes = [];
        $scope.dataSourceAdapter = {};

        $scope.schedulerTaskTrigger = {};
        $scope.timeTriggerTemplateURL = undefined;
    }

    function load() {

        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadAdapters]).finally(function () {
            if (editMode) {
                getDataSource();
            }
            else {
                $scope.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, 1, "AdapterTypeId");
            }

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isGettingData = false;
        });
    }

    function getDataSource() {
        return DataSourceAPIService.GetDataSource(dataSourceId)
           .then(function (dataSourceResponse) {
               SchedulerTaskAPIService.GetTask(dataSourceResponse.TaskId)
               .then(function (taskResponse) {
                   var dataSourceObj = { DataSourceData: dataSourceResponse, TaskData: taskResponse }
                   fillScopeFromDataSourceObj(dataSourceObj);
               })
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               });
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function loadAdapters() {
        return DataSourceAPIService.GetDataSourceAdapterTypes().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.adapterTypes.push(item);
            });
        });
    }

    function buildDataSourceObjFromScope() {
        
        var adapterData = $scope.dataSourceAdapter.getData();

        adapterData.$type = $scope.selectedAdapterType.Info.FQTN;

        var dataSourceData = {
            DataSourceId: (dataSourceId != null) ? dataSourceId : 0,
            Name: $scope.dataSourceName,
            AdapterTypeId: $scope.selectedAdapterType.AdapterTypeId,
            Settings: { Adapter: adapterData, MapperCustomCode: $scope.customCode }
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

        $scope.dataSourceAdapter.data = dataSourceObject.DataSourceData.Settings.Adapter;
        if ($scope.dataSourceAdapter.loadTemplateData != undefined)
            $scope.dataSourceAdapter.loadTemplateData();

        $scope.customCode = dataSourceObject.DataSourceData.Settings.MapperCustomCode;
        $scope.isEnabled = dataSourceObject.TaskData.IsEnabled;

        $scope.schedulerTaskTrigger.data = dataSourceObject.TaskData.TaskTrigger;
        if ($scope.schedulerTaskTrigger.loadTemplateData != undefined)
            $scope.schedulerTaskTrigger.loadTemplateData();
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
