(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'VRNavigationService', 'BusinessProcess_BPInstanceAPIService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_BPInstanceService','VRTimerService'];

    function bpTrackingModalController($scope, VRNavigationService, BusinessProcess_BPInstanceAPIService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService, VRTimerService) {

        var bpInstanceID;
        var bpDefinitionID;

        var filter;
        var instanceMonitorGridAPI;
        var instanceTrackingHistoryGridAPI;
        var instanceTrackingMonitorGridAPI;
        var taskTrackingMonitorGridAPI;
        var validationMessageMonitorGridAPI;
        var validationMessageHistoryGridAPI;

        var bpInstance;
        var job;

        function defineScope() {

            $scope.onInstanceMonitorGridReady = function (api) {
                if ($scope.process.HasChildProcesses) {
                    instanceMonitorGridAPI = api;
                    getFilterObject();
                    instanceMonitorGridAPI.loadGrid(filter);
                }
            };

            $scope.onInstanceTrackingHistoryGridReady = function (api) {
                instanceTrackingHistoryGridAPI = api;
                getFilterObject();
                instanceTrackingHistoryGridAPI.loadGrid(filter);
            };

            $scope.onInstanceTrackingMonitorGridReady = function (api) {
                instanceTrackingMonitorGridAPI = api;
                getFilterObject();
                instanceTrackingMonitorGridAPI.loadGrid(filter);
            }

            $scope.onTaskMonitorGridReady = function (api) {
                taskTrackingMonitorGridAPI = api;
                getFilterObject();
                taskTrackingMonitorGridAPI.loadGrid(filter);
            }

            $scope.onValidationMessageMonitorGridReady = function (api) {
                validationMessageMonitorGridAPI = api;
                getFilterObject();
                validationMessageMonitorGridAPI.loadGrid(filter);
            }

            $scope.onValidationMessageHistoryGridReady = function (api) {
                validationMessageHistoryGridAPI = api;
                getFilterObject();
                validationMessageHistoryGridAPI.loadGrid(filter);
            }

            $scope.modalContext.onModalHide = function () {
                instanceTrackingMonitorGridAPI.clearTimer();
                if ($scope.process.HasChildProcesses) {
                    instanceMonitorGridAPI.clearTimer();
                }
                taskTrackingMonitorGridAPI.clearTimer();
                validationMessageMonitorGridAPI.clearTimer();
                if (job) {
                    VRTimerService.unregisterJob(job);
                }
            };

            $scope.getStatusColor = function () {
                if (bpInstance) {
                    return BusinessProcess_BPInstanceService.getStatusColor(bpInstance.Status);
                }
            };
        }

        function getFilterObject() {
            filter = {
                BPInstanceID: bpInstanceID,
            };
        }


        function getInstance() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                bpDefinitionID = response.DefinitionID;
                bpInstance = response;
                $scope.process = {
                    InstanceStatus: response.InstanceStatus,
                    Title: response.Title,
                    CreatedTime: response.CreatedTime,
                    StatusUpdatedTime: response.StatusUpdatedTime,
                    Status: response.StatusDescription,
                    HasChildProcesses: false
                };
                getDefinition();
            });
        }

        function getDefinition() {
            return BusinessProcess_BPDefinitionAPIService.GetBPDefintion(bpDefinitionID).then(function (response) {
                $scope.process.HasChildProcesses = response.Configuration.HasChildProcesses;
            });
        }

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                bpInstanceID = parameters.BPInstanceID;
            }
        }

        function load() {
            loadParameters();
            getInstance();
            defineScope();
            job = createTimer();
        }

        function createTimer() {
            if (job) {
                VRTimerService.unregisterJob(job);
            }
            return VRTimerService.registerJob(onTimerElapsed);
        }

        function onTimerElapsed() {
            return BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                $scope.process.Status = response.StatusDescription;
                $scope.process.StatusUpdatedTime = response.StatusUpdatedTime;
                bpInstance = response;
            },
             function (excpetion) {
                 console.log(excpetion);
                 $scope.isLoading = false;
             });
        }

        load();
    }

    appControllers.controller("BusinessProcess_BP_TrackingModalController", bpTrackingModalController);

})(appControllers);