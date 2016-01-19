﻿"use strict";

app.directive("vrSecUserGrid", ["VRNotificationService", "VR_Sec_UserAPIService", "VR_Sec_UserService",
function (VRNotificationService, VR_Sec_UserAPIService, VR_Sec_UserService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var userGrid = new UsersGrid($scope, ctrl, $attrs);
            userGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/User/Templates/UsersGrid.html"

    };

    function UsersGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.users = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onUserAdded = function (userObject) {
                        gridAPI.itemAdded(userObject);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_UserAPIService.GetFilteredUsers(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
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

        function editUser(userObj) {
            var onUserUpdated = function (userObj) {
                gridAPI.itemUpdated(userObj);
            }

            VR_Sec_UserService.editUser(userObj.Entity.UserId, onUserUpdated);
        }

        function resetPassword(userObj) {
            VR_Sec_UserService.resetPassword(userObj.Entity.UserId);
        }

        function assignPermissions(userObj) {
            VR_Sec_UserService.assignPermissions(userObj.Entity.UserId);
        }
    }

    return directiveDefinitionObject;

}]);