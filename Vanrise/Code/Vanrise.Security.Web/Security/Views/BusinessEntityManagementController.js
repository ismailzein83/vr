BusinessEntityManagementController.$inject = ['$scope', 'BusinessEntitiesAPIService', 'PermissionAPIService', 'PermissionFlagEnum', 'HolderTypeEnum', 'VRModalService'];

function BusinessEntityManagementController($scope, BusinessEntityAPIService, PermissionAPIService, PermissionFlagEnum, HolderTypeEnum, VRModalService) {

    var treeAPI;
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
        $scope.ToggleInheritance = toggleInheritance;
        $scope.showBreakInheritance = true;

        $scope.treeReady = function (api) {
            treeAPI = api;
        }

        $scope.treeValueChanged = function () {
            if (angular.isObject($scope.currentNode)) {
                $scope.showBreakInheritance = !$scope.currentNode.BreakInheritance;
                refreshGrid();
            }
        }
    }

    function load() {
        $scope.isGettingData = true;

        loadTree().finally(function () {
            $scope.isGettingData = false;
            treeAPI.refreshTree($scope.beList);
            console.log($scope.beList);
        });
    }

    function loadTree() {
        return BusinessEntityAPIService.GetEntityNodes()
           .then(function (response) {
               $scope.beList = response;
           });
    }

    function getData() {
        var pageInfo = mainGridAPI.getPageInfo();
        
        return PermissionAPIService.GetPermissionsByEntity($scope.currentNode.EntType, $scope.currentNode.EntityId).then(function (response) {
            angular.forEach(response, function (item) {
                item.HolderTypeEnum = (item.HolderType == HolderTypeEnum.User.value) ? HolderTypeEnum.User.description : HolderTypeEnum.Group.description;
                item.PermissionFlagsDescription = buildPermissionFlagDescription(item.PermissionFlags);
                item.isInherited = !(item.EntityType == $scope.currentNode.EntType && item.EntityId == $scope.currentNode.EntityId);
                item.PermissionType = item.isInherited ? 'Inherited (' + item.PermissionPath + ')' : 'Direct';
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
        var menuActions = [{
            name: "Edit",
            clicked: editPermission,
            permissions: "Root/Administration Module/System Entities:Assign Permissions"
        },
        {
            name: "Delete",
            clicked: deletePermission,
            permissions: "Root/Administration Module/System Entities:Assign Permissions"
        }
        ];

        $scope.gridMenuActions = function (dataItem) {
            if (dataItem.isInherited)
                return null;
            else
                return menuActions;
        };
    }

    function addPermission() {

        if ($scope.currentNode == null)
            return;

        var modalSettings = {
        };
        var parameters = {
            entityType: $scope.currentNode.EntType,
            entityId: $scope.currentNode.EntityId,
            permissionOptions: $scope.currentNode.PermissionOptions,
            permissions: $scope.permissions,
            notificationResponseText: "Permissions"
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "New Permissions for Entity '" + $scope.currentNode.Name + "'";
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
            permissionOptions: $scope.currentNode.PermissionOptions,
            notificationResponseText: "Permissions"
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Permissions of Entity '" + $scope.currentNode.Name + "'";
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

    function toggleInheritance()
    {
        return BusinessEntityAPIService.ToggleBreakInheritance($scope.currentNode.EntType, $scope.currentNode.EntityId)
           .then(function (response) {
               $scope.currentNode.BreakInheritance = !$scope.currentNode.BreakInheritance;
               $scope.showBreakInheritance = !$scope.currentNode.BreakInheritance;
               refreshGrid();
           });
    }
}
appControllers.controller('Security_BusinessEntityManagementController', BusinessEntityManagementController);