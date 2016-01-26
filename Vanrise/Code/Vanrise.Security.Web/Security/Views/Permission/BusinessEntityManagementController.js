(function (appControllers) {

    'use strict';

    BusinessEntityManagementController.$inject = ['$scope', 'VR_Sec_BusinessEntityAPIService', 'VR_Sec_PermissionService', 'UtilsService', 'VRNotificationService'];

    function BusinessEntityManagementController($scope, VR_Sec_BusinessEntityAPIService, VR_Sec_PermissionService, UtilsService, VRNotificationService) {
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
                return VR_Sec_BusinessEntityAPIService.ToggleBreakInheritance($scope.currentNode.EntType, $scope.currentNode.EntityId).then(function (response) {
                       $scope.currentNode.BreakInheritance = !$scope.currentNode.BreakInheritance;
                       $scope.showBreakInheritance = !$scope.currentNode.BreakInheritance;
                       gridAPI.loadGrid(getGridQuery());
                   });
            };

            $scope.addPermission = function () {
                var onPermissionAdded = function (addedPermission) {
                    gridAPI.onPermissionAdded(addedPermission);
                };
                VR_Sec_PermissionService.addPermission($scope.currentNode.EntType, $scope.currentNode.EntityId, $scope.currentNode.PermissionOptions, $scope.permissions, onPermissionAdded);
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadTree]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadTree() {
                return VR_Sec_BusinessEntityAPIService.GetEntityNodes().then(function (response) {
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
    }

    appControllers.controller('VR_Sec_BusinessEntityManagementController', BusinessEntityManagementController);

})(appControllers);
