(function (appControllers) {

    'use strict';

    PermissionEditorController.$inject = ['$scope', 'PermissionFlagEnum', 'VR_Sec_PermissionAPIService', 'VR_Sec_BusinessEntityAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

    function PermissionEditorController($scope, PermissionFlagEnum, VR_Sec_PermissionAPIService, VR_Sec_BusinessEntityAPIService, UtilsService, VRModalService, VRNotificationService, VRNavigationService) {

        var holderType;
        var holderId;

        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                holderType = parameters.holderType;
                holderId = parameters.holderId;
                $scope.notificationResponseText = parameters.notificationResponseText;
            }
        }

        function defineScope() {
            $scope.beList = [];
            $scope.entityPermissions = []; // The temporary permission list that appear next to the tree on node selection1
            $scope.permissions = []; // The saved permissions in the data base for a single holder type and id
            $scope.permissionFlagOptions = UtilsService.getEnumPropertyAsArray(PermissionFlagEnum, 'description');

            $scope.save = savePermissions;

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.permissionsSelected = onPermissionsSelected;

            $scope.onTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            }

            $scope.treeValueChanged = treeValueChanged;
        }

        function savePermissions() {
            if ($scope.permissions) {
                var permissionObject = [];

                for (var i = 0; i < $scope.permissions.length; i++) {
                    if ($scope.permissions[i].isDirty) {
                        permissionObject.push($scope.permissions[i]);
                    }
                }

                if (permissionObject.length > 0) {
                    return VR_Sec_PermissionAPIService.UpdatePermissions(permissionObject).then(function (response) {
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

            if ($scope.permissions) {
                for (var i = 0; i < $scope.permissions.length; i++) {
                    var permission = $scope.permissions[i];

                    if (permission.EntityType == $scope.currentNode.EntType && permission.EntityId == $scope.currentNode.EntityId) {
                        //Permission is found and no need to add it to the list of permission that we need to send to the server
                        needforAddPermission = false;
                        var indexofItemtoRemove = undefined;

                        permission.isDirty = true;

                        angular.forEach(permission.PermissionFlags, function (permissionFlag, index) {
                            if (permissionFlag.Name == selectedPermission.name) {
                                needforAddPermissionFlag = false;
                                if (selectedPermission.selectedFlagOptionIndex == 0) {
                                    //Permission flag is set back to None and needs to be removed from the list of permission flags
                                    indexofItemtoRemove = index;
                                }
                                else {
                                    //Permission Flag is found in the list and no need to add a new one
                                    permissionFlag.Value = selectedPermission.selectedFlagOptionIndex;
                                }
                            }
                        });

                        if (indexofItemtoRemove != undefined) {
                            permission.PermissionFlags.splice(indexofItemtoRemove, 1);
                        }
                        else if (selectedPermission.selectedFlagOptionIndex != 0 && needforAddPermissionFlag) {
                            var permissionFlagtoAdd = {
                                Name: selectedPermission.name,
                                Value: selectedPermission.selectedFlagOptionIndex
                            };
                            permission.PermissionFlags.push(permissionFlagtoAdd);
                        }
                    }
                }
            }

            if (selectedPermission.selectedFlagOptionIndex != 0 && needforAddPermission) {
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

        function treeValueChanged() {
            if (angular.isObject($scope.currentNode)) {
                $scope.isLoadingPermissions = true;

                $scope.entityPermissions.length = 0;

                angular.forEach($scope.currentNode.PermissionOptions, function (permissionOption) {
                    var entityPermission = {
                        name: permissionOption,
                        selectedFlagOptionIndex: 0
                    };
                    
                    if ($scope.permissions) {
                        for (var i = 0; i < $scope.permissions.length; i++) {
                            var permission = $scope.permissions[i];

                            if (permission.EntityType == $scope.currentNode.EntType &&
                            permission.EntityId == $scope.currentNode.EntityId) {

                                angular.forEach(permission.PermissionFlags, function (permissionFlag) {
                                    if (permissionFlag.Name == permissionOption) {
                                        entityPermission.selectedFlagOptionIndex = permissionFlag.Value;
                                    }
                                });
                            }
                        }
                    }
                    
                    $scope.entityPermissions.push(entityPermission);
                });

                $scope.isLoadingPermissions = false;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            UtilsService.waitMultipleAsyncOperations([setTitle, loadTree, loadPermissions]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadTree() {
                var loadTreeDeferred = UtilsService.createPromiseDeferred();

                treeReadyDeferred.promise.then(function () {
                    getEntityNodes().then(function () {
                        loadTreeDeferred.resolve();
                    }).catch(function () {
                        loadTreeDeferred.reject();
                    });
                });

                return loadTreeDeferred.promise;

                function getEntityNodes() {
                    return VR_Sec_BusinessEntityAPIService.GetEntityNodes().then(function (response) {
                        if (response) {
                            $scope.beList = response;

                            for (var i = 0; i < response.length; i++) {
                                response[i].isOpened = true;
                            }

                            treeAPI.refreshTree($scope.beList);
                        }
                    });
                }
            }

            function loadPermissions() {
                return VR_Sec_PermissionAPIService.GetHolderPermissions(holderType, holderId).then(function (response) {
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.permissions.push(response[i].Entity);
                        }
                    }
                });
            }
        }

        function setTitle()
        {
            $scope.title = "Assign Permissions";
        }
    }

    appControllers.controller('VR_Sec_PermissionEditorController', PermissionEditorController);

})(appControllers);
