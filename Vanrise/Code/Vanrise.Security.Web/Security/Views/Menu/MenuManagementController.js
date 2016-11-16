(function (appControllers) {

    'use strict';

    MenuManagementController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService','VRModalService', 'VRNotificationService','VR_Sec_MenuAPIService','VR_Sec_ViewAPIService','VR_Sec_ModuleService','VR_Sec_MenuService','VR_Sec_ModuleAPIService'];

    function MenuManagementController($scope, VRUIUtilsService, UtilsService, VRModalService, VRNotificationService, VR_Sec_MenuAPIService, VR_Sec_ViewAPIService, VR_Sec_ModuleService, VR_Sec_MenuService, VR_Sec_ModuleAPIService) {

        //#region Global Variables
        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        var menuItems = [];
        var gridAPI;
        //#endregion

        //#region Load

        defineScope();
        loadParameters();
        load();

        //#endregion

        //#region Functions

        function defineScope() {
            $scope.hasRankingPermission = function () {
                return VR_Sec_ViewAPIService.HasUpdateViewsRankPermission();

            };

            $scope.hasUpdateModulePermission = function () {
                return VR_Sec_ModuleAPIService.HasUpdateModulePermission();

            };

            $scope.hasAddModulePermission = function () {
                return VR_Sec_ModuleAPIService.HasAddModulePermission();

            };

            $scope.onModulesTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.showGrid = false;

            $scope.onViewGridReady = function (api) {
                gridAPI = api;
            };

            $scope.treeValueChanged = function () {
                if ($scope.selectedMenuItem != undefined) {
                    var query = {
                        ModuleId: $scope.selectedMenuItem.Id
                    };
                    gridAPI.loadGrid(query);
                    $scope.showGrid = true;
                }
            };

            $scope.addModule = function () {
                var onModuleAdded = function (moduleObj) {
                    var node = mapModuleNode(moduleObj.Entity);
                    onAddedModule(menuItems, node);
                    treeAPI.createNode(node);
                    treeAPI.refreshTree(menuItems);
                };

                VR_Sec_ModuleService.addModule(onModuleAdded, $scope.selectedMenuItem.Id);
            };

            $scope.editModule = function () {
                var onModuleUpdated = function (moduleObj) {
                    var node = mapModuleNode(moduleObj.Entity);
                    treeAPI.createNode(node);
                    onEditModule(menuItems[0].Childs, moduleObj.Entity, node);
                    //  treeAPI.refreshTree(menuItems);
                };

                VR_Sec_ModuleService.editModule($scope.selectedMenuItem.Id, onModuleUpdated);
            };

            $scope.ranking = function () {
                var onRankingSuccess = function (moduleObj) {
                    menuItems.length = 0;
                    var menus = [];
                    for (var i = 0; i < moduleObj.length; i++) {
                        menus.push(moduleObj[i]);
                    }
                    var menu = {
                        Name: "Root",
                        Childs: menus,
                        isRoot: true,
                        isOpened: true
                    };
                    menuItems.push(menu);
                    treeAPI.refreshTree(menuItems);
                };

                VR_Sec_MenuService.openRankingEditor(onRankingSuccess);

            };
        }

        function loadParameters() {
        }

        function load() {
            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([loadTree]).then(function () {
                $scope.isLoading = false;
            }).catch(function (error) {
                $scope.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadTree() {
            var treeLoadDeferred = UtilsService.createPromiseDeferred();

            loadMenuItems().then(function () {
                treeReadyDeferred.promise.then(function () {
                    treeAPI.refreshTree(menuItems);
                    treeLoadDeferred.resolve();
                });
            }).catch(function (error) {
                treeLoadDeferred.reject(error);
            });

            return treeLoadDeferred.promise;

            function loadMenuItems() {
                return VR_Sec_MenuAPIService.GetAllMenuItems(false,true).then(function (response) {
                    if (response) {
                        var menus = [];
                        for (var i = 0; i < response.length; i++) {
                            menus.push(response[i]);
                        }
                        var menu = {
                            Name: "Root",
                            Childs: menus,
                            isRoot: true,
                            isOpened: true
                        };
                        menuItems.push(menu);
                    }
                });
            }
        }

        function mapModuleNode(moduleObj)
        {
            var node = {
                Id: moduleObj.ModuleId,
                Name: moduleObj.Name,
                Childs: []
            };
            return node;
        }

        function onEditModule(menuItems, changedObject, newNode) {

            if (menuItems != undefined)
            {
                for (var i = 0; i < menuItems.length ; i++) {
                    var menuItem = menuItems[i];
                    if (menuItem.Id != changedObject.ModuleId)///module
                    {
                        onEditModule(menuItem.Childs, changedObject, newNode);
                    } else if (menuItem.Id == changedObject.ModuleId)//View
                    {
                        menuItems[i] = newNode;
                    }
                }
            }
           
        }

        function onAddedModule(menuItems, newNode) {

            if (menuItems != undefined) {
                for (var i = 0; i < menuItems.length ; i++) {
                    var menuItem = menuItems[i];
                    if (menuItem.Id != $scope.selectedMenuItem.Id)///module
                    {
                        onAddedModule(menuItem.Childs, newNode);
                    } else if (menuItem.Id == $scope.selectedMenuItem.Id || $scope.selectedMenuItem.isRoot)//View
                    {
                        if (menuItems[i].Childs == undefined)
                            menuItems[i].Childs = [];
                        menuItems[i].Childs.push(newNode);
                    }
                }
            }

        }
    
        //#endregion
    };

    appControllers.controller('VR_Sec_MenuManagementController', MenuManagementController);

})(appControllers);
