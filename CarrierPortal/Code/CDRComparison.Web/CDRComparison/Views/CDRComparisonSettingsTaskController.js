(function (appControllers) {

    "use strict";

    CDRComparisonSettingsTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'CDRComparison_CDRComparisonAPIService', 'CDRComparison_TimeDurationEnum', 'CDRComparison_CDRComparisonService'];

    function CDRComparisonSettingsTaskController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRComparisonAPIService, CDRComparison_TimeDurationEnum, CDRComparison_CDRComparisonService) {
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
            $scope.scopeModal.durations = UtilsService.getArrayEnum(CDRComparison_TimeDurationEnum);
            $scope.scopeModal.selectedDuration = CDRComparison_TimeDurationEnum.Seconds;
            $scope.scopeModal.selectedTimeDuration = CDRComparison_TimeDurationEnum.Seconds;

            $scope.scopeModal.timeOffset = '00:00:00';
            $scope.scopeModal.openHelper = function()
            {
                var onTimeOffsetSelected = function (timeOffset) {
                    $scope.scopeModal.timeOffset = timeOffset;
                };
                CDRComparison_CDRComparisonService.openTimeOffsetHelper(onTimeOffsetSelected);
            }

            $scope.scopeModal.validateTimeOffset = function (value) {
               return UtilsService.validateTimeOffset(value);
            }

            $scope.scopeModal.continueTask = function () {
                return executeTask(true);
            }
        }

        function executeTask(taskAction) {
            var durationMarginInMilliseconds = $scope.scopeModal.durationMargin;
            if ($scope.scopeModal.selectedDuration.value = 0)
                durationMarginInMilliseconds = $scope.scopeModal.durationMargin * 1000;

            var timeMarginInMilliSeconds = $scope.scopeModal.timeMargin;
            if ($scope.scopeModal.selectedTimeDuration.value = 0)
                timeMarginInMilliSeconds = $scope.scopeModal.timeMargin * 1000;


            
            var timeOffset = "00:" + $scope.scopeModal.timeOffset.Hour + ":" + $scope.scopeModal.timeOffset.Minute;
            var executionInformation = {
                $type: "CDRComparison.BP.Arguments.SettingTaskExecutionInformation, CDRComparison.BP.Arguments",
                Decision: taskAction,
                DurationMarginInMilliSeconds: durationMarginInMilliseconds,
                TimeMarginInMilliSeconds: timeMarginInMilliSeconds,
                TimeOffset:timeOffset
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

    appControllers.controller('CDRComparison_CDRComparisonSettingsTaskController', CDRComparisonSettingsTaskController);
})(appControllers);
