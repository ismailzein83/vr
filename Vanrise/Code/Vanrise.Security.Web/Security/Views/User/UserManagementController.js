(function (appControllers) {
    'use strict';

    UserManagementController.$inject = ['$scope', 'VR_Sec_UserService', 'VR_Sec_UserAPIService'];

    function UserManagementController($scope, VR_Sec_UserService, VR_Sec_UserAPIService) {

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

            $scope.addUser = function () {
                var onUserAdded = function (userObj) {
                    gridAPI.onUserAdded(userObj);
                };

                VR_Sec_UserService.addUser(onUserAdded);
            };

            $scope.hasAddUserPermission = function () {
                return VR_Sec_UserAPIService.HasAddUserPermission();
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
