(function (appControllers) {

    "use strict";

    CDRComparisonConfigurationTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'CDRComparison_CDRComparisonAPIService'];

    function CDRComparisonConfigurationTaskController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRComparisonAPIService) {
        var bpTaskId;
        var processInstanceId;

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                bpTaskId = parameters.TaskId;
            }
        }

        function defineScope() {

            $scope.scopeModal = {};
            $scope.scopeModal.durations = [{
                value: 0,
                description:"Sec"
            }, {
                value: 1,
                description: "Ms"
            }];
            $scope.scopeModal.onDisputeCDRGridReady = function (api) {
                disputeCDRGridAPI = api;
                var payload = {};
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingDisputeCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, disputeCDRGridAPI, payload, setLoader);
            }

            $scope.scopeModal.continueTask = function () {
                return executeTask(true);
            }

            $scope.scopeModal.stopTask = function () {
                return executeTask(false);
            }
        }

        function executeTask(taskAction) {
            var durationMarginInMilliseconds = $scope.scopeModal.durationMargin;
            if ($scope.scopeModal.selectedDuration.value = 0)
                durationMarginInMilliseconds = $scope.scopeModal.durationMargin * 1000;

            var timeMarginInMilliSeconds = $scope.scopeModal.timeMargin;
            if ($scope.scopeModal.selectedTimeDuration.value = 0)
                timeMarginInMilliSeconds = $scope.scopeModal.timeMargin * 1000;
            var executionInformation = {
                $type: "CDRComparison.BP.Arguments.ConfigurationTaskExecutionInformation, CDRComparison.BP.Arguments",
                Decision: taskAction,
                DurationMarginInMilliSeconds: durationMarginInMilliseconds,
                TimeMarginInMilliSeconds: timeMarginInMilliSeconds,
                
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: bpTaskId,
                ExecutionInformation: executionInformation
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });
        }


        function load() {
            $scope.scopeModal.isLoading = true;
            BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (response) {
                processInstanceId = response.ProcessInstanceId;
                loadAllControls();
            })
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([loadSummaryData])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }

        function loadSummaryData() {
        }

    }

    appControllers.controller('CDRComparison_CDRComparisonConfigurationTaskController', CDRComparisonConfigurationTaskController);
})(appControllers);
