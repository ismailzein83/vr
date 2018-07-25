"use strict";

app.directive("businessprocessBpDefinitionManagementGrid", ["UtilsService", "VRNotificationService", "BusinessProcess_BPDefinitionAPIService", "BusinessProcess_BPInstanceAPIService", "BusinessProcess_BPInstanceService", "VRUIUtilsService", "BusinessProcess_BPSchedulerTaskService", "VRTimerService",
    function (UtilsService, VRNotificationService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, VRUIUtilsService, BusinessProcess_BPSchedulerTaskService, VRTimerService) {

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
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPDefinition/Templates/BPDefinitionGridTemplate.html"
        };

        function BPDefinitionManagementGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var gridDrillDownTabsObj;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.bfDefinitions = [];

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
                    taskdrillDownDefinition.directive = "vr-runtime-schedulertask-panel";
                    taskdrillDownDefinition.hideDrillDownFunction = function (dataItem) {
                        return (dataItem.Entity.Configuration.ScheduledExecEditor == undefined || dataItem.Entity.Configuration.ScheduledExecEditor == "");
                    };
                    taskdrillDownDefinition.loadDirective = function (directiveAPI, bpDefinitionItem) {
                        bpDefinitionItem.bpSchedulerTaskAPI = directiveAPI;
                        var payload = {
                            bpDefinitionId: bpDefinitionItem.Entity.BPDefinitionID,
                            showAddSchedulerTask: bpDefinitionItem.ScheduleTaskAccess == true && bpDefinitionItem.Entity.Configuration.ScheduledExecEditor != undefined && bpDefinitionItem.Entity.Configuration.ScheduledExecEditor != ""
                        };

                        return bpDefinitionItem.bpSchedulerTaskAPI.loadPanel(payload);
                    };
                    drillDownDefinitions.push(taskdrillDownDefinition);

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

                    createTimer();
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

                defineMenuActions();
            }

            function attachPromis(dataItem) {

            }

            function createTimer() {
                if ($scope.job) {
                    VRTimerService.unregisterJob($scope.job);
                }
                VRTimerService.registerJob(onTimerElapsed, $scope);
            }

            function onTimerElapsed() {
                return BusinessProcess_BPInstanceAPIService.GetBPDefinitionSummary().then(function (response) {
                    manipulateDataUpdated(response);
                    $scope.isLoading = false;
                },
                function (excpetion) {
                    $scope.isLoading = false;
                });
            }

            function manipulateDataUpdated(response) {
                if (response != undefined) {
                    for (var j = 0; j < $scope.bfDefinitions.length; j++) {
                        var currentBPDefition = $scope.bfDefinitions[j];
                        var isFound = false;
                        for (var i = 0; i < response.length; i++) {
                            var bpDefinitionSummary = response[i];

                            if (currentBPDefition.Entity.BPDefinitionID == bpDefinitionSummary.BPDefinitionID) {
                                currentBPDefition.RunningProcessNumber = bpDefinitionSummary.RunningProcessNumber;
                                currentBPDefition.LastProcessCreatedTime = bpDefinitionSummary.LastProcessCreatedTime;
                                isFound = true;
                                continue;
                            }
                        }
                        if (!isFound) {
                            currentBPDefition.RunningProcessNumber = undefined;
                            currentBPDefition.LastProcessCreatedTime = undefined;
                        }
                    }
                }
            }

            function defineMenuActions() {

                $scope.gridMenuActions = function (dataItem) {
                    var startNewInstanceMenu = {
                        name: "Start New Instance",
                        clicked: startNewInstance,
                        haspermission: hasStartNewInstancePermission
                    };
                    var menuActions = [];
                    if (dataItem.Entity.Configuration.ManualExecEditor != undefined && dataItem.Entity.Configuration.ManualExecEditor != "") {
                        menuActions.push(startNewInstanceMenu);
                    }
                    return menuActions;
                };
            }

            function hasStartNewInstancePermission(bpDefinitionObj) {
                var actionPromise = UtilsService.createPromiseDeferred();
                actionPromise.resolve(bpDefinitionObj.StartNewInstanceAccess);
                return actionPromise.promise;
            }

            function startNewInstance(bpDefinitionObj) {
                var onProcessInputCreated = function (processInstanceId) {
                    BusinessProcess_BPInstanceService.openProcessTracking(processInstanceId);
                };

                var onProcessInputsCreated = function () {
                    VRNotificationService.showSuccess("Business Instances created succesfully;  Open nested grid to see the created instances");
                };
                BusinessProcess_BPInstanceService.startNewInstance(bpDefinitionObj, onProcessInputCreated, onProcessInputsCreated);
            };
        }

        return directiveDefinitionObject;
    }]);