(function (appControllers) {

    "use strict";

    BusinessProcess_BPDefinitionManagementController.$inject = ['$scope'];

    function BusinessProcess_BPDefinitionManagementController($scope) {
        var gridAPI;
        defineScope();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
        }

        function getFilterObject() {
            var filter = {};
            filter.Title = $scope.title;
            return filter;
        }
    }

    appControllers.controller('BusinessProcess_BP_TechnicalDefinitionManagementController', BusinessProcess_BPDefinitionManagementController);
})(appControllers);