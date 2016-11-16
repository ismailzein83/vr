(function (appControllers) {

    'use strict';

    EditRequiredPermissionController.$inject = ['$scope', 'UtilsService', 'VR_Sec_BusinessEntityNodeAPIService', 'VRNavigationService', 'VRNotificationService', 'VR_Sec_EntityTypeEnum'];

    function EditRequiredPermissionController($scope, UtilsService, VR_Sec_BusinessEntityNodeAPIService, VRNavigationService, VRNotificationService,VR_Sec_EntityTypeEnum) {
        var treeAPI;
        var editMode;
        var permissionEntity;
        var permissions;
        var permissionSelectorAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                permissionEntity = parameters.RequiredPermission;
                permissions = parameters.Permissions;
            }
            editMode = permissionEntity != undefined;
        }

        function defineScope() {
            $scope.beList = [];
            $scope.onTreeReady = function (api) {
                treeAPI = api;
            };
            $scope.onPermissionReady = function (api) {
                permissionSelectorAPI = api;
            };
            $scope.buildTreeId = function (item) {
               return buildTreeId(item);
               
            };
            
            $scope.onTreeValueChanged = function () {
                var payload;
                if ($scope.currentNode && $scope.currentNode.EntType == 1) {
                    findInCurrentPermission($scope.currentNode);
                    payload = {
                        datasource: $scope.currentNode.PermissionOptions
                    };
                    if (permissionEntity != undefined) {
                        payload.selectedIds = permissionEntity.PermissionOptions;
                    }
                    permissionSelectorAPI.load(payload);
                }
                else{
                    permissionSelectorAPI.load(payload);
                }
            };

            $scope.save = function () {
                    return saveRequiredPermission();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function buildTreeId(item)
        {
            var id = "Module_" + item.EntityId;
            item.isLeaf = false;
            if (item.EntType == VR_Sec_EntityTypeEnum.ENTITY.value) {
                item.isLeaf = true;
                id = "Entity_" + item.EntityId;
            }
            return id;
        }
        function load() {
            $scope.title = "Required Permission";
            $scope.isLoading = true;
            loadAllControls();

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadTree, setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

           
        }

        function loadTree() {
            return VR_Sec_BusinessEntityNodeAPIService.GetEntityNodes().then(function (response) {
                $scope.beList = response;
                treeAPI.refreshTree($scope.beList);
                if (permissionEntity != undefined) {
                    permissionEntity.EntType = 1;
                    $scope.currentNode = treeAPI.setSelectedNode($scope.beList, buildTreeId(permissionEntity));
                    treeAPI.refreshTree($scope.beList);
                }

            });
        }

        function setTitle() {
            if (editMode && permissionEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(permissionEntity.EntityName, "Required Permission");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Required Permission");
        }

        function saveRequiredPermission() {
            var obj = buildRequiredPermissionObj();            
            if ($scope.onSavePermmision != undefined && typeof ($scope.onSavePermmision) == 'function') {
                $scope.onSavePermmision(obj);
            }
            $scope.modalContext.closeModal();
        }
        function buildRequiredPermissionObj() {
           
            return {
                EntityId: $scope.currentNode.EntityId,
                PermissionOptions: permissionSelectorAPI.getSelectedIds(),
                EntityName: $scope.currentNode.Name,
                EntType: $scope.currentNode.EntType

            };
        }
        
        function findInCurrentPermission(node) {
            var permission = UtilsService.getItemByVal(permissions, node.EntityId, "EntityId");
            if(permission !=null ){
                permissionEntity = permission, editMode = true;
                setTitle();
            }
                
            
            else {
                permissionEntity = undefined, editMode = false;
                setTitle();
            }
         
        }
      

    }

    appControllers.controller('VR_Sec_EditRequiredPermissionController', EditRequiredPermissionController);

})(appControllers);
