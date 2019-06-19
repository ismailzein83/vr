(function (appControllers) {

    "use strict";

    BusinessProcess_BP_TaskMonitorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function BusinessProcess_BP_TaskMonitorController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
        var gridAPI;
        var taskTypeSelectorAPI;
        var taskTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };

            $scope.onTaskTypeSelectorReady = function (api) {
                taskTypeSelectorAPI = api;
                taskTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };
        }

        function getFilterObject() {
            return {
                MyTaskSelected: true,
                BPTaskFilter: {
                    Title: $scope.taskTitle,
                    TaskTypeIds: taskTypeSelectorAPI != undefined ? taskTypeSelectorAPI.getSelectedIds() : undefined
                }
            };
        }

        function loadAllControls() {
            $scope.isLoading = true;

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadTaskTypeSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });

            function loadTaskTypeSelector() {
                var taskTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                taskTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        businessEntityDefinitionId: "d33fd65a-721f-4ae1-9d41-628be9425796"
                    };
                    VRUIUtilsService.callDirectiveLoad(taskTypeSelectorAPI, payload, taskTypeSelectorLoadPromiseDeferred);
                });
                return taskTypeSelectorLoadPromiseDeferred.promise;
            }
        }

        function setTitle() {
            $scope.title = 'Business Process Task';
        }
    }

    appControllers.controller('BusinessProcess_BP_TaskMonitorController', BusinessProcess_BP_TaskMonitorController);
})(appControllers);