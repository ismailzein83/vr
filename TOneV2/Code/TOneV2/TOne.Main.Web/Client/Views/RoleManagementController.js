/// <reference path="RoleSettings.html" />
/// <reference path="Role.html" />
appControllers.controller('RoleManagementController', function RoleController($scope, $q, RolesAPIService, VRModalService) {
    $scope.Roles = [];

    $scope.gridMenuActions = [{
        name: "Edit",
        clicked: function (dataItem) {

            var settings = {
                width: "40%"
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Role";
                modalScope.onRoleAdded = function (Role) {
                    gridApi.itemUpdated(Role);
                };
            };

            VRModalService.showModal('/Client/Modules/Main/Views/RoleEditor.html', dataItem, settings);
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
                modalScope.onRoleAdded = function (Role) {
                    gridApi.itemUpdated(Role);
                };
            };

            VRModalService.showModal('/Client/Modules/Main/Views/ResetPasswordEditor.html', dataItem, settings);
        }
    },
    {
        name: "Roles",
        clicked: function (dataItem) {

            var settings = {
                width: "40%"
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Roles";
                modalScope.onRoleAdded = function (Role) {
                    gridApi.itemUpdated(Role);
                };
            };

            VRModalService.showModal('/Client/Modules/Main/Views/RolesEditor.html', dataItem, settings);
        }
    }
    //{
    //    name: "Delete",
    //    clicked: function (dataItem) {
    //        $scope.DeleteRole(dataItem);
    //    }
    //}
    ];

    //Action
    $scope.DeleteRole = function (Role) {

        RolesAPIService.DeleteRole(Role.RoleId).then(function (response) {

        }).finally(function () {
            loadRolesSearch($scope.txtname, $scope.txtemail);
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

    $scope.loadMoreData = function () {

        var params = {};
        if (from == 0) params.fromRow = 0;
        else params.fromRow = from + 1;
        params.toRow = to;

        from = from + pageSize;
        to = to + pageSize;

        return RolesAPIService.GetRoleList(params).then(function (response) {

            angular.forEach(response, function (itm) {
                $scope.Roles.push(itm);
            });

            last = (response.length < pageSize) ? true : false;

        });
    };
    $scope.loadMoreData();

    //Action
    $scope.SearchRole = function () {
        loadRolesSearch($scope.txtname, $scope.txtemail);
    }

    $scope.ValidateUs1 = function (text) {
        if (text == undefined)
            return null;
        if (text.length < 3)
            return "Invalid";
    }

    $scope.AddNewRole = function () {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Role";
            modalScope.onRoleAdded = function (Role) {
                gridApi.itemAdded(Role);
            };
        };
        VRModalService.showModal('/Client/Modules/Main/Views/RoleEditor.html', null, settings);

    }

    function loadRolesSearch(name, email) {
        RolesAPIService.SearchRole(name == undefined ? " " : name, email == undefined ? " " : email).then(function (response) {

            $scope.Roles = response;

        }).finally(function () {

        });
    }

    function load() {
        $scope.isGettingData = true;
        $scope.txtname = '';
        $scope.txtemail = '';
        $scope.loadRoles();
    }

});