(function (appControllers) {

    "use strict";

    CDRComparisonSettingsTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'CDRComparison_CDRComparisonAPIService', 'CDRComparison_TimeUnitEnum', 'CDRComparison_CDRComparisonService', 'CDRComparison_CDRSourceConfigAPIService'];

    function CDRComparisonSettingsTaskController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRComparisonAPIService, CDRComparison_TimeUnitEnum, CDRComparison_CDRComparisonService, CDRComparison_CDRSourceConfigAPIService) {

        var bpTaskId;
        var processInstanceId;

        var tableKey;
        var partnerCDRSourceConfigId;

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
            $scope.scopeModal.timeMargin = 0;
            $scope.scopeModal.durationMargin = 0;
            $scope.scopeModal.durations = UtilsService.getArrayEnum(CDRComparison_TimeUnitEnum);
            $scope.scopeModal.selectedTimeMarginTimeUnit = CDRComparison_TimeUnitEnum.Seconds;
            $scope.scopeModal.selectedDurationMarginTimeUnit = CDRComparison_TimeUnitEnum.Seconds;
            $scope.scopeModal.timeOffset = '00:00:00';
            $scope.scopeModal.compareCGPN = false;
            $scope.scopeModal.openHelper = function () {
                var onTimeOffsetSelected = function (timeOffset) {
                    $scope.scopeModal.timeOffset = timeOffset;
                };
                CDRComparison_CDRComparisonService.openTimeOffsetHelper(onTimeOffsetSelected, tableKey);
            };

            $scope.scopeModal.validateTimeOffset = function (value) {
                return UtilsService.validateTimeOffset(value);
            };

            $scope.scopeModal.continueTask = function () {
                return executeTask(true);
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (response) {
                processInstanceId = response.ProcessInstanceId;
                if (response && response.TaskData) {
                    tableKey = response.TaskData.TableKey;
                    partnerCDRSourceConfigId = response.TaskData.PartnerCDRSourceConfigId;
                }
                loadAllControls();
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSettingsConfig]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModal.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = "CDR Comparison Settings";
        }

        function loadSettingsConfig() {
            var configSettingsLoadDeferred = UtilsService.createPromiseDeferred();

            if (partnerCDRSourceConfigId != undefined) {
                CDRComparison_CDRSourceConfigAPIService.GetCDRSourceConfig(partnerCDRSourceConfigId).then(function (response) {
                    if (response != null && response.SettingsTaskExecutionInfo != null) {
                        $scope.scopeModal.durationMargin = response.SettingsTaskExecutionInfo.DurationMargin;
                        $scope.scopeModal.selectedDurationMarginTimeUnit = UtilsService.getEnum(CDRComparison_TimeUnitEnum, 'value', response.SettingsTaskExecutionInfo.DurationMarginTimeUnit);
                        $scope.scopeModal.timeMargin = response.SettingsTaskExecutionInfo.TimeMargin;
                        $scope.scopeModal.selectedTimeMarginTimeUnit = UtilsService.getEnum(CDRComparison_TimeUnitEnum, 'value', response.SettingsTaskExecutionInfo.TimeMarginTimeUnit);
                        $scope.scopeModal.timeOffset = response.SettingsTaskExecutionInfo.TimeOffset;
                        $scope.scopeModal.compareCGPN = response.SettingsTaskExecutionInfo.CompareCGPN; 
                    }
                    configSettingsLoadDeferred.resolve();
                }).catch(function (error) {
                    configSettingsLoadDeferred.reject(error);
                });
            }
            else {
                configSettingsLoadDeferred.resolve();
            }

            return configSettingsLoadDeferred.promise;
        }

        function executeTask(taskAction) {

            var timeOffset = $scope.scopeModal.timeOffset;

            var executionInformation = {
                $type: "CDRComparison.BP.Arguments.SettingTaskExecutionInformation, CDRComparison.BP.Arguments",
                Decision: taskAction,
                DurationMargin: $scope.scopeModal.durationMargin,
                DurationMarginTimeUnit: $scope.scopeModal.selectedDurationMarginTimeUnit.value,
                TimeMargin: $scope.scopeModal.timeMargin,
                TimeMarginTimeUnit: $scope.scopeModal.selectedTimeMarginTimeUnit.value,
                TimeOffset: timeOffset,
                CompareCGPN: $scope.scopeModal.compareCGPN 
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
    }

    appControllers.controller('CDRComparison_CDRComparisonSettingsTaskController', CDRComparisonSettingsTaskController);

})(appControllers);
