BusinessEntityManagementController.$inject = ['$scope', 'BusinessEntityAPIService2', 'PermissionAPIService', 'PermissionFlagEnum', 'VRModalService'];

function BusinessEntityManagementController($scope, BusinessEntityAPIService, PermissionAPIService, PermissionFlagEnum, VRModalService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {

        $scope.beList = [];

        $scope.gridMenuActions = [];

        $scope.permissions = [];

        defineMenuActions();

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            //getData();
        };

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.AddNewPermission = addPermission;
    }

    function load() {
        $scope.isGettingData = true;
        loadTree().finally(function () {
            $scope.isGettingData = false;
        });

        $scope.$watch('beTree.currentNode', function (newObj, oldObj) {
            if ($scope.beTree && angular.isObject($scope.beTree.currentNode)) {
                refreshGrid();
            }
        }, false);
    }

    function loadTree() {
        return BusinessEntityAPIService.GetEntityNodes()
           .then(function (response) {
               $scope.beList = response;
           });
    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();
        
        return PermissionAPIService.GetPermissionsByEntity($scope.beTree.currentNode.EntType, $scope.beTree.currentNode.EntityId).then(function (response) {
            angular.forEach(response, function (item) {
                item.PermissionFlagsDescription = buildPermissionFlagDescription(item.PermissionFlags);
                $scope.permissions.push(item);
            });
        });
    }

    function buildPermissionFlagDescription(permissionFlags)
    {
        var allowFlags = ""
        var denyFlags = "";

        angular.forEach(permissionFlags, function (item) {
            if(item.Value == PermissionFlagEnum.Allow.value)
                allowFlags = allowFlags + item.Name + ", ";
            else
                denyFlags = denyFlags + item.Name + ", ";
        });

        var result = "";
        if (allowFlags.length > 0)
        {
            allowFlags = allowFlags.substring(0, allowFlags.length - 2);
            result = "Allow (" + allowFlags + ")";
        }

        if (denyFlags.length > 0) {
            denyFlags = denyFlags.substring(0, denyFlags.length - 2);
            if (allowFlags.length > 0)
                result = result + ' | ';
            result = result + "Deny (" + denyFlags + ")";
        }

        return result;
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editPermission
        },
        {
            name: "Delete",
            clicked: deletePermission
        }
        ];
    }

    function addPermission() {

        if ($scope.beTree.currentNode == null)
            return;

        var modalSettings = {
        };
        var parameters = {
            entityType: $scope.beTree.currentNode.EntType,
            entityId: $scope.beTree.currentNode.EntityId,
            permissionOptions: $scope.beTree.currentNode.PermissionOptions,
            permissions: $scope.permissions,
            notificationResponseText: "Permissions"
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "New Permissions for Entity '" + $scope.beTree.currentNode.Name + "'";
            modalScope.onPermissionsAdded = function () {
                refreshGrid();
            };
        };
        
        VRModalService.showModal('/Client/Modules/Security/Views/BusinessEntityEditor.html', parameters, modalSettings);

    }

    function editPermission(permissionObj) {
        var modalSettings = {
        };
        var parameters = {
            holderType: permissionObj.HolderType,
            holderId: permissionObj.HolderId,
            entityType: permissionObj.EntityType,
            entityId: permissionObj.EntityId,
            permissionFlags: permissionObj.PermissionFlags,
            permissionOptions: $scope.beTree.currentNode.PermissionOptions,
            notificationResponseText: "Permissions"
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Permissions of Entity '" + $scope.beTree.currentNode.Name + "'";
            modalScope.onPermissionsUpdated = function () {
                refreshGrid();
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/BusinessEntityEditor.html', parameters, modalSettings);
    }

    function deletePermission(permissionObj) {
        return PermissionAPIService.DeletePermission(permissionObj.HolderType, permissionObj.HolderId, permissionObj.EntityType, permissionObj.EntityId)
           .then(function (response) {
               refreshGrid();
           });
    }

    function refreshGrid() {
        mainGridAPI.clearDataAndContinuePaging();
        getData();
    }
}
appControllers.controller('Security_BusinessEntityManagementController', BusinessEntityManagementController);