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
            var gridQuery;
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
                            gridQuery = query;
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

                $scope.gridMenuActions = function (dataItem) {
                   
                    var menuActions = [
                        {
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
                        }
                    ];

                    if (dataItem.Status) {
                        var menuAction1 = {
                            name: "Disable",
                            clicked: disableUser,
                            haspermission: hasDisableSystemEntityPermissionsPermission
                        };
                        menuActions.push(menuAction1);
                    } else {
                        var menuAction2 = {
                            name: "Enable",
                            clicked: enableUser,
                            haspermission: hasEnableSystemEntityPermissionsPermission
                        };
                        menuActions.push(menuAction2);
                    }
                        return menuActions;
                 
                };
                
            }

            function hasUpdateUserPermission() {
                return VR_Sec_UserAPIService.HasUpdateUserPermission();
            }
            function hasResetUserPasswordPermission() {
                return VR_Sec_UserAPIService.HasResetUserPasswordPermission();
            }
            function hasUpdateSystemEntityPermissionsPermission() {
                return VR_Sec_PermissionAPIService.HasUpdatePermissionsPermission();
            }
            function hasDisableSystemEntityPermissionsPermission() {
                return VR_Sec_PermissionAPIService.HasDisablePermission();
            }
            function hasEnableSystemEntityPermissionsPermission() {
                return VR_Sec_PermissionAPIService.HasEnablePermission();
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

            function disableUser(dataItem) {
                var onPermissionDisabled = function (entity) {
                    var gridDataItem = { Entity: entity };
                    gridDataItem.Status = false;
                    $scope.gridMenuActions(gridDataItem);
                    gridAPI.itemUpdated(gridDataItem);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return VR_Sec_UserAPIService.DisableUser(dataItem.Entity).then(function(response) {
                            if (onPermissionDisabled && typeof onPermissionDisabled == 'function') {
                                dataItem.Entity.EnabledTill = response.UpdatedObject.Entity.EnabledTill;
                                onPermissionDisabled(dataItem.Entity);
                            }
                        }).catch(function(error) {
                            VRNotificationService.notifyException(error, scope);
                        });
                    }
                });
            }

            function enableUser(dataItem) {
                var onPermissionEnabled = function (entity) {
                    var gridDataItem = { Entity: entity };
                    gridDataItem.Status = true;
                    $scope.gridMenuActions(gridDataItem);
                    gridAPI.itemUpdated(gridDataItem);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return VR_Sec_UserAPIService.EnableUser(dataItem.Entity).then(function (response) {
                            if (onPermissionEnabled && typeof onPermissionEnabled == 'function') {
                                dataItem.Entity.EnabledTill = response.UpdatedObject.Entity.EnabledTill;
                                onPermissionEnabled(dataItem.Entity);
                            }
                        }).catch(function(error) {
                            VRNotificationService.notifyException(error, scope);
                        });
                    }
                });
            }
        }

        return directiveDefinitionObject;

    }]);