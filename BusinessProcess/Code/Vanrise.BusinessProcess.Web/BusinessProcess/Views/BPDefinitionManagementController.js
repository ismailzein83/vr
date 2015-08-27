"use strict";

BPDefinitionManagementController.$inject = ['$scope', 'BusinessProcessService', 'BusinessProcessAPIService', 'BPInstanceStatusEnum', 'VRModalService', '$interval', 'VRNotificationService', 'UtilsService'];

function BPDefinitionManagementController($scope, BusinessProcessService, BusinessProcessAPIService, BPInstanceStatusEnum, VRModalService, $interval, VRNotificationService, UtilsService) {
    
    var interval;
    var mainGridApi;
    var statusUpdatedAfter = '';

    defineScope();
    load();
    startGetData();

    function defineScope() {

        $scope.filteredDefinitions = [];
        $scope.gridMenuActions = [];

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.processInstanceClicked = function (dataItem) {
            showBPTrackingModal(dataItem.ProcessInstanceID);
        }

        $scope.schedulerTaskClicked = function (dataItem) {
            showEditTaskModal(dataItem);
        }

        $scope.onGridReady = function (api) {
            mainGridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return BusinessProcessAPIService.GetFilteredDefinitions(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
                getRecentInstancesData();
                getScheduledTasksData();
            });
        };

        $scope.$on('$destroy', function () {
            stopGetData();
        });

        $scope.getStatusColor = function (dataItem, colDef) {
            return BusinessProcessService.getStatusColor(dataItem.Status);
        };

        defineMenuActions();
    }

    function load() {
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
             { name: "Start New Instance", clicked: showStartNewInstance }, { name: "Schedule a Task", clicked: showAddTaskModal }
        ];
    }

    function showStartNewInstance(BPDefinitionObj) {
        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/InstanceEditor.html', {
            BPDefinitionID: BPDefinitionObj.BPDefinitionID
        },
        {
            onScopeReady: function (modalScope) {
                modalScope.title = "Start New Instance";
                modalScope.onProcessInputCreated = function (processInstanceId) {
                    getRecentInstancesData();
                    showBPTrackingModal(processInstanceId);
                };

                modalScope.onProcessInputsCreated = function () {
                    VRNotificationService.showSuccess("Bussiness Instances created succesfully;  Open nested grid to see the created instances");
                    getRecentInstancesData();
                };
            }
        });
    }

    function showBPTrackingModal(processInstanceId) {

        BusinessProcessService.openProcessTracking(processInstanceId);

    }

    function startGetData() {
        if (angular.isDefined(interval)) return;
        interval = $interval(function callAtInterval() {
            getRecentInstancesData();
            getScheduledTasksData();
        }, 1000);
    }

    function stopGetData() {
        if (angular.isDefined(interval)) {
            $interval.cancel(interval);
            interval = undefined;
        }
    }

    function getRecentInstancesData() {

        angular.forEach($scope.filteredDefinitions, function (def) { def.openedInstances = []; });

        BusinessProcessAPIService.GetRecentInstances(statusUpdatedAfter).then(function (response) {

            angular.forEach(response, function (inst) {

                if (statusUpdatedAfter == '' && UtilsService.getEnum(BPInstanceStatusEnum, 'value', inst.Status).isOpened)
                    statusUpdatedAfter = inst.StatusUpdatedTime;


                angular.forEach($scope.filteredDefinitions, function (def) {
                    if (def.BPDefinitionID == inst.DefinitionID) {

                        if (angular.isUndefined(def.recentInstances)) {
                            def.recentInstances = [];
                        }

                        if (angular.isUndefined(def.openedInstances)) {
                            def.openedInstances = [];
                        }


                        if (UtilsService.getEnum(BPInstanceStatusEnum, 'value', inst.Status).isOpened) {
                            var openedProcessInstanceIndex = UtilsService.getItemIndexByVal(def.openedInstances, inst.ProcessInstanceID, "ProcessInstanceID");

                            if (openedProcessInstanceIndex >= 0)
                                def.openedInstances[openedProcessInstanceIndex] = inst;
                            else
                                def.openedInstances.push(inst);
                        }


                        var processInstanceIndex = UtilsService.getItemIndexByVal(def.recentInstances, inst.ProcessInstanceID, "ProcessInstanceID");

                        if (processInstanceIndex >= 0)
                            def.recentInstances[processInstanceIndex] = inst;
                        else
                            def.recentInstances.push(inst);


                    }
                });
            });
        });

    }

    function getScheduledTasksData() {

        angular.forEach($scope.filteredDefinitions, function (def) {
            def.scheduledTasks = [];
        });

        BusinessProcessAPIService.GetWorkflowTasksByDefinitionIds().then(function (response) {
            angular.forEach(response, function (task) {
                angular.forEach($scope.filteredDefinitions, function (def) {
                    if (def.BPDefinitionID == task.TaskAction.BPDefinitionID) {
                        if (angular.isUndefined(def.scheduledTasks)) {
                            def.scheduledTasks = [];
                        }
                        task.IsEnabledDescription = (task.IsEnabled ? "yes" : "no");
                        def.scheduledTasks.push(task);
                    }
                });
            });
        });

    }

    function showAddTaskModal(BPDefinitionObj) {
        var settings = {
        };
        var parameters = {
            additionalParameter: { bpDefinitionID: BPDefinitionObj.bpDefinitionID }
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Schedule Task";
            modalScope.onTaskAdded = function (task) {
                getScheduledTasksData();
            };
        };
        VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, settings);
    }

    function showEditTaskModal(taskObj) {
        var settings = {
        };
        var parameters = {
            taskId: taskObj.TaskId
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Task: " + taskObj.Name;
            modalScope.onTaskUpdated = function (task) {
                getScheduledTasksData();
            };
        };
        VRModalService.showModal('/Client/Modules/Runtime/Views/SchedulerTaskEditor.html', parameters, settings);
    }

    function retrieveData() {

        return mainGridApi.retrieveData({
            Title: $scope.title
        });
    }


};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);


