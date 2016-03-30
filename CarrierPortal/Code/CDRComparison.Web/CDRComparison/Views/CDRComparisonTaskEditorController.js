(function (appControllers) {

    "use strict";

    CDRComparisonTaskEditorController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','CDRComparison_CDRComparisonAPIService'];

    function CDRComparisonTaskEditorController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRComparisonAPIService) {
        var bpTaskId;
        var processInstanceId;

        var missingCDRGridAPI;
        var partialMatchCDRGridAPI;
        var disputeCDRGridAPI;

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

            $scope.scopeModal = {
                missingTabObject: {},
                partialTabObject: {},
                disputeTabObject: {}
                };

            $scope.scopeModal.onMissingCDRDirectiveReady = function (api) {
                missingCDRGridAPI = api;
                var payload = {
                    IsPartnerCDRs: true
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingMissingCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, missingCDRGridAPI, payload, setLoader);
            }

            $scope.scopeModal.onPartialMatchCDRDirectiveReady = function (api) {
                var payload = {};
                partialMatchCDRGridAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPartialMatchCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partialMatchCDRGridAPI, payload, setLoader);
             
            }

            $scope.scopeModal.onDisputeCDRGridReady = function (api) {
                disputeCDRGridAPI = api;
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
            var executionInformation = {
                $type: "CDRComparison.BP.Arguments.Tasks.CDRComparisonTaskExecutionInformation, CDRComparison.BP.Arguments.Tasks",
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
            return CDRComparison_CDRComparisonAPIService.GetCDRComparisonSummary().then(function (response) {
                if (response)
                {
                    $scope.scopeModal.allCDRsCount = response.AllCDRsCount;
                    $scope.scopeModal.missingCDRsCount = response.MissingCDRsCount;
                    $scope.scopeModal.partialMatchCDRsCount = response.PartialMatchCDRsCount;
                    $scope.scopeModal.disputeCDRsCount = response.DisputeCDRsCount;
                }
                
            });
        }

    }

    appControllers.controller('CDRComparison_CDRComparisonTaskEditorController', CDRComparisonTaskEditorController);
})(appControllers);
