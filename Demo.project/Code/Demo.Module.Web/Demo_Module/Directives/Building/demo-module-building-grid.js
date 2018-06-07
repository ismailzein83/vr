﻿"use strict"
app.directive("demoModuleBuildingGrid", ["UtilsService", "VRNotificationService", "Demo_Module_BuildingAPIService", "Demo_Module_BuildingService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_BuildingAPIService, Demo_Module_BuildingService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var buildingGrid = new BuildingGrid($scope, ctrl, $attrs);
            buildingGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Building/Templates/BuildingGridTemplate.html"
    };

    function BuildingGrid($scope, ctrl) {

        var gridApi;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.buildings = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                var drillDownDefinitions = [];
                AddRoomDrillDown();
                function AddRoomDrillDown() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Room";
                    drillDownDefinition.directive = "demo-Module-room-search";

                    drillDownDefinition.loadDirective = function (directiveAPI, buildingItem) {
                        buildingItem.roomGridAPI = directiveAPI;
                        var payload = {
                            buildingId: buildingItem.BuildingId
                        };
                        return buildingItem.roomGridAPI.load(payload);
                    };
                    drillDownDefinitions.push(drillDownDefinition);
                }

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, $scope.gridMenuActions);



                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (query) {
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onBuildingAdded = function (building) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(building);
                        gridApi.itemAdded(building);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_BuildingAPIService.GetFilteredBuildings(dataRetrievalInput)
                .then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editBuilding,

            }];
        };
        function editBuilding(building) {
            var onBuildingUpdated = function (building) {
                gridApi.itemUpdated(building);
            };
            Demo_Module_BuildingService.editBuilding(building.BuildingId, onBuildingUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
