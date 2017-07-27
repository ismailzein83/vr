"use strict"
app.directive("demoModuleBuildingGrid", ["VRNotificationService", "Demo_Module_BuildingAPIService", "Demo_Module_BuildingService",
    function (VRNotificationService, Demo_Module_BuildingAPIService, Demo_Module_BuildingService) {
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
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Building/templates/BuildingGridTemplate.html"
        };
        function BuildingGrid($scope, ctrl, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.buildings = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }
                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onBuildingAdded = function (building) {
                            gridAPI.itemAdded(building);
                        };
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_BuildingAPIService.GetFilteredBuildings(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                defineMenuActions();
            }
            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editBuilding,

                }, {
                    name: "Delete",
                    clicked: deleteBuilding,
                }];
            }

            function editBuilding(building) {
                var onBuildingUpdated = function (building) {
                    gridAPI.itemUpdated(building);
                }
                Demo_Module_BuildingService.editBuilding(building.Entity.BuildingId, onBuildingUpdated);
            }

            function deleteBuilding(building) {
                var onBuildingDeleted = function (building) {
                    gridAPI.itemDeleted(building);
                };
                Demo_Module_BuildingService.deleteBuilding($scope, building, onBuildingDeleted)
            }


        }
        return directiveDefinitionObject;
    }]
    );