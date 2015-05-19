/// <reference path="UserSettings.html" />
/// <reference path="User.html" />
appControllers.controller('UserManagementController', function UserController($scope, UsersAPIService, VRModalService) {
    $scope.users = [];
    var current = 0;
    //$scope.loadUsers = function () {
    //    var params = {};
    //    params.pageSize = 20;
    //    params.pageNumber = current;
    //    UsersAPIService.GetUserList(params).then(function (response) {
    //        $scope.users = response;
    //    }).finally(function () {

    //        $scope.isGettingData = false;
    //    });
    //}
    //load();

    $scope.gridMenuActions = [{
        name: "Edit",
        clicked: function (dataItem) {
            var modalScope = VRModalService.showModal('/Client/Modules/Main/Views/UserEditor.html', false, dataItem);
            modalScope.title = "Edit User";
            modalScope.onUserUpdated = function (user) {
                gridApi.itemUpdated(user);
            };
        }
    },
    {
        name: "Delete",
        clicked: function (dataItem) {
            $scope.DeleteUser(dataItem);
        }
    }
    ];

    //Action
    $scope.DeleteUser = function (user) {

        UsersAPIService.DeleteUser(user.UserId).then(function (response) {

        }).finally(function () {
            loadUsersSearch($scope.txtname, $scope.txtemail);
        });
    }

  
    
    var gridApi;
    $scope.gridReady = function (api) {
        gridApi = api;
    };


    var current = 0;
    var last = false;
    $scope.loadMoreData = function (asyncHandle) {
       
        if (last) {
            if (asyncHandle)
                asyncHandle.operationDone();
            return;
        }
        var params = {};
        params.pageSize = 20;
        params.pageNumber = current;      
        
        UsersAPIService.GetUserList(params).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.users.push(itm);
            });
            current = current + params.pageSize;
            last = (response.length < params.pageSize) ? true : false;

        }).finally(function () {
            if (asyncHandle) {

                asyncHandle.operationDone();
            }

        });
    };
    $scope.loadMoreData();

    //Action
    $scope.SearchUser = function () {
        loadUsersSearch($scope.txtname, $scope.txtemail);
    }

    $scope.ValidateUser = function (text) {
        if(text.length < 3)
            return "Invalid";
    }

    $scope.AddNewUser = function () {

        var modalScope = VRModalService.showModal('/Client/Modules/Main/Views/UserEditor.html', false);

        modalScope.title = "New User";
        modalScope.onUserAdded = function (user) {
            gridApi.itemAdded(user);
        };
    }

    function loadUsersSearch(name, email) {
        UsersAPIService.SearchUser(name == undefined ? " " : name, email == undefined ? " " : email).then(function (response) {

            $scope.users = response;

        }).finally(function () {

        });
    }
    function load() {
        $scope.isGettingData = true;
        $scope.txtname = '';
        $scope.txtemail = '';
        $scope.loadUsers();
    }


    
});