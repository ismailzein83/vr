BPDefinitionManagementController.$inject = ['$scope', 'BusinessProcessAPIService', 'VRModalService', '$interval', 'VRNotificationService'];

function BPDefinitionManagementController($scope, BusinessProcessAPIService, VRModalService, $interval, VRNotificationService) {

    "use strict";
    var interval, mainGridAPI;



    function showStartNewInstance(BPDefinitionObj) {
        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/InstanceEditor.html', {
            BPDefinitionID: BPDefinitionObj.BPDefinitionID
        },
        {
            onScopeReady: function (modalScope) {
                modalScope.title = "Start New Instance";
                modalScope.onProcessInputCreated = function (processInstanceId) {
                    getOpenedInstancesData();
                    showBPTrackingModal(processInstanceId);
                };

                modalScope.onProcessInputsCreated = function () {
                    VRNotificationService.showSuccess("Bussiness Instances created succesfully;  Open nested grid to see the created instances");
                    getOpenedInstancesData();
                };
            }
        });
    }


    function showBPTrackingModal(processInstanceId) {

        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            BPInstanceID: processInstanceId
        }, {
            onScopeReady: function (modalScope) {
                modalScope.title = "Tracking";
            }
        });
    }

    function startGetData() {
        if (angular.isDefined(interval)) return;
        interval = $interval(function callAtInterval() {
            getOpenedInstancesData();
            getScheduledTasksData();
        }, 10000);
    }

    function stopGetData() {
        if (angular.isDefined(interval)) {
            $interval.cancel(interval);
            interval = undefined;
        }
    }

    function getMainData() {
        var pageInfo = mainGridAPI.getPageInfo();

        var title = $scope.title != undefined ? $scope.title : '';

        BusinessProcessAPIService.GetFilteredDefinitions(title).then(function (response) {
            angular.forEach(response, function (def) {            
                $scope.filteredDefinitions.push(def);
            });
          
            getOpenedInstancesData();
            getScheduledTasksData();
        });
        
    }

    function getOpenedInstancesData() {

        angular.forEach($scope.filteredDefinitions, function (def) {
                def.openedInstances = [];
        });

        BusinessProcessAPIService.GetOpenedInstances().then(function (response) {
            angular.forEach(response, function (inst) {
                angular.forEach($scope.filteredDefinitions, function (def) {
                    if (def.BPDefinitionID == inst.DefinitionID) {
                        if (angular.isUndefined(def.openedInstances)) {
                            def.openedInstances = [];
                        }

                        if (angular.isUndefined(def.runningInstances)) {
                            def.runningInstances = 0;
                        }
                        def.runningInstances = parseInt(def.runningInstances)+1;
                        def.openedInstances.push(inst);
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


    function defineGrid() {
        $scope.filteredDefinitions = [];
        $scope.gridMenuActions = [];
       
        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            return getMainData();
        };
        $scope.gridMenuActions = [
            { name: "Start New Instance", clicked: showStartNewInstance }, { name: "Schedule a Task", clicked: showAddTaskModal }
        ];

        $scope.$on('$destroy', function () {
            stopGetData();
        });

    }

    $scope.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getMainData();
    };


    $scope.processInstanceClicked = function (dataItem) {
        showBPTrackingModal(dataItem.ProcessInstanceID);
    }


    $scope.schedulerTaskClicked = function (dataItem) {
        showEditTaskModal(dataItem);
    }



    

    function showAddTaskModal(BPDefinitionObj) {
        var settings = {
        };
        var parameters = {
            additionalParameter: { bpDefinitionID: BPDefinitionObj.bpDefinitionID }
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Add Task";
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








    defineGrid();
    startGetData();
};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);


