﻿UserManagementController.$inject = ['$scope', 'UsersAPIService', 'VRModalService'];

function UserManagementController($scope, UsersAPIService, VRModalService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.users = [];

        defineMenuActions();

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            getData();
        };

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };

        $scope.AddNewUser = AddUser;
    }

    function load() {
        
    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();

        var name = $scope.name != undefined ? $scope.name : '';
        var email = $scope.email != undefined ? $scope.email : '';
        return UsersAPIService.GetFilteredUsers(pageInfo.fromRow, pageInfo.toRow, name, email).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.users.push(itm);
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editUser
        },
        {
            name: "Reset Password",
            clicked: resetPassword
        }
        ];
    }

    function AddUser() {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New User";
            modalScope.onUserAdded = function (user) {
                mainGridAPI.itemAdded(user);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/UserEditor.html', null, settings);

    }

    function editUser(userObj) {
        var modalSettings = {
        };
        var parameters = {
            userId: userObj.UserId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit User: " + userObj.Name;
            modalScope.onUserUpdated = function (user) {
                mainGridAPI.itemUpdated(user);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/UserEditor.html', parameters, modalSettings);
    }

    function resetPassword(userObj) {
        var modalSettings = {
        };
        var parameters = {
            userId: userObj.UserId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Reset Password for User: " + userObj.Name;
            modalScope.onRoleUpdated = function (user) {
                mainGridAPI.itemUpdated(user);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/ResetPasswordEditor.html', parameters, modalSettings);
    }    
}
appControllers.controller('Security_UserManagementController', UserManagementController);