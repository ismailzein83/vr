(function (appControllers) {

    "use strict";

    popManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_PopService'];

    function popManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, Demo_PopService) {
        var gridAPI;
       
        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }

            $scope.AddNewPop = addNewPop;

            function getFilterObject() {
                var data = {
                    Name: $scope.name
                };
                return data;
            }
        }

        function load() {
        }

        function addNewPop() {
            var onPopAdded = function (popObj) {
                gridAPI.onPopAdded(popObj);
            };
            Demo_PopService.addPop(onPopAdded);
        }
    }

    appControllers.controller('Demo_PopManagementController', popManagementController);
})(appControllers);