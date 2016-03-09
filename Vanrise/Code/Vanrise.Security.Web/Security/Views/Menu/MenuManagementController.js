(function (appControllers) {

    'use strict';

    MenuManagementController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService','VRModalService', 'VRNotificationService','VR_Sec_MenuAPIService','VR_Sec_ViewAPIService','VR_Sec_ModuleService','VR_Sec_MenuService'];

    function MenuManagementController($scope, VRUIUtilsService, UtilsService, VRModalService, VRNotificationService, VR_Sec_MenuAPIService, VR_Sec_ViewAPIService, VR_Sec_ModuleService, VR_Sec_MenuService) {

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

            $scope.onModulesTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.showGrid = false;

            $scope.onViewGridReady = function(api)
            {
                gridAPI = api;
            }

            $scope.treeValueChanged = function () {
                if ($scope.selectedMenuItem != undefined) {
                    var query = {
                        ModuleId: $scope.selectedMenuItem.Id
                        };
                    gridAPI.loadGrid(query);
                    $scope.showGrid = true;
                }
            }

            $scope.addModule = function () {
                var onModuleAdded = function (moduleObj) {
                    console.log(moduleObj);
                    var node = mapModuleNode(moduleObj.Entity)
                    treeAPI.createNode(node);
                    for (var i = 0; i < menuItems.length; i++) {
                        if (menuItems[i].nodeId == moduleObj.ParentId) {
                            if (menuItems[i].Childs == undefined)
                                menuItems[i].Childs = [];
                            menuItems[i].Childs.push(node);
                        }
                    }
                    treeAPI.refreshTree(menuItems);
                };

                VR_Sec_ModuleService.addModule(onModuleAdded);
            }

            $scope.editModule = function()
            {
                var onModuleUpdated = function (moduleObj) {
                    console.log(moduleObj);
                    var node = mapModuleNode(moduleObj.Entity)
                    treeAPI.createNode(node);
                    console.log($scope.nodes)
                    for (var i = 0; i < menuItems.length; i++) {
                        var menuItem = menuItems[i];
                        if (menuItems[i].nodeId == moduleObj.ParentId) {
                            for (var j = 0; j < menuItem.Childs.length; j++)
                            {
                                if (menuItem.Childs[j].Id == node.Id)
                                    menuItem.Childs[j] = node;
                            }
                        }
                       
                    }
                    treeAPI.refreshTree(menuItems);
                };

                VR_Sec_ModuleService.editModule($scope.selectedMenuItem.Id,onModuleUpdated);
            }

            $scope.ranking = function()
            {
                var onRankingSuccess = function (moduleObj) {
                };

                VR_Sec_MenuService.openRankingEditor(onRankingSuccess);
                
            }
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
                return VR_Sec_MenuAPIService.GetAllMenuItems(false).then(function (response) {
                    if (response) {
                        menuItems = [];
                        for (var i = 0; i < response.length; i++) {
                            menuItems.push(response[i]);
                        }
                    }
                });
            }
        }

        function mapModuleNode(moduleObj)
        {
            var node = {
                Id:moduleObj.ModuleId,
                Name: moduleObj.Name,
                Childs:[]
            }
            return node;
        }
        //#endregion
    };

    appControllers.controller('VR_Sec_MenuManagementController', MenuManagementController);

})(appControllers);
