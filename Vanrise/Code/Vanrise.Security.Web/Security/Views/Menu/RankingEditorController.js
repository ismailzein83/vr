'use strict'
RankingEditorController.$inject = ['$scope', 'VR_Sec_ViewAPIService', 'VRNotificationService', 'UtilsService', 'VR_Sec_MenuAPIService'];
function RankingEditorController($scope, VR_Sec_ViewAPIService, VRNotificationService, UtilsService, VR_Sec_MenuAPIService) {
    var treeAPI;
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
        }
        $scope.hasRankingPermission = function () {
            return VR_Sec_ViewAPIService.HasUpdateViewsRankPermission();

        };
        $scope.save = function () {
            if (treeAPI.getTree() != undefined) {
                var menu = treeAPI.getTree();
                var changedMenu = menu[0].Childs;
                console.log(changedMenu);
                var modules = [];
                var views = [];
                return VR_Sec_ViewAPIService.UpdateViewsRank(changedMenu).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("MenuItems", response)) {
                        if ($scope.onPageUpdated != undefined)
                            $scope.onPageUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                })
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal();
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

        $scope.title = UtilsService.buildTitleForAddEditor('Ranking MenuItems');
    }

    function loadViews() {
        return VR_Sec_MenuAPIService.GetMenuItems(true).then(function (response) {
            $scope.menuItems = [];
            setLeafNodes(response);
            angular.forEach(response, function (item) {
                $scope.menuItems.push(item);
            })
            var menu = {
                Name: "Root",
                Childs: $scope.menuItems,
                isOpened: true
            }
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

            if (childs[i].MenuType == 1)
            {
                childs[i].isLeaf = true;
                setLeafNodes(childs[i].Childs)
            }
                
            else
            {
                childs[i].isLeaf = false;
                setLeafNodes(childs[i].Childs)
            }
                
        }
    }

    function prepareChangedViews(menuItems)
    {
        var menu = [];
        for (var i = 0; i < menuItems.length ; i++) {
           var  menuItem = menuItems[i];
            if (!menuItem.isLeaf)///module
            {
                prepareViews(menuItem.Childs, menuItem, menu);
            } else if (menuItem.MenuType == 1) //View
            {
                views.push({ ModuleId: menuItem.Id })
            }
        }
    }
    function prepareViews(childs, parent, menu)
    {
       
        if (childs != undefined)
        {
            for (var i = 0; i < childs.length; i++) {
                var child = childs[i];
                if (child.isLeaf)
                    views.push({ ModuleId: child.Id, Rank: viewIndex++ });
                else {
                    modules.push({ ParentId: parent.Id, Rank: moduleIndex++ });
                    prepareViews(child.Childs, child,views, modules, viewIndex, moduleIndex)
                }
            }
        }
       
    }
};

appControllers.controller('VR_Sec_RankingEditorController', RankingEditorController);