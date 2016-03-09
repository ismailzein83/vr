(function (appControllers) {

    'use strict';

    MenuManagementController.$inject = ['$scope', 'VRUIUtilsService', 'UtilsService','VRModalService', 'VRNotificationService','VR_Sec_MenuAPIService','VR_Sec_ViewAPIService','VR_Sec_ModuleService'];

    function MenuManagementController($scope, VRUIUtilsService, UtilsService, VRModalService, VRNotificationService, VR_Sec_MenuAPIService, VR_Sec_ViewAPIService, VR_Sec_ModuleService) {

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
            $scope.nodes = [];
            $scope.currentNode;

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
                    treeAPI.createNode(mapModuleNode(moduleObj.Entity));
                };

                VR_Sec_ModuleService.addModule(onModuleAdded);
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
                    //if (viewEntity != undefined) {

                    //    $scope.selectedMenuItem = treeAPI.setSelectedNode(menuItems, viewEntity.ModuleId, "Id", "Childs");
                    //}
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

        function onZoneAdded(addedZones) {
            if (addedZones != undefined) {
                $scope.hasState = true;
                for (var i = 0; i < addedZones.length; i++) {
                    var node = mapZoneToNode(addedZones[i]);
                    treeAPI.createNode(node);
                    for (var i = 0; i < $scope.nodes.length; i++) {
                        if ($scope.nodes[i].nodeId == $scope.currentNode.nodeId) {
                            $scope.nodes[i].effectiveZones.push(node);
                        }

                    }
                }


            }
        }

        function addNewZone() {
            var parameters = {
                CountryId: $scope.currentNode.nodeId,
                CountryName: $scope.currentNode.nodeName,
                SellingNumberPlanId: filter.sellingNumberPlanId
            };
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onZoneAdded = onZoneAdded;
            };

            VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/NewZoneDialog.html", parameters, settings);
        }

        function mapModuleNode(moduleObj)
        {
            var node = {
                Id:moduleObj.ModuleId,
                Name: moduleObj.Name,
            }
            return node;
        }
        //#endregion
    };

    appControllers.controller('VR_Sec_MenuManagementController', MenuManagementController);

})(appControllers);
