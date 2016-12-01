(function (appControllers) {

    "use strict";

    popManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Retail_BE_PopService'];

    function popManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_PopService) {
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
            };

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
            Retail_BE_PopService.addPop(onPopAdded);
        }
    }

    appControllers.controller('Retail_BE_PopManagementController', popManagementController);
})(appControllers);