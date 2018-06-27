"use strict";

app.directive("businessprocessVrWorkflowManagementGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "BusinessProcess_VRWorkflowAPIService", "BusinessProcess_VRWorkflowService",
function (UtilsService, VRNotificationService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var vrWorkflowManagementGrid = new VRWorkflowManagementGrid($scope, ctrl, $attrs);
            vrWorkflowManagementGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowGridTemplate.html"

    };

    function VRWorkflowManagementGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.workflows = [];
            $scope.scopeModel.gridMenuActions = [];
           

            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = BusinessProcess_VRWorkflowService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };

                    directiveAPI.onWorkflowAdded = function (addedWorkflow) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(addedWorkflow);
                        gridAPI.itemAdded(addedWorkflow);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_VRWorkflowAPIService.GetFilteredVRWorkflows(dataRetrievalInput)
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
        }

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions.push({
                name: 'Edit',
                clicked: editVRWorkflow
            });
        }

        function editVRWorkflow(vrWorkflow) {
            var onVRWorkflowUpdated = function (vrWorkflow) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(vrWorkflow);
                gridAPI.itemUpdated(vrWorkflow);
            };
            BusinessProcess_VRWorkflowService.editWorkflow(vrWorkflow.VRWorkflowID, onVRWorkflowUpdated);
        }

    }
    return directiveDefinitionObject;

}]);