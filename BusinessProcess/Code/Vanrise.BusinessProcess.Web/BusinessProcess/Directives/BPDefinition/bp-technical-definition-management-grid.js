﻿"use strict";

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
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPDefinition/Templates/BPTechnicalDefinitionGridTemplate.html"

    };

    function BPDefinitionManagementGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.bfDefinitions = [];
            defineMenuActions();

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = BusinessProcess_BPDefinitionService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_BPDefinitionAPIService.GetFilteredBPDefinitionsForTechnical(dataRetrievalInput)
                    .then(function (response) {
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

        function hasUpdateBPDefinitionPermission() {
            return BusinessProcess_BPDefinitionAPIService.HasUpdateBPDefinitionPermission();
        }
        function edit(dataItem) {
            var onBPDefinitionUpdated = function (updatedBPDefinition) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(updatedBPDefinition);
                gridAPI.itemUpdated(updatedBPDefinition);
            };
            BusinessProcess_BPDefinitionService.editBusinessProcessDefinition(dataItem.Entity.BPDefinitionID, onBPDefinitionUpdated);
           
        }
    }

    return directiveDefinitionObject;

}]);