BusinessEntityEditorController.$inject = ['$scope', 'PermissionAPIService', 'UtilsService', 'GroupAPIService', 'UsersAPIService', 'HolderTypeEnum', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function BusinessEntityEditorController($scope, PermissionAPIService, UtilsService, GroupAPIService, UsersAPIService, HolderTypeEnum, VRModalService, VRNotificationService, VRNavigationService) {

    var permissionFlags;
    var permissions;
    //var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.holderType = undefined;
        $scope.holderId = undefined;
        $scope.entityType = undefined;
        $scope.entityId = undefined;
        $scope.permissionFlags = undefined;
        $scope.permissionOptions = undefined;

        if (parameters != undefined && parameters != null) {
            $scope.holderType = parameters.holderType;
            $scope.holderId = parameters.holderId;
            $scope.entityType = parameters.entityType;
            $scope.entityId = parameters.entityId;
            permissionFlags = parameters.permissionFlags;
            $scope.permissionOptions = parameters.permissionOptions;
            permissions = parameters.permissions,
            $scope.notificationResponseText = parameters.notificationResponseText;
        }

        $scope.isEditMode = false;

        if(permissionFlags != undefined)
            $scope.isEditMode = true;
    }

    function defineScope() {
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

        $scope.entityPermissions = [];

        //The saved permissions in the data base for a single holder type and id
        $scope.permissions = [];

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

        $scope.permissionFlagOptions = ['None', 'Allow', 'Deny'];

        $scope.optionsUsers = {
            selectedvalues: [],
            datasource: []
        };

        $scope.optionsGroups = {
            selectedvalues: [],
            datasource: []
        };
    }

    function load() {

        $scope.isGettingData = true;

        if($scope.isEditMode) {
            angular.forEach($scope.permissionOptions, function (permissionOption) {
                var entityPermission = {
                    name: permissionOption,
                    selectedFlagOptionIndex: 0
                };

                var respectiveFlag = UtilsService.getItemByVal(permissionFlags, permissionOption, "Name");
                if (respectiveFlag != null)
                    entityPermission.selectedFlagOptionIndex = respectiveFlag.Value;

                $scope.entityPermissions.push(entityPermission);
            });
        }
        else
        {
            angular.forEach($scope.permissionOptions, function (permissionOption) {
                var entityPermission = {
                    name: permissionOption,
                    selectedFlagOptionIndex: 0
                };

                $scope.entityPermissions.push(entityPermission);
            });

            UsersAPIService.GetUsers().then(function (response) {
                //Remove existing users
                angular.forEach(permissions, function (perm) {
                    if (perm.EntityType == $scope.entityType && perm.EntityId == $scope.entityId
                        && perm.HolderType == HolderTypeEnum.User.value)
                    {
                        var index = UtilsService.getItemIndexByVal(response, perm.HolderId, "UserId");
                        response.splice(index, 1);
                    }
                });

                $scope.optionsUsers.datasource = response;
            });

            GroupAPIService.GetGroups().then(function (response) {
                //Remove existing groups
                angular.forEach(permissions, function (perm) {
                    if (perm.EntityType == $scope.entityType && perm.EntityId == $scope.entityId
                        && perm.HolderType == HolderTypeEnum.Group.value) {
                        var index = UtilsService.getItemIndexByVal(response, perm.HolderId, "GroupId");
                        response.splice(index, 1);
                    }
                });

                $scope.optionsGroups.datasource = response;

            });
        }

        $scope.isGettingData = false;
    }

    function updatePermissions() {

        var permissiontoUpdate = {
            HolderType: $scope.holderType,
            HolderId: $scope.holderId,
            EntityType: $scope.entityType,
            EntityId: $scope.entityId,
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
        angular.forEach($scope.optionsUsers.selectedvalues, function (user) {
            var permissiontoAdd = {
                HolderType: HolderTypeEnum.User.value,
                HolderId: user.UserId,
                EntityType: $scope.entityType,
                EntityId: $scope.entityId,
                PermissionFlags: permissionFlags
            };

            permissions.push(permissiontoAdd);

        });

        //Loop again on all selected groups
        angular.forEach($scope.optionsGroups.selectedvalues, function (group) {
            var permissiontoAdd = {
                HolderType: HolderTypeEnum.Group.value,
                HolderId: group.GroupId,
                EntityType: $scope.entityType,
                EntityId: $scope.entityId,
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
