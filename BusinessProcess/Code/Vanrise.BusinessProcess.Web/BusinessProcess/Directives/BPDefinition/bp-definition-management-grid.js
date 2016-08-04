"use strict";

app.directive("businessprocessBpDefinitionManagementGrid", ["UtilsService", "VRNotificationService", "BusinessProcess_BPDefinitionAPIService", "BusinessProcess_BPInstanceService", "VRUIUtilsService","BusinessProcess_BPSchedulerTaskService",
function (UtilsService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService, VRUIUtilsService, BusinessProcess_BPSchedulerTaskService) {

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

                drillDownDefinition.title = "Recent Instances";
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




                var taskdrillDownDefinition = {};

                taskdrillDownDefinition.title = "Scheduled Instances";
                taskdrillDownDefinition.directive = "vr-runtime-schedulertask-grid";
                taskdrillDownDefinition.hideDrillDownFunction = function (dataItem) {
                    return (dataItem.Entity.Configuration.ScheduledExecEditor == undefined || dataItem.Entity.Configuration.ScheduledExecEditor == "");
                };
                taskdrillDownDefinition.loadDirective = function (directiveAPI, bpDefinitionItem) {
                    bpDefinitionItem.bpInstanceGridAPI = directiveAPI;
                    
                    var filter = {
                        $type: "Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.WFTaskBPDefinitionFilter, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments",
                        BPDefinitionId: bpDefinitionItem.Entity.BPDefinitionID
                    };
                    var payload = { Filters: [] };
                    payload.Filters.push(filter);
                   
                    return bpDefinitionItem.bpInstanceGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(taskdrillDownDefinition);






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
            var startNewInstanceMenu = {
                name: "Start New Instance", clicked: startNewInstance
            };
            var schedualTaskMenu = {
                name: "Schedule a Task", clicked: scheduleTask
            }

            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                if (dataItem.Entity.Configuration.ManualExecEditor != undefined && dataItem.Entity.Configuration.ManualExecEditor != "")
                {
                    menuActions.push(startNewInstanceMenu);
                }
                if (dataItem.Entity.Configuration.ScheduledExecEditor != undefined && dataItem.Entity.Configuration.ScheduledExecEditor != "")
                {
                    menuActions.push(schedualTaskMenu);
                }
                return menuActions;
            };
            
        }

        function startNewInstance(bpDefinitionObj) {
            var onProcessInputCreated = function (processInstanceId) {
                BusinessProcess_BPInstanceService.openProcessTracking(processInstanceId);
            };

            var onProcessInputsCreated = function () {
                VRNotificationService.showSuccess("Bussiness Instances created succesfully;  Open nested grid to see the created instances");
            };
            BusinessProcess_BPInstanceService.startNewInstance(bpDefinitionObj, onProcessInputCreated, onProcessInputsCreated);
        };

        function scheduleTask(bpDefinitionObj) {
            BusinessProcess_BPSchedulerTaskService.showAddTaskModal(bpDefinitionObj);
        };
    }

    return directiveDefinitionObject;

}]);