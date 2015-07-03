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
                    $scope.searchClicked();
                    showBPTrackingModal(processInstanceId);
                };

                modalScope.onProcessInputsCreated = function () {
                    VRNotificationService.showSuccess("Bussiness Instances created succesfully;  Open nested grid to see the created instances");
                    $scope.searchClicked();
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
        }, 60000);
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
        $scope.bpDefintionIDs = [1, 2, 3];
        BusinessProcessAPIService.GetWorkflowTasksByDefinitionIds().then(function (response) {
            angular.forEach(response, function (task) {
                angular.forEach($scope.filteredDefinitions, function (def) {
                    if (def.BPDefinitionID == task.TaskAction.BPDefinitionID) {
                        if (angular.isUndefined(def.scheduledTasks)) {
                            def.scheduledTasks = [];
                        }

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
        $scope.gridMenuActions = [{
            name: "Start New Instance",
            clicked: showStartNewInstance
        }];

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
        console.log(dataItem)
        //showBPTrackingModal(dataItem.ProcessInstanceID);
    }


    defineGrid();
    startGetData();
};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);


