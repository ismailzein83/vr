/// <reference path="UserSettings.html" />
/// <reference path="User.html" />
appControllers.controller('UserManagementController', function UserController($scope, UsersAPIService, VRModalService) {
    $scope.users = [];
    
    $scope.gridMenuActions = [{
        name: "Edit",
        clicked: function (dataItem) {

            var settings = {
                width: "40%"
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit User";
                modalScope.onUserAdded = function (user) {
                    gridApi.itemUpdated(user);
                };
            };

            VRModalService.showModal('/Client/Modules/Main/Views/UserEditor.html',  dataItem, settings);
        }
    },
    {
            name: "Reset Password",
            clicked: function (dataItem) {

                var settings = {
                    width: "40%"
                };

                settings.onScopeReady = function (modalScope) {
                    modalScope.title = "Reset Pasword";
                    modalScope.onUserAdded = function (user) {
                        gridApi.itemUpdated(user);
                    };
                };

                VRModalService.showModal('/Client/Modules/Main/Views/ResetPasswordEditor.html', dataItem, settings);
            }
    }
    //{
    //    name: "Delete",
    //    clicked: function (dataItem) {
    //        $scope.DeleteUser(dataItem);
    //    }
    //}
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

    var pageSize = 20;
    var from = 0;
    var to = pageSize;
    var last = false;

    $scope.loadMoreData = function (asyncHandle) {
       
        if (last) {
            if (asyncHandle)
                asyncHandle.operationDone();
            return;
        }
        var params = {};
        if (from == 0) params.fromRow = 0;
        else params.fromRow =  from + 1;
        params.toRow = to;
        
        from = from + pageSize ;
        to = to + pageSize;

        UsersAPIService.GetUserList(params).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.users.push(itm);
            });
            
            last = (response.length < pageSize ) ? true : false;

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

    $scope.ValidateUs1 = function (text) {
        if (text == undefined)
            return null;
        if(text.length < 3)
            return "Invalid";
    }

    $scope.AddNewUser = function () {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New User";
            modalScope.onUserAdded = function (user) {
                gridApi.itemAdded(user);
            };
        };
        VRModalService.showModal('/Client/Modules/Main/Views/UserEditor.html', null, settings);
        
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