"use strict";

app.directive("vrSecUserGrid", ["VR_Sec_UserAPIService", "VR_Sec_UserService", 'VR_Sec_PermissionAPIService', "VR_Sec_PermissionService", "VR_Sec_HolderTypeEnum", 'VRNotificationService', 'VR_Sec_SecurityAPIService',
    function (VR_Sec_UserAPIService, VR_Sec_UserService, VR_Sec_PermissionAPIService, VR_Sec_PermissionService, VR_Sec_HolderTypeEnum, VRNotificationService, VR_Sec_SecurityAPIService) {

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
            var hasAuthServer;
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
                            return VR_Sec_SecurityAPIService.HasAuthServer().then(function (response) {
                                hasAuthServer = response;
                                gridAPI.retrieveData(query);
                            });
                        };

                        directiveAPI.onUserAdded = function (userObject) {
                            gridAPI.itemAdded(userObject);
                        };

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
                    haspermission: hasUpdateUserPermission
                }, {
                    name: "Reset Password",
                    clicked: resetPassword,
                    haspermission: hasResetUserPasswordPermission
                }, {
                    name: "Assign Permissions",
                    clicked: assignPermissions,
                    haspermission: hasUpdateSystemEntityPermissionsPermission // System Entities:Assign Permissions
                }];

                function hasUpdateUserPermission() {
                    return VR_Sec_UserAPIService.HasUpdateUserPermission();
                }
                function hasResetUserPasswordPermission() {
                    return VR_Sec_UserAPIService.HasResetUserPasswordPermission();
                }
                function hasUpdateSystemEntityPermissionsPermission() {
                    return VR_Sec_PermissionAPIService.HasUpdatePermissionsPermission();
                }
            }

            function editUser(userObj) {
                var onUserUpdated = function (userObj) {
                    gridAPI.itemUpdated(userObj);
                };

                VR_Sec_UserService.editUser(userObj.Entity.UserId, onUserUpdated);
            }

            function resetPassword(userObj) {
                if (hasAuthServer) {
                    VR_Sec_UserService.resetAuthServerPassword($scope, userObj.Entity.UserId);
                } else {
                    VR_Sec_UserService.resetPassword(userObj.Entity.UserId);
                }
            }

            function assignPermissions(userObj) {
                VR_Sec_PermissionService.assignPermissions(VR_Sec_HolderTypeEnum.User.value, userObj.Entity.UserId);
            }
        }

        return directiveDefinitionObject;

    }]);