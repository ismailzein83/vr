(function (appControllers) {

    "use strict";

    dataRecordTypeManagementController.$inject = ['$scope'];

    function dataRecordTypeManagementController($scope) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }
            $scope.name;
        }

        function load() {
         
        }

        function getFilterObject() {
            var data = {
                Name: $scope.name,
            };
            return data;
        }
    }

    appControllers.controller('VRCommon__DataRecordTypeManagementController', dataRecordTypeManagementController);
})(appControllers);