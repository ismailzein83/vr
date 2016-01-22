(function (appControllers) {
    'use strict';

    UserManagementController.$inject = ['$scope', 'VR_Sec_UserService'];

    function UserManagementController($scope, VR_Sec_UserService) {

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
                gridAPI.loadGrid(filter);
            };

            $scope.addUser = function () {
                var onUserAdded = function (userObj) {
                    if (gridAPI) {
                        gridAPI.onUserAdded(userObj);
                    }
                };
                VR_Sec_UserService.addUser(onUserAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
            filter = {
                Name: $scope.name,
                Email: $scope.email
            };
        }
    }

    appControllers.controller('VR_Sec_UserManagementController', UserManagementController);

})(appControllers);
