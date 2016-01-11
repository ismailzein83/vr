(function (appControllers) {

    "use strict";

    Qm_CliTester_ScheduleTestCallManagementController.$inject = ['$scope', 'VR_Runtime_SchedulerTaskService'];

    function Qm_CliTester_ScheduleTestCallManagementController($scope, VR_Runtime_SchedulerTaskService) {

        var gridAPI;

        defineScope();

        function defineScope() {

            $scope.name = undefined;
            $scope.schedulerTasks = [];
            $scope.gridMenuActions = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                return retrieveData();
            };

            $scope.searchClicked = function () {
                return retrieveData();
            };

            $scope.AddNewTask = addTask;

        }

        function retrieveData() {
            var name = ($scope.name != undefined && $scope.name != '') ? $scope.name : null;

            return gridAPI.loadGrid(name);
        }

        function addTask() {
            var onTaskAdded = function (addedItem) {
                gridAPI.onTaskAdded(addedItem);
            };

            VR_Runtime_SchedulerTaskService.addTask(onTaskAdded);
        }

    }

    appControllers.controller('Qm_CliTester_ScheduleTestCallManagementController', Qm_CliTester_ScheduleTestCallManagementController);
})(appControllers);