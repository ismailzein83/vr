PermissionEditorController.$inject = ['$scope', 'PermissionAPIService', 'BusinessEntitiesAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function PermissionEditorController($scope, PermissionAPIService, BusinessEntityAPIService, VRModalService, VRNotificationService, VRNavigationService) {

    var treeAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        $scope.holderType = undefined;
        $scope.holderId = undefined;

        if (parameters != undefined && parameters != null)
        {
            $scope.holderType = parameters.holderType;
            $scope.holderId = parameters.holderId;
            $scope.notificationResponseText = parameters.notificationResponseText;
        }
    }

    function defineScope() {
        $scope.SavePermissions = function () {
            if ($scope.permissions != undefined && $scope.permissions != null && $scope.permissions.length > 0)
            {
                $scope.issaving = true;

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
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.beList = [];

        //The temporary permission list that appear next to the tree on node selection1
        $scope.entityPermissions = [];

        //The saved permissions in the data base for a single holder type and id
        $scope.permissions = [];

        $scope.permissionsSelected = function (selectedPermission) {
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
                    HolderType: $scope.holderType,
                    HolderId: $scope.holderId,
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
        };

        $scope.permissionFlagOptions = ['None', 'Allow', 'Deny'];

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode)) {
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
            }
        }
    }

    function load() {
        $scope.isGettingData = true;
            loadTree().finally(function () {
                $scope.isGettingData = false;
            })
        
            $scope.isGettingData = true;

            loadPermissions().finally(function () {
                $scope.isGettingData = false;
            })
    }

    function loadTree() {
        return BusinessEntityAPIService.GetEntityNodes()
           .then(function (response) {
               $scope.beList = response;
               treeAPI.refreshTree($scope.beList);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function loadPermissions() {
        return PermissionAPIService.GetPermissionsByHolder($scope.holderType, $scope.holderId)
           .then(function (response) {
               $scope.permissions = response;
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }
}
appControllers.controller('Security_PermissionEditorController', PermissionEditorController);
