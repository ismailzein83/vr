UserManagementController.$inject = ['$scope', 'UsersAPIService', 'VRModalService', 'VRNotificationService'];

function UserManagementController($scope, UsersAPIService, VRModalService, VRNotificationService) {

    var gridApi;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {

        $scope.users = [];
        $scope.gridMenuActions = [];
        
        defineMenuActions();

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return UsersAPIService.GetFilteredUsers(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.AddNewUser = AddUser;
    }

    function load() {
        
    }

    function retrieveData() {
        var query = {
            Name: $scope.name,
            Email: $scope.email
        };

        return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editUser,
            permissions: "Root/Administration Module/Users:Edit"
        },
        {
            name: "Reset Password",
            clicked: resetPassword,
            permissions: "Root/Administration Module/Users:Reset Password"
        },
        {
            name: "Assign Permissions",
            clicked: assignPermissions,
            permissions: "Root/Administration Module/System Entities:Assign Permissions"
        }
        ];
    }

    function AddUser() {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New User";
            modalScope.onUserAdded = function (user) {
                gridApi.itemAdded(user);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/UserEditor.html', null, settings);

    }

    function editUser(userObj) {
        var modalSettings = {
        };
        var parameters = {
            userId: userObj.Entity.UserId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit User: " + userObj.Name;
            modalScope.onUserUpdated = function (user) {
                gridApi.itemUpdated(user);
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/UserEditor.html', parameters, modalSettings);
    }

    function resetPassword(userObj) {
        var modalSettings = {
        };
        var parameters = {
            userId: userObj.Entity.UserId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Reset Password for User: " + userObj.Name;
            modalScope.onPasswordReset = function (user) {
                // user is null
                //gridApi.itemUpdated(user);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/ResetPasswordEditor.html', parameters, modalSettings);
    }

    function assignPermissions(userObj) {
        var modalSettings = {
        };
        var parameters = {
            holderType: 0,
            holderId: userObj.Entity.UserId,
            notificationResponseText: "User Permissions"
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Assign Permissions to User: " + userObj.Name;
        };
        VRModalService.showModal('/Client/Modules/Security/Views/PermissionEditor.html', parameters, modalSettings);
    }
}

appControllers.controller('Security_UserManagementController', UserManagementController);