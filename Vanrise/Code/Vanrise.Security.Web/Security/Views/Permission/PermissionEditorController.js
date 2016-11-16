(function (appControllers) {

    'use strict';

    PermissionEditorController.$inject = ['$scope', 'VR_Sec_BusinessEntityNodeAPIService', 'VR_Sec_PermissionAPIService', 'VR_Sec_PermissionFlagEnum', 'UtilsService', 'VRNavigationService', 'VRNotificationService'];

    function PermissionEditorController($scope, VR_Sec_BusinessEntityNodeAPIService, VR_Sec_PermissionAPIService, VR_Sec_PermissionFlagEnum, UtilsService, VRNavigationService, VRNotificationService) {
        var holderType;
        var holderId;
        var notificationResponseText;

        var treeAPI;
        
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                holderType = parameters.holderType;
                holderId = parameters.holderId;
                notificationResponseText = parameters.notificationResponseText;
            }
        }

        function defineScope() {
            $scope.beList = [];
            $scope.entityPermissions = []; // The temporary permission list that appear next to the tree on node selection1
            $scope.permissions = []; // The saved permissions in the data base for a single holder type and id
            $scope.permissionFlagOptions = UtilsService.getEnumPropertyAsArray(VR_Sec_PermissionFlagEnum, 'description');

            $scope.hasSaveAssignPermission = function () {
                return VR_Sec_PermissionAPIService.HasUpdatePermissionsPermission();
            };

            $scope.onTreeReady = function (api) {
                treeAPI = api;
            };

            $scope.onTreeValueChanged = onTreeValueChanged;

            $scope.onPermissionSelectionChanged = onPermissionSelectionChanged;

            $scope.save = savePermissions;

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadTree, loadPermissions]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = "Assign Permissions";
            }

            function loadTree() {
                return getEntityNodes().then(function () {
                    treeAPI.refreshTree($scope.beList);
                });

                function getEntityNodes() {
                    return VR_Sec_BusinessEntityNodeAPIService.GetEntityNodes().then(function (response) {
                        if (response) {
                            $scope.beList = response;

                            for (var i = 0; i < response.length; i++) {
                                response[i].isOpened = true;
                            }
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

        function onTreeValueChanged() {
            if ($scope.currentNode) {
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

                            if (permission.EntityType == $scope.currentNode.EntType && permission.EntityId == $scope.currentNode.EntityId) {
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

        function onPermissionSelectionChanged(selectedPermission) {
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

        function savePermissions() {
            var saveDeferred = UtilsService.createPromiseDeferred();
            $scope.isLoading = true;

            if ($scope.permissions) {
                var permissionObject = [];

                for (var i = 0; i < $scope.permissions.length; i++) {
                    if ($scope.permissions[i].isDirty) {
                        permissionObject.push($scope.permissions[i]);
                    }
                }

                if (permissionObject.length > 0) {
                    VR_Sec_PermissionAPIService.UpdatePermissions(permissionObject).then(function (response) {
                        saveDeferred.resolve();

                        if (VRNotificationService.notifyOnItemUpdated(notificationResponseText, response)) {
                            $scope.modalContext.closeModal();
                        }
                    }).catch(function (error) {
                        saveDeferred.reject();
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.isLoading = false;
                    });
                }
                else {
                    resolveDeferredAndNotifyInfo();
                }
            }
            else {
                resolveDeferredAndNotifyInfo();
            }

            return saveDeferred.promise;

            function resolveDeferredAndNotifyInfo() {
                saveDeferred.resolve();
                $scope.isLoading = false;
                VRNotificationService.showInformation('No changes were made');
            }
        }
    }

    appControllers.controller('VR_Sec_PermissionEditorController', PermissionEditorController);

})(appControllers);