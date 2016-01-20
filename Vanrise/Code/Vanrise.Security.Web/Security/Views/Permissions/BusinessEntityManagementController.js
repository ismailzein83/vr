(function (appControllers) {

    'use strict';

    BusinessEntityManagementController.$inject = ['$scope', 'BusinessEntitiesAPIService', 'VR_Sec_PermissionService', 'PermissionFlagEnum', 'HolderTypeEnum', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function BusinessEntityManagementController($scope, BusinessEntityAPIService, VR_Sec_PermissionService, PermissionFlagEnum, HolderTypeEnum, UtilsService, VRModalService, VRNotificationService) {

        var treeAPI;
        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.beList = [];

            $scope.onPermissionGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.addPermission = function () {
                var onPermissionAdded = function (addedPermission) {
                    gridAPI.onPermissionAdded(addedPermission);
                };
                VR_Sec_PermissionService.addPermission($scope.currentNode.EntType, $scope.currentNode.EntityId, $scope.currentNode.PermissionOptions, $scope.permissions, onPermissionAdded);
            };

            $scope.ToggleInheritance = toggleInheritance;
            $scope.showBreakInheritance = true;

            $scope.treeReady = function (api) {
                treeAPI = api;
            }

            $scope.treeValueChanged = function () {
                if (angular.isObject($scope.currentNode)) {
                    $scope.showBreakInheritance = !$scope.currentNode.BreakInheritance;

                    var query = {
                        EntityId: $scope.currentNode.EntityId,
                        EntityType: $scope.currentNode.EntType,
                        BusinessEntityNode: $scope.currentNode
                    };

                    gridAPI.loadGrid(query);
                }
            }
        }

        function load() {
            $scope.isGettingData = true;

            UtilsService.waitMultiplePromises([loadTree(), gridReadyDeferred.promise]).finally(function () {
                $scope.isGettingData = false;
                treeAPI.refreshTree($scope.beList);
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations();
        }

        function loadTree() {
            return BusinessEntityAPIService.GetEntityNodes()
               .then(function (response) {
                   $scope.beList = response;
               }).catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               });
        }

        function toggleInheritance() {
            return BusinessEntityAPIService.ToggleBreakInheritance($scope.currentNode.EntType, $scope.currentNode.EntityId)
               .then(function (response) {
                   $scope.currentNode.BreakInheritance = !$scope.currentNode.BreakInheritance;
                   $scope.showBreakInheritance = !$scope.currentNode.BreakInheritance;
                   refreshGrid();
               });
        }
    }

    appControllers.controller('VR_Sec_BusinessEntityManagementController', BusinessEntityManagementController);

})(appControllers);
