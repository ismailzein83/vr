(function (appControllers) {
    "use strict";

    UserManagementController.$inject = ['$scope', 'VR_Sec_UserService'];

    function UserManagementController($scope, VR_Sec_UserService) {

        var gridApi;
        var filter = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onUserGridReady = function (api) {
                gridApi = api;
                gridApi.loadGrid(filter);
            }

            $scope.searchClicked = function () {
                getFilterObject();
                gridApi.loadGrid(filter);
            };

            $scope.AddNewUser = AddUser;
        }

        function load() {

        }

        function getFilterObject() {
            filter = {
                Name: $scope.name,
                Email: $scope.email
            };
        }

        function AddUser() {
            var onUserAdded = function (userObj) {
                if (gridApi != undefined) {
                    gridApi.onUserAdded(userObj);
                }
            };
            VR_Sec_UserService.addUser(onUserAdded);
        }
    }

    appControllers.controller('VR_Sec_UserManagementController', UserManagementController);

})(appControllers);






