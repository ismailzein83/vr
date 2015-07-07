DataSourceTaskEditorController.$inject = ['$scope', 'DataSourceAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DataSourceTaskEditorController($scope, DataSourceAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    var dataSourceId;
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

        if (taskId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.SaveDataSourceTask = function () {
            if (editMode) {
                return updateDataSourceTask();
            }
            else {
                return insertDataSourceTask();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.schedulerTaskTrigger = {};

    }

    function load() {
        if (editMode) {
            $scope.isGettingData = true;
            return DataSourceAPIService.GetDataSource(dataSourceId)
          .then(function (response) {
              fillScopeFromDataSourceObj(response);
          })
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           }).finally(function () {
               $scope.isGettingData = false;
           });
        }
    }

    function buildDataSourceTaskObjFromScope() {

        var dataSourceTaskObject = {
            DataSourceId: dataSourceId, Task: {
                TaskId: (taskId != null) ? taskId : 0,
                Name: 'Data Source Task',
                IsEnabled: $scope.isEnabled,
                TaskType: 0,
                TriggerTypeId: 1,
                TaskTrigger: $scope.schedulerTaskTrigger.getData(),
                ActionTypeId: 2,
                TaskAction: { $type: "Vanrise.Integration.Extensions.DSSchedulerTaskAction, Vanrise.Integration.Extensions" }
            }
        };
        return dataSourceTaskObject;
    }

    function fillScopeFromDataSourceTaskObj(dataSourceObject) {
        $scope.selectedAdapterType = UtilsService.getItemByVal($scope.adapterTypes, dataSourceObject.AdapterTypeId, "AdapterTypeId");
        $scope.dataSourceAdapter.data = dataSourceObject.Settings.Adapter;
    }

    function insertDataSourceTask() {
        //$scope.issaving = true;
        var dataSourceTaskObject = buildDataSourceTaskObjFromScope();
        return DataSourceAPIService.AddDataSourceTask(dataSourceTaskObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Data Source", response)) {
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateDataSourceTask() {
        var dataSourceObject = buildDataSourceTaskObjFromScope();
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
appControllers.controller('Integration_DataSourceTaskEditorController', DataSourceTaskEditorController);
