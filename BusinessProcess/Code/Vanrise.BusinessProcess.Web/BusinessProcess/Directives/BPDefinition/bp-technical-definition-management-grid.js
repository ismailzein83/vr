"use strict";

app.directive("businessprocessBpTechnicalDefinitionManagementGrid", ["UtilsService", "VRNotificationService", "BusinessProcess_BPDefinitionAPIService", "BusinessProcess_BPDefinitionService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPDefinitionService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var bpDefinitionManagementGrid = new BPDefinitionManagementGrid($scope, ctrl, $attrs);
                bpDefinitionManagementGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPDefinition/Templates/BPTechnicalDefinitionGridTemplate.html"
        };

        function BPDefinitionManagementGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;


            function initializeController() {
                $scope.bfDefinitions = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = BusinessProcess_BPDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return BusinessProcess_BPDefinitionAPIService.GetFilteredBPDefinitionsForTechnical(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
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
            }
            function defineAPI() {
                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                directiveAPI.onBPTechnicalDefinitionAdded = function (addedBPTechnicalDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedBPTechnicalDefinition);
                    gridAPI.itemAdded(addedBPTechnicalDefinition);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(directiveAPI);
            }

            function defineMenuActions() {
                $scope.gridMenuActions = function () {
                    var menuActions = [{
                        name: "Edit",
                        clicked: edit,
                        haspermission: hasUpdateBPDefinitionPermission
                    }];
                    return menuActions;
                };
            }
            function edit(dataItem) {
                var onBPDefinitionUpdated = function (updatedBPDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedBPDefinition);
                    gridAPI.itemUpdated(updatedBPDefinition);
                };
                BusinessProcess_BPDefinitionService.editBusinessProcessDefinition(dataItem.Entity.BPDefinitionID, onBPDefinitionUpdated);
            }
            function hasUpdateBPDefinitionPermission() {
                return BusinessProcess_BPDefinitionAPIService.HasUpdateBPDefinitionPermission();
            }
        }

        return directiveDefinitionObject;
    }]);