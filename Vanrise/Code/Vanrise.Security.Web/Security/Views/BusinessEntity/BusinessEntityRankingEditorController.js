'use strict';
BusinessEntityRankingEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VR_Sec_BusinessEntityNodeAPIService','VR_Sec_EntityTypeEnum'];
function BusinessEntityRankingEditorController($scope, VRNotificationService, UtilsService, VR_Sec_BusinessEntityNodeAPIService, VR_Sec_EntityTypeEnum) {
    var treeAPI;
    var maxMenuLevels = 2;
    var businessEntitiesItemId = 0;
    defineScope();
    load();
    function defineScope() {
        $scope.BusinessEntities = [];

        $scope.businessEntitiesReady = function (api) {
            treeAPI = api;
            if ($scope.BusinessEntities.length > 0) {
                treeAPI.refreshTree($scope.BusinessEntities);
            }
        };

        $scope.hasRankingPermission = function () {
            return VR_Sec_BusinessEntityNodeAPIService.HasUpdateEntityNodesRankPermission();

        };

        $scope.save = function () {
            if (treeAPI.getTree != undefined) {

                var changedBusinessEntities = builBusinessEntityItemsForSave();
                return VR_Sec_BusinessEntityNodeAPIService.UpdateEntityNodesRank(changedBusinessEntities).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Business Entities", response)) {
                        if ($scope.onRankingSuccess != undefined)
                            $scope.onRankingSuccess(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                });
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.onMoveItem = function (node, parent) {

            if (parent.isLeaf)
                return false;
        };
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls().finally(function () {
            $scope.isLoading = false;
        });
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadBusinessEntities])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.scopeModal.isLoading = false;
          });
    }

    function setTitle() {
        $scope.title = 'Ranking Business Entities';
    }

    function loadBusinessEntities() {
        return VR_Sec_BusinessEntityNodeAPIService.GetEntityNodes().then(function (response) {
            $scope.businessEntitiesItems = [];
            setLeafNodes(response);
            angular.forEach(response, function (item) {
                $scope.businessEntitiesItems.push(item);
            });
            if (treeAPI != undefined) {
                treeAPI.refreshTree($scope.businessEntitiesItems);
            }
        });
    }

    function setLeafNodes(children) {
        if (children == null || children.length == 0)
            return;
        for (var i = 0 ; i < children.length; i++) {
            var child = children[i];
            if (child.EntType == VR_Sec_EntityTypeEnum.ENTITY.value) {
                children[i].isLeaf = true;
                children[i].ItemId = businessEntitiesItemId++;
                setLeafNodes(child.Children)
            }

            else {
                children[i].isLeaf = false;
                children[i].ItemId = businessEntitiesItemId++;
                setLeafNodes(child.Children)
            }

        }
    }

    function builBusinessEntityItemsForSave() {
        var preparedBusinessEntityItems = [];
        var businessEntities = treeAPI.getTree();
        prepareBusinessEntityItems(businessEntities, preparedBusinessEntityItems);
        return preparedBusinessEntityItems;
    }

    function prepareBusinessEntityItems(businessEntities, preparedBusinessEntityItems, parent) {
        for (var i = 0; i < businessEntities.length ; i++) {
            var businessEntity = businessEntities[i];
            var preparedBusinessEntityItem = {
                EntityId: businessEntity.EntityId,
                Name: businessEntity.Name,
                Title: businessEntity.Title,
                EntType: businessEntity.EntType,
                PermissionOptions: businessEntity.PermissionOptions,
                BreakInheritance: businessEntity.BreakInheritance,
                Parent: parent,
                Children: []
            };
            preparedBusinessEntityItems.push(preparedBusinessEntityItem);
            prepareBusinessEntityItems(businessEntity.Children, preparedBusinessEntityItem.Children, businessEntity);
        }
    }
};

appControllers.controller('VR_Sec_BusinessEntityRankingEditorController', BusinessEntityRankingEditorController);