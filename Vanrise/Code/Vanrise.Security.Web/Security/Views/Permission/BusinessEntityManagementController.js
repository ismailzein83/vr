(function (appControllers) {

    'use strict';

    BusinessEntityManagementController.$inject = ['$scope', 'VR_Sec_BusinessEntityNodeAPIService', 'VR_Sec_PermissionService', 'VR_Sec_PermissionAPIService', 'UtilsService', 'VRNotificationService'];

    function BusinessEntityManagementController($scope, VR_Sec_BusinessEntityNodeAPIService, VR_Sec_PermissionService, VR_Sec_PermissionAPIService, UtilsService, VRNotificationService) {
        var treeAPI;

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.beList = [];
            $scope.showBreakInheritance = true;

            $scope.onTreeReady = function (api) {
                treeAPI = api;
            };

            $scope.onTreeValueChanged = function () {
                if ($scope.currentNode) {
                    $scope.showBreakInheritance = !$scope.currentNode.BreakInheritance;
                    gridAPI.loadGrid(getGridQuery());
                }
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.toggleInheritance = function () {
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return VR_Sec_BusinessEntityNodeAPIService.ToggleBreakInheritance($scope.currentNode.EntType, $scope.currentNode.EntityId).then(function (response) {
                            if (VRNotificationService.notifyOnItemUpdated("Toggle Break Inheritance", response)) {
                                $scope.currentNode.BreakInheritance = !$scope.currentNode.BreakInheritance;
                                $scope.showBreakInheritance = !$scope.currentNode.BreakInheritance;
                                gridAPI.loadGrid(getGridQuery());
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
            };

            $scope.addPermission = function () {
                var onPermissionAdded = function (addedPermission) {
                    gridAPI.onPermissionAdded(addedPermission);
                };
                VR_Sec_PermissionService.addPermission($scope.currentNode.EntType, $scope.currentNode.EntityId, $scope.currentNode.PermissionOptions, $scope.permissions, onPermissionAdded);
            };



            $scope.hasAddPermissionPermission = function () {
                return VR_Sec_PermissionAPIService.HasUpdatePermissionsPermission();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadTree, hasBreakInheritancePermission]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadTree() {
                return VR_Sec_BusinessEntityNodeAPIService.GetEntityNodes().then(function (response) {
                    $scope.beList = response;
                    treeAPI.refreshTree($scope.beList);
                });
            }
        }

        function getGridQuery() {
            return {
                EntityId: $scope.currentNode.EntityId,
                EntityType: $scope.currentNode.EntType,
                BusinessEntityNode: $scope.currentNode
            };
        }

        function hasBreakInheritancePermission() {
            return VR_Sec_BusinessEntityNodeAPIService.HasBreakInheritancePermission().then(function (response) {
                $scope.showBreakInheritane = response;
            });
        };

    }

    appControllers.controller('VR_Sec_BusinessEntityManagementController', BusinessEntityManagementController);

})(appControllers);