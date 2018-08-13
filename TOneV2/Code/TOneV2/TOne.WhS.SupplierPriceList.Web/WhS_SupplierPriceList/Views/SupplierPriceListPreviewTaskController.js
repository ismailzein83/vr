(function (appControllers) {

    "use strict";

    supplierPriceListPreviewTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'WhS_SupPL_PreviewChangeTypeEnum', 'WhS_SupPL_PreviewGroupedBy', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function supplierPriceListPreviewTaskController($scope, BusinessProcess_BPTaskAPIService, WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var bpTaskId;
        var processInstanceId;

        var taskData;

        var supplierPriceListPreviewSectionApi;
        var supplierPriceListPreviewSectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onSupplierPriceListPreviewSectionReady = function (api) {
                supplierPriceListPreviewSectionApi = api;
                supplierPriceListPreviewSectionReadyPromiseDeferred.resolve();
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
                taskData = response.TaskData;
                loadAllControls();
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadSupplierPriceListPreviewSection])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }
        function setTitle() {
            $scope.title = 'Supplier Pricelist Preview';
        }
        function loadSupplierPriceListPreviewSection() {
            var loadSupplierPriceListPreviewSectionPromiseDeferred = UtilsService.createPromiseDeferred();


           supplierPriceListPreviewSectionReadyPromiseDeferred.promise.then(function () {
                var SupplierPriceListPreviewSectionPayload = {
                    processInstanceId: processInstanceId,
                    fileId: taskData.FileId,
                    supplierPricelistType: taskData.SupplierPricelistType,
                    pricelistDate: taskData.PricelistDate,
                    currencyId: taskData.CurrencyId,
                    supplierId: taskData.SupplierId,
                    requireWarningConfirmation: true,
                };
                VRUIUtilsService.callDirectiveLoad(supplierPriceListPreviewSectionApi, SupplierPriceListPreviewSectionPayload, loadSupplierPriceListPreviewSectionPromiseDeferred);

            });
            return loadSupplierPriceListPreviewSectionPromiseDeferred.promise;
        }
    }
    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewTaskController', supplierPriceListPreviewTaskController);
})(appControllers);