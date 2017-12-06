"use strict";

app.directive("vrSecUserGrid", ["VR_Sec_UserAPIService", "VR_Sec_UserService", 'VRUIUtilsService', 'VR_Sec_PermissionAPIService', "VR_Sec_PermissionService", "VR_Sec_HolderTypeEnum", 'VRNotificationService', 'VR_Sec_SecurityAPIService', 'VR_Sec_UserActivationStatusEnum',
    function (VR_Sec_UserAPIService, VR_Sec_UserService, VRUIUtilsService, VR_Sec_PermissionAPIService, VR_Sec_PermissionService, VR_Sec_HolderTypeEnum, VRNotificationService, VR_Sec_SecurityAPIService, VR_Sec_UserActivationStatusEnum) {

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
            var gridDrillDownTabsObj;
            var gridAPI;
            var gridQuery;
            var hasAuthServer;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.users = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Sec_UserService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

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
                            gridDrillDownTabsObj.setDrillDownExtensionObject(userObject);
                            gridAPI.itemAdded(userObject);
                        };

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Sec_UserAPIService.GetFilteredUsers(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }
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

                    var menuAction;
                    switch (dataItem.Status) {
                        case VR_Sec_UserActivationStatusEnum.Inactive.value:
                            menuAction = {
                                name: "Enable",
                                clicked: enableUser,
                                haspermission: hasEnableSystemEntityPermissionsPermission
                            };
                            break;
                        case VR_Sec_UserActivationStatusEnum.Active.value:
                            menuAction = {
                                name: "Disable",
                                clicked: disableUser,
                                haspermission: hasDisableSystemEntityPermissionsPermission
                            };
                            break;
                        case VR_Sec_UserActivationStatusEnum.Locked.value:
                            menuAction = {
                                name: "Unlock",
                                clicked: unlockUser,
                                haspermission: hasUnlockSystemEntityPermissionsPermission
                            };
                            break;
                    }

                    menuActions.push(menuAction);


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
                return VR_Sec_UserAPIService.HasResetUserPasswordPermission();
            }
            function hasUnlockSystemEntityPermissionsPermission() {
                return VR_Sec_UserAPIService.HasResetUserPasswordPermission();
            }
            function hasEnableSystemEntityPermissionsPermission() {
                return VR_Sec_UserAPIService.HasResetUserPasswordPermission();
            }


            function editUser(userObj) {
                var onUserUpdated = function (userObj) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(userObj);
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
                    gridDataItem.Status = VR_Sec_UserActivationStatusEnum.Inactive.value;
                    gridDrillDownTabsObj.setDrillDownExtensionObject(gridDataItem);
                    $scope.gridMenuActions(gridDataItem);
                    gridAPI.itemUpdated(gridDataItem);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return VR_Sec_UserAPIService.DisableUser(dataItem.Entity).then(function (response) {
                            if (onPermissionDisabled && typeof onPermissionDisabled == 'function') {
                                dataItem.Entity.EnabledTill = response.UpdatedObject.Entity.EnabledTill;
                                onPermissionDisabled(dataItem.Entity);
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, scope);
                        });
                    }
                });
            }

            function unlockUser(dataItem) {
                var onPermissionUnlocked = function (entity) {
                    var gridDataItem = { Entity: entity };
                    gridDataItem.Status = VR_Sec_UserActivationStatusEnum.Active.value;
                    gridDrillDownTabsObj.setDrillDownExtensionObject(gridDataItem);
                    $scope.gridMenuActions(gridDataItem);
                    gridAPI.itemUpdated(gridDataItem);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return VR_Sec_UserAPIService.UnlockUser(dataItem.Entity).then(function (response) {
                            if (onPermissionUnlocked && typeof onPermissionUnlocked == 'function') {
                                dataItem.Entity.EnabledTill = response.UpdatedObject.Entity.EnabledTill;
                                onPermissionUnlocked(dataItem.Entity);
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, scope);
                        });
                    }
                });
            }

            function enableUser(dataItem) {
                var onPermissionEnabled = function (entity) {
                    var gridDataItem = { Entity: entity };
                    gridDataItem.Status = VR_Sec_UserActivationStatusEnum.Active.value;
                    gridDrillDownTabsObj.setDrillDownExtensionObject(gridDataItem);

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
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, scope);
                        });
                    }
                });
            }
        }

        return directiveDefinitionObject;

    }]);