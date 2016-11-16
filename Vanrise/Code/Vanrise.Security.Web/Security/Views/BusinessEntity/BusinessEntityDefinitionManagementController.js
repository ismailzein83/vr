(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionManagementController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService', 'VRModalService', 'VRNotificationService', 'VR_Sec_MenuAPIService', 'VR_Sec_BusinessEntityDefinitionService', 'VR_Sec_BusinessEntityModuleService', 'VR_Sec_MenuService', 'VR_Sec_BusinessEntityNodeAPIService', 'VR_Sec_BusinessEntityModuleAPIService', 'VR_Sec_BusinessEntityAPIService'];

    function BusinessEntityDefinitionManagementController($scope, VRUIUtilsService, UtilsService, VRModalService, VRNotificationService, VR_Sec_MenuAPIService, VR_Sec_BusinessEntityDefinitionService, VR_Sec_BusinessEntityModuleService, VR_Sec_MenuService, VR_Sec_BusinessEntityNodeAPIService, VR_Sec_BusinessEntityModuleAPIService, VR_Sec_BusinessEntityAPIService) {

        //#region Global Variables
        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        var entityItems = [];
        var gridAPI;
        //#endregion

        //#region Load

        defineScope();
        loadParameters();
        load();

        //#endregion

        //#region Functions

        function defineScope() {
            $scope.addEntity = false;
            $scope.addModule = false;
            $scope.addActions = [{
                name: "Add Module",
                    clicked: function () {
                            var onBusinessEntityModuleAdded = function (moduleObj) {
                                var node = mapBusinessEntityModuleNode(moduleObj);
                                onAddedModule(entityItems, node);
                                treeAPI.createNode(node);
                                treeAPI.refreshTree(entityItems);
                            };

                            return VR_Sec_BusinessEntityModuleService.addBusinessEntityModule(onBusinessEntityModuleAdded, $scope.selectedMenuItem.EntityId);
                    },
                    haspermission: function () {
                        return VR_Sec_BusinessEntityModuleAPIService.HasAddBusinessEntityModulePermission();
                       
                    }

                },
            {
                name: "Add Entity",
                clicked: function () {
                    var onBusinessEntityDefinitionAdded = function (entityObj) {
                        gridAPI.onBusinessEntityAdded(entityObj);
                    };
                    return VR_Sec_BusinessEntityDefinitionService.addBusinessEntityDefinition(onBusinessEntityDefinitionAdded, $scope.selectedMenuItem.EntityId);
                },
                haspermission: function () {
                    return VR_Sec_BusinessEntityAPIService.HasAddBusinessEntityPermission();                  
                }

            }];
            $scope.hasRankingPermission = function () {
                return VR_Sec_BusinessEntityNodeAPIService.HasUpdateEntityNodesRankPermission();

            };

            $scope.hasUpdateModulePermission = function () {
                return VR_Sec_BusinessEntityModuleAPIService.HasUpdateBusinessEntityModulePermission();

            };

            $scope.onModulesTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.showGrid = false;

            $scope.onEntitiesGridReady = function (api) {
                gridAPI = api;
            };

            $scope.treeValueChanged = function () {
                if ($scope.selectedMenuItem != undefined) {
                    var query = {
                        ModuleId: $scope.selectedMenuItem.EntityId
                    };
                    gridAPI.loadGrid(query);
                    $scope.showGrid = true;
                }
            };

            $scope.editBusinessEntityModule = function () {
                var onBusinessEntityModuleUpdated = function (moduleObj) {
                    var node = mapBusinessEntityModuleNode(moduleObj);
                    treeAPI.createNode(node);
                    onEditModule(entityItems, moduleObj, node);
                };

                VR_Sec_BusinessEntityModuleService.updateBusinessEntityModule($scope.selectedMenuItem.EntityId, onBusinessEntityModuleUpdated);
            };

            $scope.ranking = function () {
                var onRankingSuccess = function (moduleObj) {
                    entityItems.length = 0;

                    for (var i = 0; i < moduleObj.length; i++) {
                        entityItems.push(moduleObj[i]);
                    }
                    treeAPI.refreshTree(entityItems);
                };

                VR_Sec_BusinessEntityDefinitionService.openBusinessEntityRankingEditor(onRankingSuccess);

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

            loadEntityModules().then(function () {
                treeReadyDeferred.promise.then(function () {
                    treeAPI.refreshTree(entityItems);
                    treeLoadDeferred.resolve();
                });
            }).catch(function (error) {
                treeLoadDeferred.reject(error);
            });

            return treeLoadDeferred.promise;

            function loadEntityModules() {
                return VR_Sec_BusinessEntityNodeAPIService.GetEntityModules().then(function (response) {
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            response[i].isOpened = true;
                            entityItems.push(response[i]);
                        }
                    }
                });
            }
        }

        function mapBusinessEntityModuleNode(moduleObj) {
            var node = {
                EntityId: moduleObj.ModuleId,
                Name: moduleObj.Name,
                Children: []
            };
            return node;
        }

        function onEditModule(entityItems, changedObject, newNode) {

            if (entityItems != undefined) {
                for (var i = 0; i < entityItems.length ; i++) {
                    var menuItem = entityItems[i];
                    if (menuItem.EntityId != changedObject.ModuleId)///module
                    {
                        onEditModule(menuItem.Children, changedObject, newNode);
                    } else if (menuItem.EntityId == changedObject.ModuleId)//View
                    {
                        entityItems[i] = newNode;
                    }
                }
            }

        }

        function onAddedModule(entityItems, newNode) {

            if (entityItems != undefined) {
                for (var i = 0; i < entityItems.length ; i++) {
                    var menuItem = entityItems[i];
                    if (menuItem.EntityId != $scope.selectedMenuItem.EntityId)///module
                    {
                        onAddedModule(menuItem.Children, newNode);
                    } else if (menuItem.EntityId == $scope.selectedMenuItem.EntityId || $scope.selectedMenuItem.isRoot)//View
                    {
                        if (entityItems[i].Children == undefined)
                            entityItems[i].Children = [];
                        entityItems[i].Children.push(newNode);
                    }
                }
            }

        }


        //#endregion
    };

    appControllers.controller('VR_Sec_BusinessEntityDefinitionManagementController', BusinessEntityDefinitionManagementController);

})(appControllers);
