(function (appControllers) {

    "use strict";

    CDRComparisonPreviewTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'CDRComparison_CDRComparisonAPIService'];

    function CDRComparisonPreviewTaskController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, CDRComparison_CDRComparisonAPIService) {
        var bpTaskId;
        var processInstanceId;

        var systemMissingCDRGridAPI;
        var partnerMissingCDRGridAPI;
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
                systemmissingTabObject: {},
                partnerMissingTabObject:{},
                partialTabObject: {},
                disputeTabObject: {}
                };

            $scope.scopeModal.onSystemMissingCDRDirectiveReady = function (api) {
                systemMissingCDRGridAPI = api;
                var payload = {
                    IsPartnerCDRs: false
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingSystemMissingCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, systemMissingCDRGridAPI, payload, setLoader);
            }
            $scope.scopeModal.onPartnerMissingCDRDirectiveReady = function (api) {
                partnerMissingCDRGridAPI = api;
                var payload = {
                    IsPartnerCDRs: true
                };
                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPartnerMissingCDRDirective = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerMissingCDRGridAPI, payload, setLoader);
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
            var executionInformation = {
                $type: "CDRComparison.BP.Arguments.CDRComparisonTaskExecutionInformation, CDRComparison.BP.Arguments",
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

    appControllers.controller('CDRComparison_CDRComparisonPreviewTaskController', CDRComparisonPreviewTaskController);
})(appControllers);
