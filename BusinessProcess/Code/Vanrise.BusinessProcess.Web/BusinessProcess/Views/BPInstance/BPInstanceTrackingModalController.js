(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'VRNavigationService', 'BusinessProcess_BPInstanceAPIService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionAPIService'];

    function bpTrackingModalController($scope, VRNavigationService, BusinessProcess_BPInstanceAPIService, VRUIUtilsService, BusinessProcess_BPDefinitionAPIService) {

        var bpInstanceID;
        var bpDefinitionID;

        var filter;
        var instanceMonitorGridAPI;
        var instanceTrackingHistoryGridAPI;
        var instanceTrackingMonitorGridAPI;

        var timer;
        var isGettingData = false;
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


            $scope.modalContext.onModalHide = function () {
                instanceTrackingMonitorGridAPI.clearTimer();
                if ($scope.process.HasChildProcesses) {
                    instanceMonitorGridAPI.clearTimer();
                }
                clearTimeout(timer);
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

                $scope.process = {
                    InstanceStatus: response.InstanceStatus,
                    Title: response.Title,
                    Date: response.CreatedTime,
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
            createTimer();
        }

        function createTimer() {
            timer = setInterval(function () {
                if (!isGettingData) {
                    isGettingData = true;
                    BusinessProcess_BPInstanceAPIService.GetBPInstance(bpInstanceID).then(function (response) {
                        $scope.process.Status = response.StatusDescription;

                    })
                    .finally(function () {
                        isGettingData = false;
                    });
                }
            }, 5000);
        };

        load();
    }

    appControllers.controller("BusinessProcess_BP_TrackingModalController", bpTrackingModalController);

})(appControllers);