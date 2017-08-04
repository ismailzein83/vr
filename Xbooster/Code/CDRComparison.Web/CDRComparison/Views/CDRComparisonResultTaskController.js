(function (appControllers) {

    "use strict";

    CDRComparisonResultTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'CDRComparison_CDRComparisonAPIService'];

    function CDRComparisonResultTaskController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRComparisonAPIService) {

        var bpTaskId;
        var processInstanceId;

        var tableKey;

        var systemMissingCDRGridAPI;
        var partnerMissingCDRGridAPI;
        var partialMatchCDRGridAPI;
        var disputeCDRGridAPI;
        var systemInvalidCDRGridAPI;
        var partnerInvalidCDRGridAPI;

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

            $scope.scopeModal.onSystemMissingCDRDirectiveReady = function (api) {
                systemMissingCDRGridAPI = api;
                var payload = {
                    IsPartnerCDRs: false,
                    TableKey: tableKey
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingSystemMissingCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, systemMissingCDRGridAPI, payload, setLoader);
            };
            $scope.scopeModal.onPartnerMissingCDRDirectiveReady = function (api) {
                partnerMissingCDRGridAPI = api;
                var payload = {
                    IsPartnerCDRs: true,
                    TableKey: tableKey
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPartnerMissingCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerMissingCDRGridAPI, payload, setLoader);
            };
            $scope.scopeModal.onPartialMatchCDRDirectiveReady = function (api) {
                var payload = {
                    TableKey: tableKey
                };
                partialMatchCDRGridAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPartialMatchCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partialMatchCDRGridAPI, payload, setLoader);

            };

            $scope.scopeModal.onDisputeCDRGridReady = function (api) {
                disputeCDRGridAPI = api;
                var payload = {
                    TableKey: tableKey
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingDisputeCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, disputeCDRGridAPI, payload, setLoader);
            };

            $scope.scopeModal.onSystemInvalidCDRGridReady = function (api) {
                systemInvalidCDRGridAPI = api;
                var systemInvalidCDRGridPayload = {
                    TableKey: tableKey,
                    IsPartnerCDRs: false
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingSystemInvalidCDRGrid = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, systemInvalidCDRGridAPI, systemInvalidCDRGridPayload, setLoader);
            };

            $scope.scopeModal.onPartnerInvalidCDRGridReady = function (api) {
                partnerInvalidCDRGridAPI = api;
                var partnerInvalidCDRGridPayload = {
                    TableKey: tableKey,
                    IsPartnerCDRs: true
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPartnerInvalidCDRGrid = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerInvalidCDRGridAPI, partnerInvalidCDRGridPayload, setLoader);
            };

            $scope.scopeModal.continueTask = function () {
                return executeTask(true);
            };
        }

        function executeTask(taskAction) {
            var executionInformation = {
                $type: "CDRComparison.BP.Arguments.ComparisonResultTaskExecutionInformation, CDRComparison.BP.Arguments",
                Decision: taskAction
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
                if (response && response.TaskData)
                    tableKey = response.TaskData.TableKey;
                loadAllControls();
            })
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([loadSummaryData, setTitle])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }

        function loadSummaryData() {
            return CDRComparison_CDRComparisonAPIService.GetCDRComparisonSummary(tableKey).then(function (response) {
                if (response != null) {
                    $scope.scopeModal.systemCDRsCount = response.SystemCDRsCount;
                    $scope.scopeModal.partnerCDRsCount = response.PartnerCDRsCount;
                    $scope.scopeModal.systemMissingCDRsCount = response.SystemMissingCDRsCount;
                    $scope.scopeModal.partnerMissingCDRsCount = response.PartnerMissingCDRsCount;
                    $scope.scopeModal.partialMatchCDRsCount = response.PartialMatchCDRsCount;
                    $scope.scopeModal.disputeCDRsCount = response.DisputeCDRsCount;
                    $scope.scopeModal.systemInvalidCDRsCount = response.SystemInvalidCDRsCount;
                    $scope.scopeModal.partnerInvalidCDRsCount = response.PartnerInvalidCDRsCount;

                    $scope.scopeModal.durationOfSystemCDRs = response.DurationOfSystemCDRs;
                    $scope.scopeModal.durationOfPartnerCDRs = response.DurationOfPartnerCDRs;
                    $scope.scopeModal.durationOfSystemMissingCDRs = response.DurationOfSystemMissingCDRs;
                    $scope.scopeModal.durationOfPartnerMissingCDRs = response.DurationOfPartnerMissingCDRs;
                    $scope.scopeModal.durationOfSystemPartialMatchCDRs = response.DurationOfSystemPartialMatchCDRs;
                    $scope.scopeModal.durationOfPartnerPartialMatchCDRs = response.DurationOfPartnerPartialMatchCDRs;
                    $scope.scopeModal.totalDurationDifferenceOfPartialMatchCDRs = response.TotalDurationDifferenceOfPartialMatchCDRs;
                    $scope.scopeModal.durationOfSystemDisputeCDRs = response.DurationOfSystemDisputeCDRs;
                    $scope.scopeModal.durationOfPartnerDisputeCDRs = response.DurationOfPartnerDisputeCDRs;
                    $scope.scopeModal.systemInvalidCDRsDuration = response.SystemInvalidCDRsDuration;
                    $scope.scopeModal.partnerInvalidCDRsDuration = response.PartnerInvalidCDRsDuration;
                }
            });
        }
        function setTitle() {
            $scope.title = "CDR Comparison Result";
        }
    }

    appControllers.controller('CDRComparison_CDRComparisonResultTaskController', CDRComparisonResultTaskController);
})(appControllers);
