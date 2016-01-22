﻿(function (appControllers) {

    'use strict';

    BusinessEntityEditorController.$inject = ['$scope', 'VR_Sec_PermissionAPIService', 'VRUIUtilsService', 'UtilsService', 'VR_Sec_GroupAPIService', 'VR_Sec_UserAPIService', 'HolderTypeEnum', 'PermissionFlagEnum', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

    function BusinessEntityEditorController($scope, VR_Sec_PermissionAPIService, VRUIUtilsService, UtilsService, VR_Sec_GroupAPIService, VR_Sec_UserAPIService, HolderTypeEnum, PermissionFlagEnum, VRModalService, VRNotificationService, VRNavigationService) {
        // Modal parameters
        var holderType;
        var holderId;
        var entityType;
        var entityId;
        var entityName;
        var permissionFlags;
        var permissionOptions;
        var permissions;
        var notificationResponseText;

        // Directive vars
        var userSelectorAPI;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var groupSelectorAPI;
        var groupSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                holderType = parameters.holderType;
                holderId = parameters.holderId;
                entityType = parameters.entityType;
                entityId = parameters.entityId;
                entityName = parameters.entityName;
                permissionFlags = parameters.permissionFlags;
                permissionOptions = parameters.permissionOptions;
                permissions = parameters.permissions;
                notificationResponseText = parameters.notificationResponseText;
            }

            $scope.isEditMode = (permissionFlags != undefined);
        }

        function defineScope() {
            $scope.selectedUsers = [];
            $scope.selectedGroups = [];

            $scope.entityPermissions = [];
            $scope.permissionFlagOptions = UtilsService.getEnumPropertyAsArray(PermissionFlagEnum, 'description');

            $scope.onUserSelectorReady = function (api) {
                userSelectorAPI = api;
                userSelectorReadyDeferred.resolve();
            };

            $scope.onGroupSelectorReady = function (api) {
                groupSelectorAPI = api;
                groupSelectorReadyDeferred.resolve();
            };

            $scope.onPermissionSelectionChanged = function (selectedPermission) {
                if (permissionFlags == undefined)
                    permissionFlags = [];

                var respectiveFlagIndex = UtilsService.getItemIndexByVal(permissionFlags, selectedPermission.name, 'Name');
                if (selectedPermission.selectedFlagOptionIndex == 0) {
                    //Permission flag is set back to None and needs to be removed from the list of permission flags
                    permissionFlags.splice(respectiveFlagIndex, 1);
                }
                else if (respectiveFlagIndex == -1) {
                    //the permission is not found in permission flags array and needs to be added
                    var permissionFlagtoAdd = {
                        Name: selectedPermission.name,
                        Value: selectedPermission.selectedFlagOptionIndex
                    };
                    permissionFlags.push(permissionFlagtoAdd);
                }
                else {
                    //Update the exisiting one
                    permissionFlags[respectiveFlagIndex].Value = selectedPermission.selectedFlagOptionIndex;
                }
            }

            $scope.save = function () {
                var result;
                if ($scope.isEditMode)
                    return updatePermissions();
                else
                    return addPermissions();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.validateSelectors = function () {
                if ($scope.isEditMode) return null;
                return (userSelectorAPI.getSelectedIds() != undefined || groupSelectorAPI.getSelectedIds() != undefined) ? null : 'No user(s) / group(s) are selected';
            };

            $scope.validateAssignedPermissions = function () {
                return (permissionFlags && permissionFlags.length > 0) ? null : 'No permissions are assigned';
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadUserSelector, loadGroupSelector, loadAssignedPermissions]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = $scope.isEditMode ? UtilsService.buildTitleForUpdateEditor(entityName, 'Permissions') : UtilsService.buildTitleForAddEditor('Permissions');
            }

            function loadUserSelector() {
                var loadUserSelectorDeferred = UtilsService.createPromiseDeferred();

                userSelectorReadyDeferred.promise.then(function () {
                    if ($scope.isEditMode) {
                        loadUserSelectorDeferred.resolve();
                    }
                    else {
                        var userSelectorPayload = {};

                        userSelectorPayload.filter = $scope.isEditMode ? null : {
                            EntityType: entityType,
                            EntityId: entityId
                        };

                        VRUIUtilsService.callDirectiveLoad(userSelectorAPI, userSelectorPayload, loadUserSelectorDeferred);
                    }
                });
                
                return loadUserSelectorDeferred.promise;
            }

            function loadGroupSelector() {
                var loadGroupSelectorDeferred = UtilsService.createPromiseDeferred();

                groupSelectorReadyDeferred.promise.then(function () {
                    if ($scope.isEditMode) {
                        loadGroupSelectorDeferred.resolve();
                    }
                    else {
                        var groupSelectorPayload = {};

                        groupSelectorPayload.filter = $scope.isEditMode ? null : {
                            EntityType: entityType,
                            EntityId: entityId
                        };

                        VRUIUtilsService.callDirectiveLoad(groupSelectorAPI, groupSelectorPayload, loadGroupSelectorDeferred);
                    }
                });
                
                return loadGroupSelectorDeferred.promise;
            }

            function loadAssignedPermissions() {
                if (!permissionOptions)
                    return;

                if ($scope.isEditMode) {
                    for (var i = 0; i < permissionOptions.length; i++) {
                        var entityPermission = {
                            name: permissionOptions[i],
                            selectedFlagOptionIndex: 0
                        };

                        var respectiveFlag = UtilsService.getItemByVal(permissionFlags, permissionOptions[i], 'Name');
                        if (respectiveFlag != null)
                            entityPermission.selectedFlagOptionIndex = respectiveFlag.Value;

                        $scope.entityPermissions.push(entityPermission);
                    }
                }
                else {
                    for (var i = 0; i < permissionOptions.length; i++) {
                        var entityPermission = {
                            name: permissionOptions[i],
                            selectedFlagOptionIndex: 0
                        };

                        $scope.entityPermissions.push(entityPermission);
                    }
                }
            }
        }

        function addPermissions() {
            $scope.isLoading = true;
            var permissions = [];

            //Loop on all selected users
            angular.forEach($scope.selectedUsers, function (user) {
                var permissiontoAdd = {
                    HolderType: HolderTypeEnum.User.value,
                    HolderId: user.UserId,
                    EntityType: entityType,
                    EntityId: entityId,
                    PermissionFlags: permissionFlags
                };

                permissions.push(permissiontoAdd);

            });

            //Loop again on all selected groups
            angular.forEach($scope.selectedGroups, function (group) {
                var permissiontoAdd = {
                    HolderType: HolderTypeEnum.Group.value,
                    HolderId: group.GroupId,
                    EntityType: entityType,
                    EntityId: entityId,
                    PermissionFlags: permissionFlags
                };

                permissions.push(permissiontoAdd);

            });

            return VR_Sec_PermissionAPIService.UpdatePermissions(permissions).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated(notificationResponseText, response)) {

                    if ($scope.onPermissionAdded && typeof $scope.onPermissionAdded == 'function') {
                        $scope.onPermissionAdded();
                    }

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function updatePermissions() {
            $scope.isLoading = true;

            var permissiontoUpdate = {
                HolderType: holderType,
                HolderId: holderId,
                EntityType: entityType,
                EntityId: entityId,
                PermissionFlags: permissionFlags
            };

            var permissions = [];
            permissions.push(permissiontoUpdate);

            return VR_Sec_PermissionAPIService.UpdatePermissions(permissions).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated(notificationResponseText, response)) {
                    if ($scope.onPermissionsUpdated)
                        $scope.onPermissionsUpdated();

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_BusinessEntityEditorController', BusinessEntityEditorController);

})(appControllers);
