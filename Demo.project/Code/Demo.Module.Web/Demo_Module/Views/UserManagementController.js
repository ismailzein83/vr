(function (appControllers) {

    "use strict";

    userManagementController.$inject = ['$scope', 'VRNotificationService', 'Demo_Module_UserAPIService', 'UtilsService', 'VRUIUtilsService', 'Demo_Module_UserService'];

    function userManagementController($scope, VRNotificationService, Demo_Module_UserAPIService, UtilsService, VRUIUtilsService, Demo_Module_UserService) {




        var gridAPI;
        var query = {};
        defineScope();
        load();





        function defineScope() {
            $scope.users = [];

            $scope.searchClicked = function () {
                getfilterdobject()
                getfilterdobject()
                gridAPI.loadGrid(query);
            };

            function getfilterdobject() {
                query = {
                    Name: $scope.name
                };
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(query);
            };
            $scope.addNewUser = addNewUser;
        }
        function load() {

            loadAllControls();
        }

        function loadAllControls() {

        }
        function addNewUser() {
            var onUserAdded = function (UserObj) {
                gridAPI.onUserAdded(UserObj);
            };

            Demo_Module_UserService.addUser(onUserAdded);
        }
    }




    appControllers.controller('Demo_Module_UserManagementController', userManagementController);
})(appControllers);