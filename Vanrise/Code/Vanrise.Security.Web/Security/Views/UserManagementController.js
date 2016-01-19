UserManagementController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VR_Sec_UserService', 'VRModalService', 'VRNotificationService'];

function UserManagementController($scope, UsersAPIService, VR_Sec_UserService , VRModalService, VRNotificationService) {

    var gridApi;
    var filter = {};
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {
        $scope.onUserGridReady = function (api) {
            gridApi = api;
            gridApi.loadGrid(filter);
        }

        $scope.searchClicked = function () {
            setFilterObject()
            gridApi.loadGrid(filter)
        };

        $scope.AddNewUser = AddUser;

      

    }

    function load() {
        
    }

    
    function setFilterObject() {
        filter = {
            Name: $scope.name,
            Email: $scope.email
        };


    }
    //function defineMenuActions() {
    //    $scope.gridMenuActions = [{
    //        name: "Edit",
    //        clicked: editUser,
    //        permissions: "Root/Administration Module/Users:Edit"
    //    },
    //    {
    //        name: "Reset Password",
    //        clicked: resetPassword,
    //        permissions: "Root/Administration Module/Users:Reset Password"
    //    },
    //    {
    //        name: "Assign Permissions",
    //        clicked: assignPermissions,
    //        permissions: "Root/Administration Module/System Entities:Assign Permissions"
    //    }
    //    ];
    //}

    function AddUser() {
        var onUserAdded = function (userObj) {
            if (gridApi != undefined) {
                gridApi.onUserAdded(userObj); 
            }
        };
        VR_Sec_UserService.addUser(onUserAdded)
       

    }

    //function editUser(userObj) {
    //    var modalSettings = {
    //    };
    //    var parameters = {
    //        userId: userObj.Entity.UserId
    //    };

    //    modalSettings.onScopeReady = function (modalScope) {
    //        modalScope.title = "Edit User: " + userObj.Entity.Name;
    //        modalScope.onUserUpdated = function (user) {
    //            gridApi.itemUpdated(user);
    //        };
    //    };
    //    VRModalService.showModal('/Client/Modules/Security/Views/UserEditor.html', parameters, modalSettings);
    //}

    //function resetPassword(userObj) {
    //    var modalSettings = {
    //    };
    //    var parameters = {
    //        userId: userObj.Entity.UserId
    //    };

    //    modalSettings.onScopeReady = function (modalScope) {
    //        modalScope.title = "Reset Password for User: " + userObj.Entity.Name;
    //        modalScope.onPasswordReset = function (user) {
    //            // user is null
    //            //gridApi.itemUpdated(user);
    //        };
    //    };

    //    VRModalService.showModal('/Client/Modules/Security/Views/ResetPasswordEditor.html', parameters, modalSettings);
    //}

    //function assignPermissions(userObj) {
    //    var modalSettings = {
    //    };
    //    var parameters = {
    //        holderType: 0,
    //        holderId: userObj.Entity.UserId,
    //        notificationResponseText: "User Permissions"
    //    };

    //    modalSettings.onScopeReady = function (modalScope) {
    //        modalScope.title = "Assign Permissions to User: " + userObj.Entity.Name;
    //    };
    //    VRModalService.showModal('/Client/Modules/Security/Views/PermissionEditor.html', parameters, modalSettings);
    //}
}

appControllers.controller('VRSec_UserManagementController', UserManagementController);