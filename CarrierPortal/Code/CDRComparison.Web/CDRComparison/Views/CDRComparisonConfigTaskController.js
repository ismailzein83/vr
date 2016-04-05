(function (appControllers) {

    'use strict';

    CDRComparisonConfigTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'CDRComparison_CDRSourceConfigAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function CDRComparisonConfigTaskController($scope, BusinessProcess_BPTaskAPIService, CDRComparison_CDRSourceConfigAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var bpTaskId;
        var bpInstanceId;

        var systemConfigId;
        var partnerConfigId;

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

            $scope.scopeModel = {};

            $scope.scopeModel.isSystemConfigNameDisabled = false;
            $scope.scopeModel.isPartnerConfigNameDisabled = false;

            $scope.scopeModel.continueTask = function () {
                return executeTask(true);
            }
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (response) {
                bpInstanceId = response.bpInstanceId;
                if (response != null && response.TaskData != null) {
                    systemConfigId = response.SystemCDRSourceConfigId;
                    partnerConfigId = response.PartnerCDRSourceConfigId;
                }
                loadAllControls();
            })
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, getSystemConfigName, getPartnerConfigName]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = 'CDR Comparison Configuration';
        }

        function getSystemConfigName() {
            var systemConfigNameGetDeferred = UtilsService.createPromiseDeferred();

            if (systemConfigId != undefined) {
                CDRComparison_CDRSourceConfigAPIService.GetCDRSourceConfig(systemConfigId).then(function (response) {
                    if (response != null) {
                        $scope.scopeModel.systemConfigName = response.Name;
                        $scope.scopeModel.isSystemConfigNameDisabled = true;
                    }
                }).catch(function (error) {
                    systemConfigNameGetDeferred.reject(error);
                });
            }
            else {
                systemConfigNameGetDeferred.resolve();
            }
            
            return systemConfigNameGetDeferred.promise;
        }

        function getPartnerConfigName() {
            var partnerConfigNameGetDeferred = UtilsService.createPromiseDeferred();

            if (partnerConfigId != undefined) {
                CDRComparison_CDRSourceConfigAPIService.GetCDRSourceConfig(partnerConfigId).then(function (response) {
                    if (response != null) {
                        $scope.scopeModel.partnerConfigName = response.Name;
                        $scope.scopeModel.isPartnerConfigNameDisabled = true;
                    }
                }).catch(function (error) {
                    partnerConfigNameGetDeferred.reject(error);
                });
            }
            else {
                partnerConfigNameGetDeferred.resolve();
            }

            return partnerConfigNameGetDeferred.promise;
        }

        function executeTask(taskAction) {

            var executionInformation = {
                $type: 'CDRComparison.BP.Arguments.CDRComparisonConfigTaskExecutionInformation, CDRComparison.BP.Arguments',
                SystemCDRSourceConfigName: $scope.scopeModel.systemConfigName,
                PartnerCDRSourceConfigName: $scope.scopeModel.partnerConfigName
            };

            var input = {
                $type: 'Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities',
                TaskId: bpTaskId,
                ExecutionInformation: executionInformation
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('CDRComparison_CDRComparisonConfigTaskController', CDRComparisonConfigTaskController);

})(appControllers);