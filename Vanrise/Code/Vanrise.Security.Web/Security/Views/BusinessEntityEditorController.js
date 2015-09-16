BusinessEntityEditorController.$inject = ['$scope', 'PermissionAPIService', 'UtilsService', 'GroupAPIService', 'UsersAPIService', 'HolderTypeEnum', 'PermissionFlagEnum', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function BusinessEntityEditorController($scope, PermissionAPIService, UtilsService, GroupAPIService, UsersAPIService, HolderTypeEnum, PermissionFlagEnum, VRModalService, VRNotificationService, VRNavigationService) {

    var permissionFlags = [];
    var permissionOptions = [];
    var permissions;
    var holderType;
    var holderId;
    var entityType;
    var entityId;

    defineScope();
    loadParameters();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            holderType = parameters.holderType;
            holderId = parameters.holderId;
            entityType = parameters.entityType;
            entityId = parameters.entityId;
            permissionFlags = parameters.permissionFlags;
            permissionOptions = parameters.permissionOptions;
            permissions = parameters.permissions,
            $scope.notificationResponseText = parameters.notificationResponseText;
        }

        $scope.isEditMode = false;

        if(permissionFlags != undefined)
            $scope.isEditMode = true;
    }

    function defineScope() {

        $scope.entityPermissions = [];

        $scope.users = [];
        $scope.selectedUsers = [];

        $scope.groups = [];
        $scope.selectedGroups = [];

        $scope.SavePermissions = function () {
            var result;
                if ($scope.isEditMode)
                return updatePermissions();
            else
                return addNewPermissions();
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

            var respectiveFlagIndex = UtilsService.getItemIndexByVal(permissionFlags, selectedPermission.name, "Name");
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
            else
            {
                //Update the exisiting one
                permissionFlags[respectiveFlagIndex].Value = selectedPermission.selectedFlagOptionIndex;
            }
        }
    }

    function load() {

        $scope.isGettingData = true;

        $scope.permissionFlagOptions = UtilsService.getEnumPropertyAsArray(PermissionFlagEnum, 'description');

        if($scope.isEditMode) {
            angular.forEach(permissionOptions, function (permissionOption) {
                var entityPermission = {
                    name: permissionOption,
                    selectedFlagOptionIndex: 0
                };

                var respectiveFlag = UtilsService.getItemByVal(permissionFlags, permissionOption, "Name");
                if (respectiveFlag != null)
                    entityPermission.selectedFlagOptionIndex = respectiveFlag.Value;

                $scope.entityPermissions.push(entityPermission);
            });

            $scope.isGettingData = false;
        }
        else
        {
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

    function loadUsers()
    {
        return UsersAPIService.GetUsers().then(function (response) {
            //Remove existing users
            angular.forEach(permissions, function (perm) {
                if (perm.EntityType == entityType && perm.EntityId == entityId
                    && perm.HolderType == HolderTypeEnum.User.value) {
                    var index = UtilsService.getItemIndexByVal(response, perm.HolderId, "UserId");
                    response.splice(index, 1);
                }
            });

            $scope.users = response;
        });
    }

    function loadGroups()
    {
        return GroupAPIService.GetGroups().then(function (response) {
            //Remove existing groups
            angular.forEach(permissions, function (perm) {
                if (perm.EntityType == entityType && perm.EntityId == entityId
                    && perm.HolderType == HolderTypeEnum.Group.value) {
                    var index = UtilsService.getItemIndexByVal(response, perm.HolderId, "GroupId");
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

        return PermissionAPIService.UpdatePermissions(permissions)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated($scope.notificationResponseText, response)) {
                    if ($scope.onPermissionsUpdated != undefined)
                        $scope.onPermissionsUpdated();

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function addNewPermissions()
    {
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
        

        return PermissionAPIService.UpdatePermissions(permissions)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated($scope.notificationResponseText, response)) {
                    if ($scope.onPermissionsAdded != undefined)
                        $scope.onPermissionsAdded();

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

}
appControllers.controller('Security_BusinessEntityEditorController', BusinessEntityEditorController);
