'use strict'
BusinessEntityRankingEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VR_Sec_BusinessEntityNodeAPIService','VR_Sec_EntityTypeEnum'];
function BusinessEntityRankingEditorController($scope, VRNotificationService, UtilsService, VR_Sec_BusinessEntityNodeAPIService, VR_Sec_EntityTypeEnum) {
    var treeAPI;
    var maxMenuLevels = 2;
    var menuItemId = 0;
    defineScope();
    load();
    function defineScope() {
        $scope.menu = [];

        $scope.businessEntitiesReady = function (api) {
            treeAPI = api;
            if ($scope.menu.length > 0) {
                treeAPI.refreshTree($scope.menu);
            }
        }

        //$scope.hasRankingPermission = function () {
        //    return VR_Sec_ViewAPIService.HasUpdateViewsRankPermission();

        //};

        $scope.save = function () {
            if (treeAPI.getTree != undefined) {

                var changedMenu = builMenuItemsForSave();
                return VR_Sec_ViewAPIService.UpdateViewsRank(changedMenu).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Business Entities", response)) {
                        if ($scope.onRankingSuccess != undefined)
                            $scope.onRankingSuccess(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.onMoveMenuItem = function (node, parent) {
            if (parent.isLeaf)
                return false;
        }
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
            $scope.menuItems = [];
            setLeafNodes(response);
            angular.forEach(response, function (item) {
                $scope.menuItems.push(item);
            })
            if (treeAPI != undefined) {
                treeAPI.refreshTree($scope.menuItems);
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
                children[i].ItemId = menuItemId++;
                setLeafNodes(child.Children)
            }

            else {
                children[i].isLeaf = false;
                children[i].ItemId = menuItemId++;
                setLeafNodes(child.Children)
            }

        }
    }

    function builMenuItemsForSave() {
        var preparedMenuItems = [];
        var menu = treeAPI.getTree();
        prepareMenuItems(menu, preparedMenuItems);
        return preparedMenuItems;
    }

    function prepareMenuItems(menuItems, preparedMenuItems) {
        for (var i = 0; i < menuItems.length ; i++) {
            var menuItem = menuItems[i];
            var preparedMenuItem = {
                Id: menuItem.Id,
                Location: menuItem.Location,
                MenuType: menuItem.MenuType,
                Name: menuItem.Name,
                Rank: i + 1,
                Type: menuItem.Type,
                Title: menuItem.Title,
                AllowDynamic: menuItem.AllowDynamic,
                Childs: []
            }
            preparedMenuItems.push(preparedMenuItem);
            prepareMenuItems(menuItem.Children, preparedMenuItem.Children);
        }
    }
};

appControllers.controller('VR_Sec_BusinessEntityRankingEditorController', BusinessEntityRankingEditorController);