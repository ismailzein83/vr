(function (appControllers) {
    "use strict";

    GroupManagementController.$inject = ['$scope', 'VR_Sec_GroupService'];

    function GroupManagementController($scope, VR_Sec_GroupService) {

        var gridApi;
        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.onGroupGridReady = function (api) {
                gridApi = api;
                gridApi.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                gridApi.loadGrid(filter);
            };

            $scope.addNewGroup = addGroup;
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
                if (gridApi != undefined) {
                    gridApi.onGroupAdded(groupObj);
                }
            };

            VR_Sec_GroupService.addGroup(onGroupAdded);
        }
    }

    appControllers.controller('VR_Sec_GroupManagementController', GroupManagementController);

})(appControllers);

