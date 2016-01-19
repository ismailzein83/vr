(function (appControllers) {

    'use strict';

    BusinessEntityEditorController.$inject = ['$scope', 'VR_Sec_PermissionAPIService', 'UtilsService', 'GroupAPIService', 'VR_Sec_UserAPIService', 'HolderTypeEnum', 'PermissionFlagEnum', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

    function BusinessEntityEditorController($scope, VR_Sec_PermissionAPIService, UtilsService, GroupAPIService, VR_Sec_UserAPIService, HolderTypeEnum, PermissionFlagEnum, VRModalService, VRNotificationService, VRNavigationService) {

        var permissionFlags = [];
        var permissionOptions = [];
        var permissions;
        var holderType;
        var holderId;
        var entityType;
        var entityId;
        var entityName;
        var notificationResponseText;

        defineScope();
        loadParameters();
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

            $scope.isEditMode = false;

            if (permissionFlags != undefined)
                $scope.isEditMode = true;
        }

        function defineScope() {

            $scope.entityPermissions = [];

            $scope.users = [];
            $scope.selectedUsers = [];

            $scope.groups = [];
            $scope.selectedGroups = [];

            $scope.savePermissions = function () {
                var result;
                if ($scope.isEditMode)
                    return updatePermissions();
                else
                    return addPermission();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.isPermissionsFlagSelected = function () {
                return permissionFlags != undefined && permissionFlags.length > 0;
            };

            $scope.permissionsSelected = function (selectedPermission) {

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
        }

        function load() {
            $scope.title = entityName ? UtilsService.buildTitleForUpdateEditor(entityName, 'Permissions') : 'Edit Permissions';
            $scope.isGettingData = true;

            $scope.permissionFlagOptions = UtilsService.getEnumPropertyAsArray(PermissionFlagEnum, 'description');

            if ($scope.isEditMode) {
                angular.forEach(permissionOptions, function (permissionOption) {
                    var entityPermission = {
                        name: permissionOption,
                        selectedFlagOptionIndex: 0
                    };

                    var respectiveFlag = UtilsService.getItemByVal(permissionFlags, permissionOption, 'Name');
                    if (respectiveFlag != null)
                        entityPermission.selectedFlagOptionIndex = respectiveFlag.Value;

                    $scope.entityPermissions.push(entityPermission);
                });

                $scope.isGettingData = false;
            }
            else {
                angular.forEach(permissionOptions, function (permissionOption) {
                    var entityPermission = {
                        name: permissionOption,
                        selectedFlagOptionIndex: 0
                    };

                    $scope.entityPermissions.push(entityPermission);
                });

                UtilsService.waitMultipleAsyncOperations([loadUsers, loadGroups]).then(function () {

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isGettingData = false;
                });
            }
        }

        function loadUsers() {
            return VR_Sec_UserAPIService.GetUsers().then(function (response) {
                //Remove existing users
                angular.forEach(permissions, function (perm) {
                    if (perm.EntityType == entityType && perm.EntityId == entityId
                        && perm.HolderType == HolderTypeEnum.User.value) {
                        var index = UtilsService.getItemIndexByVal(response, perm.HolderId, 'UserId');
                        response.splice(index, 1);
                    }
                });

                $scope.users = response;
            });
        }

        function loadGroups() {
            return GroupAPIService.GetGroups().then(function (response) {
                //Remove existing groups
                angular.forEach(permissions, function (perm) {
                    if (perm.EntityType == entityType && perm.EntityId == entityId
                        && perm.HolderType == HolderTypeEnum.Group.value) {
                        var index = UtilsService.getItemIndexByVal(response, perm.HolderId, 'GroupId');
                        response.splice(index, 1);
                    }
                });

                $scope.groups = response;

            });
        }

        function updatePermissions() {

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
            });
        }

        function addPermission() {
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
                });
        }
    }

    appControllers.controller('VR_Sec_BusinessEntityEditorController', BusinessEntityEditorController);

})(appControllers);
