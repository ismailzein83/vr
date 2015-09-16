PermissionEditorController.$inject = ['$scope', 'PermissionFlagEnum', 'PermissionAPIService', 'BusinessEntitiesAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function PermissionEditorController($scope, PermissionFlagEnum, PermissionAPIService, BusinessEntityAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

    var treeAPI;
    var holderType;
    var holderId;
    defineScope();
    loadParameters();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
        {
            holderType = parameters.holderType;
            holderId = parameters.holderId;
            $scope.notificationResponseText = parameters.notificationResponseText;
        }
    }

    function defineScope() {

        $scope.beList = [];

        //The temporary permission list that appear next to the tree on node selection1
        $scope.entityPermissions = [];

        //The saved permissions in the data base for a single holder type and id
        $scope.permissions = [];

        $scope.SavePermissions = savePermissions;

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.permissionsSelected = onPermissionsSelected;

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = treeValueChanged;
    }

    function savePermissions() {
        if ($scope.permissions != undefined && $scope.permissions != null && $scope.permissions.length > 0)
        {
            var permissionObject = [];

            angular.forEach($scope.permissions, function (permission) {
                if (permission.isDirty)
                    permissionObject.push(permission);
            });

            if (permissionObject.length > 0)
            {
                return PermissionAPIService.UpdatePermissions(permissionObject)
               .then(function (response) {
                   if (VRNotificationService.notifyOnItemUpdated($scope.notificationResponseText, response)) {
                       $scope.modalContext.closeModal();
                   }
               }).catch(function (error) {
                   VRNotificationService.notifyException(error, $scope);
               });
            }
        }
    }

    function onPermissionsSelected(selectedPermission) {
        var needforAddPermission = true;
        var needforAddPermissionFlag = true;
        angular.forEach($scope.permissions, function (permission) {
            if (permission.EntityType == $scope.currentNode.EntType &&
                permission.EntityId == $scope.currentNode.EntityId) {

                //Permission is found and no need to add it to the list of permission that we need to send to the server
                needforAddPermission = false;
                var indexofItemtoRemove = undefined;

                permission.isDirty = true;

                angular.forEach(permission.PermissionFlags, function (permissionFlag, index) {
                    if (permissionFlag.Name == selectedPermission.name) {
                        needforAddPermissionFlag = false;
                        if (selectedPermission.selectedFlagOptionIndex == 0)
                        {
                            //Permission flag is set back to None and needs to be removed from the list of permission flags
                            indexofItemtoRemove = index;
                        }
                        else
                        {
                            //Permission Flag is found in the list and no need to add a new one
                            permissionFlag.Value = selectedPermission.selectedFlagOptionIndex;
                        }
                    }
                });

                if (indexofItemtoRemove != undefined)
                {
                    permission.PermissionFlags.splice(indexofItemtoRemove, 1);
                }
                else if (selectedPermission.selectedFlagOptionIndex != 0 && needforAddPermissionFlag)
                {
                    var permissionFlagtoAdd = {
                        Name: selectedPermission.name,
                        Value: selectedPermission.selectedFlagOptionIndex
                    };
                    permission.PermissionFlags.push(permissionFlagtoAdd);
                }
            }
        });

        if(selectedPermission.selectedFlagOptionIndex != 0 && needforAddPermission)
        {
            var permissiontoAdd = {
                HolderType: holderType,
                HolderId: holderId,
                EntityType: $scope.currentNode.EntType,
                EntityId: $scope.currentNode.EntityId
            };

            permissiontoAdd.PermissionFlags = [];

            var permissionFlagtoAdd = {
                Name: selectedPermission.name,
                Value: selectedPermission.selectedFlagOptionIndex
            };
            permissiontoAdd.PermissionFlags.push(permissionFlagtoAdd);
            permissiontoAdd.isDirty = true;
            $scope.permissions.push(permissiontoAdd);
        }
    }

    function treeValueChanged()
    {
        if (angular.isObject($scope.currentNode)) {
            $scope.isLoadingPermissions = true;

            $scope.entityPermissions.length = 0;

            angular.forEach($scope.currentNode.PermissionOptions, function (permissionOption) {
                var entityPermission = {
                    name: permissionOption,
                    selectedFlagOptionIndex: 0
                };

                angular.forEach($scope.permissions, function (permission) {
                    if (permission.EntityType == $scope.currentNode.EntType &&
                        permission.EntityId == $scope.currentNode.EntityId) {

                        angular.forEach(permission.PermissionFlags, function (permissionFlag) {
                            if (permissionFlag.Name == permissionOption) {
                                entityPermission.selectedFlagOptionIndex = permissionFlag.Value;
                            }
                        });
                    }
                });

                $scope.entityPermissions.push(entityPermission);
            });

            $scope.isLoadingPermissions = false;
        }
    }

    function load() {
        $scope.isGettingData = true;

        $scope.permissionFlagOptions = UtilsService.getEnumPropertyAsArray(PermissionFlagEnum, 'description');

        UtilsService.waitMultipleAsyncOperations([loadTree, loadPermissions])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
    }

    function loadTree() {
        return BusinessEntityAPIService.GetEntityNodes()
            .then(function (response) {
                $scope.beList = response;
                angular.forEach($scope.beList, function (item) {
                    item.isOpened = true;
                });
                treeAPI.refreshTree($scope.beList);
            });
    }

    function loadPermissions() {
        return PermissionAPIService.GetPermissionsByHolder(holderType, holderId)
            .then(function (response) {
                $scope.permissions = response;
            });
    }
}

appControllers.controller('Security_PermissionEditorController', PermissionEditorController);
