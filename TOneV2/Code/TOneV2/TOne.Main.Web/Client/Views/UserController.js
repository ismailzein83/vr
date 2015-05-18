/// <reference path="UserSettings.html" />
/// <reference path="User.html" />
appControllers.controller('UserController', function UserController($scope, UsersAPIService, $modal) {
    $scope.loadUsers = function () {
        UsersAPIService.GetUserList().then(function (response) {
            $scope.users = response;
        }).finally(function () {

            $scope.isGettingData = false;
        });
    }

    load();

    $scope.gridMenuActions = [{
        name: "Edit",
        clicked: function (dataItem) {
            var scopeDetails = $scope.$root.$new();
            scopeDetails.title = "Edit User";
            scopeDetails.user = dataItem;
            var addModal = $modal({ scope: scopeDetails, template: '/Client/Modules/Main/Views/UserEditor.html', show: true, animation: "am-fade-and-scale" });
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

    //Action
    $scope.SaveUser = function (user) {

        UsersAPIService.SaveUser(user).then(function (response) {

        }).finally(function () {
            loadUsersSearch($scope.txtname, $scope.txtemail);
        });
    }
    
    //Action
    $scope.SearchUser = function () {
        loadUsersSearch($scope.txtname, $scope.txtemail);
    }

    $scope.ValidateUser = function (text) {
        if(text.length < 3)
            return "Invalid";
    }

    $scope.AddNewUser = function () {

        var scopeDetails = $scope.$root.$new();
        scopeDetails.title = "New User";
        scopeDetails.callBack = $scope.loadUsers;
        scopeDetails.grid = gridApi;
        var addModal = $modal({ scope: scopeDetails, template: '/Client/Modules/Main/Views/UserEditor.html', show: true, animation: "am-fade-and-scale" });
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

    var gridApi;
    $scope.gridReady = function (api) {
        gridApi = api;
    };

    $scope.loadMoreData = function (asyncHandle) {
        BusinessEntityAPIService.GetCodeGroups().then(function (response) {
            var count = current + 20;
            for (current; current < count; current++) {
                $scope.gridData.push({
                    col1: "test " + current + "1",
                    col2: "test " + current + "2",
                    col3: "test " + current + "3",
                });
            }


        })
            .finally(function () {
                if (asyncHandle)
                    asyncHandle.operationDone();
            });
        //setTimeout(function () {
        //    $scope.$apply(function () {


        //    });

        //}, 2000);

    };
    $scope.loadMoreData();
    
});