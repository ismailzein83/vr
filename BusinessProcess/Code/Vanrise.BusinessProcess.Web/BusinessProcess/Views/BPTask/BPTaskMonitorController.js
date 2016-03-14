(function (appControllers) {

    "use strict";

    BusinessProcess_BP_TaskMonitorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService','VR_Sec_UserAPIService'];

    function BusinessProcess_BP_TaskMonitorController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VR_Sec_UserAPIService) {
        var gridAPI;
        var filter = {};

        defineScope();
        loadAllControls();
        var userId;
        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                getFilterObject();
                gridAPI.loadGrid(filter);
            };
        }

        function getFilterObject() {
            filter = {
                MyTaskSelected: true
            };
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = 'Business Process Task';
        }
    }

    appControllers.controller('BusinessProcess_BP_TaskMonitorController', BusinessProcess_BP_TaskMonitorController);
})(appControllers);