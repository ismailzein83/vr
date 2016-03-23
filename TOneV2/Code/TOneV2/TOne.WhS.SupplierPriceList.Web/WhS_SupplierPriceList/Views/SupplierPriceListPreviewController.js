(function (appControllers) {

    "use strict";

    supplierPriceListPreviewController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function supplierPriceListPreviewController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var bpTaskId;
        var processInstanceId;

        var validationMessageHistoryGridAPI;
        var validationMessageHistoryReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var zonePreviewDirectiveAPI;
        var zonePreviewReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var codePreviewDirectiveAPI;
        var codePreviewReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var ratePreviewDirectiveAPI;
        var ratePreviewReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModal.onZonePreviewDirectiveReady = function (api) {
                zonePreviewDirectiveAPI = api;
                zonePreviewReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onCodePreviewDirectiveReady = function (api) {
                codePreviewDirectiveAPI = api;
                codePreviewReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onRatePreviewDirectiveReady = function (api) {
                ratePreviewDirectiveAPI = api;
                ratePreviewReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.continueTask = function () {
               return executeTask(true);
            }

            $scope.scopeModal.stopTask = function () {
              return executeTask(false);
            }


            $scope.scopeModal.onValidationMessageHistoryGridReady = function (api) {
                validationMessageHistoryGridAPI = api;
                validationMessageHistoryReadyPromiseDeferred.resolve();
            }

               
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
            })
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([loadZonePreview, loadCodePreview, loadRatePreview, loadValidationMessageHistory])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }

        function loadZonePreview() {
            var loadZonePreviewPromiseDeferred = UtilsService.createPromiseDeferred();

            zonePreviewReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    ProcessInstanceId: processInstanceId
                }
                VRUIUtilsService.callDirectiveLoad(zonePreviewDirectiveAPI, payload, loadZonePreviewPromiseDeferred)
            })
            return loadZonePreviewPromiseDeferred.promise;
        }

        function loadCodePreview() {
            var loadCodePreviewPromiseDeferred = UtilsService.createPromiseDeferred();

            codePreviewReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    ProcessInstanceId: processInstanceId
                }
                VRUIUtilsService.callDirectiveLoad(codePreviewDirectiveAPI, payload, loadCodePreviewPromiseDeferred)
            })
            return loadCodePreviewPromiseDeferred.promise;
        }

        function loadRatePreview() {
            var loadRatePreviewPromiseDeferred = UtilsService.createPromiseDeferred();

            ratePreviewReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    ProcessInstanceId: processInstanceId
                }
                VRUIUtilsService.callDirectiveLoad(ratePreviewDirectiveAPI, payload, loadRatePreviewPromiseDeferred)
            })
            return loadRatePreviewPromiseDeferred.promise;
        }

        function loadValidationMessageHistory() {
            var loadValidationMessageHistoryPromiseDeferred = UtilsService.createPromiseDeferred();

            validationMessageHistoryReadyPromiseDeferred.promise.then(function () {
                var payload ={
                    BPInstanceID: processInstanceId,
                };

                VRUIUtilsService.callDirectiveLoad(validationMessageHistoryGridAPI, payload, loadValidationMessageHistoryPromiseDeferred)
            })

            return loadValidationMessageHistoryPromiseDeferred.promise;
        }

    }

    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewController', supplierPriceListPreviewController);
})(appControllers);
