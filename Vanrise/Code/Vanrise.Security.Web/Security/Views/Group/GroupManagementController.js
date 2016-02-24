(function (appControllers) {
    "use strict";

    GroupManagementController.$inject = ['$scope', 'VR_Sec_GroupService', 'VR_Sec_GroupAPIService'];

    function GroupManagementController($scope, VR_Sec_GroupService, VR_Sec_GroupAPIService) {

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

            $scope.hasAddGroupPermission = function () {
                return VR_Sec_GroupAPIService.HasAddGroupPermission();
            };
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
