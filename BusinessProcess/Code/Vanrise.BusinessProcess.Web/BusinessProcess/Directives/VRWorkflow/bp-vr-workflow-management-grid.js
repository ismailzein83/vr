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
                var ctor = new VRWorkflowManagementGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowGridTemplate.html"
        };

        function VRWorkflowManagementGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.workflows = [];
                $scope.scopeModel.gridMenuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = BusinessProcess_VRWorkflowService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.scopeModel.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return BusinessProcess_VRWorkflowAPIService.GetFilteredVRWorkflows(dataRetrievalInput).then(function (response) {
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

            function defineAPI() {
                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                directiveAPI.onVRWorkflowAdded = function (addedVRWorkflow) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVRWorkflow);
                    gridAPI.itemAdded(addedVRWorkflow);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(directiveAPI);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions.push({
                    name: 'Edit',
                    clicked: editVRWorkflow,
                    haspermission: hasEditVRWorkflowPermission
                });
            }

            function editVRWorkflow(vrWorkflow) {
                var onVRWorkflowUpdated = function (updatedVRWorkflow) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRWorkflow);
                    gridAPI.itemUpdated(updatedVRWorkflow);
                };

                BusinessProcess_VRWorkflowService.editVRWorkflow(vrWorkflow.VRWorkflowID, onVRWorkflowUpdated);
            }

            function hasEditVRWorkflowPermission() {
                return BusinessProcess_VRWorkflowAPIService.HasEditVRWorkflowPermission();
            }
        }

        return directiveDefinitionObject;
    }]);