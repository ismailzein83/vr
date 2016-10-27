(function (appControllers) {

    "use strict";

    StateBackupManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function StateBackupManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService) {
        var gridAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {

            $scope.searchClicked = function () {

                setFilterObject();
                return gridAPI.loadGrid(filter);
            };
           
            $scope.onGridReady = function (api) {
                gridAPI = api;
            }
        }

        function load() {
            //$scope.isGettingData = true;
            //loadAllControls();
        }


        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }

      
        function setFilterObject() {
            filter = {
            };

        }

    }

    appControllers.controller('WhS_BE_StateBackupManagementController', StateBackupManagementController);
})(appControllers);