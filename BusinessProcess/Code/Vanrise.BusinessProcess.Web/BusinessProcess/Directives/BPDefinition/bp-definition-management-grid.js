"use strict";

app.directive("businessprocessBpDefinitionManagementGrid", ["UtilsService", "VRNotificationService", "BusinessProcess_BPDefinitionAPIService", "BusinessProcess_BPInstanceService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService, VRUIUtilsService) {

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
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPDefinition/Templates/BPDefinitionGridTemplate.html"

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

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Monitor";
                drillDownDefinition.directive = "businessprocess-bp-instance-monitor-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, bpDefinitionItem) {
                    bpDefinitionItem.bpInstanceGridAPI = directiveAPI;
                    var bpDefinitionIds = [];
                    bpDefinitionIds.push(bpDefinitionItem.Entity.BPDefinitionID);
                    var payload = {
                        DefinitionsId: bpDefinitionIds
                    };
                    return bpDefinitionItem.bpInstanceGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_BPDefinitionAPIService.GetFilteredBPDefinitions(dataRetrievalInput)
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

        }


        function defineMenuActions() {
            $scope.gridMenuActions = [
             { name: "Start New Instance", clicked: startNewInstance }
            ];
        }

        function startNewInstance(bpDefinitionObj) {
            var onProcessInputCreated = function (processInstanceId) {
                BusinessProcess_BPInstanceService.openProcessTracking(processInstanceId);
            };

            var onProcessInputsCreated = function () {
                VRNotificationService.showSuccess("Bussiness Instances created succesfully;  Open nested grid to see the created instances");
            };
            BusinessProcess_BPInstanceService.startNewInstance(bpDefinitionObj, onProcessInputCreated, onProcessInputsCreated);
        }
    }

    return directiveDefinitionObject;

}]);