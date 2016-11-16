'use strict';
MenuRankingEditorController.$inject = ['$scope', 'VR_Sec_ViewAPIService', 'VRNotificationService', 'UtilsService', 'VR_Sec_MenuAPIService','VR_Sec_MenuTypeEnum'];
function MenuRankingEditorController($scope, VR_Sec_ViewAPIService, VRNotificationService, UtilsService, VR_Sec_MenuAPIService, VR_Sec_MenuTypeEnum) {
    var treeAPI;
    var maxMenuLevels = 2;
    var menuItemId = 0;
    defineScope();
    load();
    function defineScope() {
        $scope.menu = [];

        $scope.selectedMenuNode;

        $scope.menuReady = function (api) {
            treeAPI = api;
            if ($scope.menu.length > 0) {
                treeAPI.refreshTree($scope.menu);
            }
        };

        $scope.hasRankingPermission = function () {
            return VR_Sec_ViewAPIService.HasUpdateViewsRankPermission();

        };

        $scope.save = function () {
            if (treeAPI.getTree != undefined) {

                var changedMenu = builMenuItemsForSave();
                return VR_Sec_ViewAPIService.UpdateViewsRank(changedMenu).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("MenuItems", response)) {
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

        $scope.onMoveMenuItem = function (node, parent) {

            if (parent.isRoot && node.isLeaf)
                return false;
            if (parent.isLeaf)
                return false;

            var menu = $scope.menu;
            if (treeAPI.getTree != undefined)
                menu = treeAPI.getTree();
            return isAllowedNodeLevel(node, getNodeLevel(parent, menu) + 1);

        };
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls().finally(function () {
            $scope.isLoading = false;
        });
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadViews])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.scopeModal.isLoading = false;
          });
    }

    function setTitle() {

        $scope.title = 'Ranking MenuItems';
    }

    function loadViews() {
        return VR_Sec_MenuAPIService.GetMenuItems(true).then(function (response) {
            $scope.menuItems = [];
            setLeafNodes(response);
            angular.forEach(response, function (item) {
                $scope.menuItems.push(item);
            });
            var menu = {
                Name: "Root",
                Childs: $scope.menuItems,
                isRoot: true,
                isOpened: true
            };
            $scope.menu.push(menu);
            if (treeAPI != undefined) {
                treeAPI.refreshTree($scope.menu);
            }
        });
    }

    function setLeafNodes(childs)
    {
        if (childs == null || childs.length == 0)
            return;
        for(var i=0 ; i<childs.length;i++)
        {
            var child = childs[i];
            if (child.MenuType == VR_Sec_MenuTypeEnum.View.value)
            {
                childs[i].isLeaf = true;
                childs[i].ItemId = menuItemId++;
                setLeafNodes(child.Childs)
            }
                
            else
            {
                childs[i].isLeaf = false;
                childs[i].ItemId = menuItemId++;
                setLeafNodes(child.Childs)
            }
                
        }
    }

    function builMenuItemsForSave()
    {
        var preparedMenuItems = [];
        var menu = treeAPI.getTree();
        prepareMenuItems(menu[0].Childs, preparedMenuItems);
        return preparedMenuItems;
    }

    function prepareMenuItems(menuItems, preparedMenuItems)
    {
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
            };
            preparedMenuItems.push(preparedMenuItem);
            prepareMenuItems(menuItem.Childs, preparedMenuItem.Childs);
        }
    }

    function getNodeLevel(node,items)
    {
        if (items == undefined)
            return;
        for (var i = 0; i < items.length; i++) {
            var menuItem = items[i];
            if (node.ItemId == menuItem.ItemId)
                return 0;
            var obj = getNodeLevel(node, menuItem.Childs);
            if (obj != undefined)
                return obj + 1;
        }     
    }

    function isAllowedNodeLevel(node, level) {
        if (node == undefined)
            return true;
        if (node.isLeaf)
            return level <= maxMenuLevels + 1;

        //not leaf
        if (level > maxMenuLevels)
            return false;

        //level < 2
        if (node.Childs == undefined || node.Childs.length == 0)
            return true;

        for (var i = 0; i < node.Childs.length; i++) {
            if (!isAllowedNodeLevel(node.Childs[i], level + 1))
                return false;
        }

        return true;
    }

};

appControllers.controller('VR_Sec_MenuRankingEditorController', MenuRankingEditorController);