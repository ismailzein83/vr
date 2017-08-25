﻿(function (appControllers) {

    "use strict";

    supplierPriceListPreviewController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'WhS_SupPL_PreviewChangeTypeEnum', 'WhS_SupPL_PreviewGroupedBy', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function supplierPriceListPreviewController($scope, BusinessProcess_BPTaskAPIService, WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var bpTaskId;
        var processInstanceId;

        var SupplierPriceListPreviewDirectiveApi;
        var SupplierPriceListPreviewDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModal.continueTask = function () {
                return executeTask(true);
            };

            $scope.scopeModal.stopTask = function () {
                return executeTask(false);
            };

            $scope.onSupplierPriceListPreviewDirectiveReady = function (api) {
                SupplierPriceListPreviewDirectiveApi = api;
                SupplierPriceListPreviewDirectiveReadyPromiseDeferred.resolve();
            };

        }

        function executeTask(taskAction) {
            var executionInformation = {
                $type: "TOne.WhS.SupplierPriceList.BP.Arguments.Tasks.PreviewTaskExecutionInformation, TOne.WhS.SupplierPriceList.BP.Arguments",
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
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([loadSupplierPriceListPreviewDirective])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }

        function loadSupplierPriceListPreviewDirective() {
            var loadSupplierPriceListPreviewDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            SupplierPriceListPreviewDirectiveReadyPromiseDeferred.promise.then(function () {
                var SupplierPriceListPreviewDirectivePayload = {
                    processInstanceId: processInstanceId,
                };
                VRUIUtilsService.callDirectiveLoad(SupplierPriceListPreviewDirectiveApi, SupplierPriceListPreviewDirectivePayload, loadSupplierPriceListPreviewDirectivePromiseDeferred);

            });
            return loadSupplierPriceListPreviewDirectivePromiseDeferred.promise;
        }
    }
    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewController', supplierPriceListPreviewController);
})(appControllers);