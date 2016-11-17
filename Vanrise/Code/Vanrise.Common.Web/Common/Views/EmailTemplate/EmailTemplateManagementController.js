(function (appControllers) {

    "use strict";

    emailTemplateManagementController.$inject = ['$scope'];

    function emailTemplateManagementController($scope) {
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
                gridAPI.loadGrid(filter)
            };
        }

        function load() {
        }

        function setFilterObject() {
            filter = {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('VRCommon_EmailTemplateManagementController', emailTemplateManagementController);
})(appControllers);