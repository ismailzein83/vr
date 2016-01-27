(function (appControllers) {
    "use strict";

    GroupManagementController.$inject = ['$scope', 'VR_Sec_GroupService'];

    function GroupManagementController($scope, VR_Sec_GroupService) {

        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addGroup = addGroup;
        }

        function load() {
        }

        function getFilterObject() {
            filter = {
                Name: $scope.name
            };
        }

        function addGroup() {
            var onGroupAdded = function (groupObj) {
                if (gridAPI) {
                    gridAPI.onGroupAdded(groupObj);
                }
            };
            VR_Sec_GroupService.addGroup(onGroupAdded);
        }
    }

    appControllers.controller('VR_Sec_GroupManagementController', GroupManagementController);

})(appControllers);
