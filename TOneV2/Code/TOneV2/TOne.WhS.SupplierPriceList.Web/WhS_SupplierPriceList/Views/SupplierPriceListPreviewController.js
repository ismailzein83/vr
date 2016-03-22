(function (appControllers) {

    "use strict";

    supplierPriceListPreviewController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function supplierPriceListPreviewController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var bpTaskId;
        var processInstanceId;

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

            $scope.onZonePreviewDirectiveReady = function (api) {
                zonePreviewDirectiveAPI = api;
                zonePreviewReadyPromiseDeferred.resolve();
            }

            $scope.onCodePreviewDirectiveReady = function (api) {
                codePreviewDirectiveAPI = api;
                codePreviewReadyPromiseDeferred.resolve();
            }

            $scope.onRatePreviewDirectiveReady = function (api) {
                ratePreviewDirectiveAPI = api;
                ratePreviewReadyPromiseDeferred.resolve();
            }

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.startTask = function () {
                executeTask(true);
            }

            $scope.cancelTask = function () {
                executeTask(false);
            }

               
        }

        function executeTask(taskAction) {
            var executionInformation = {
                $type: "TOne.WhS.SupplierPriceList.BP.Arguments.Tasks.PreviewTaskExecutionInformation, TOne.WhS.SupplierPriceList.BP.Arguments",
                ContinueExecution: taskAction
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: bpTaskId,
                Notes: $scope.notes,
                Decision: 'Dummy Decision',
                ExecutionInformation: executionInformation
            };

            BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });
        }


        function load() {
            $scope.isLoading = true;
            BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (response) {
                processInstanceId = response.ProcessInstanceId;
                loadAllControls();
            })
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([loadZonePreview, loadCodePreview, loadRatePreview])
                          .catch(function (error) {
                              VRNotificationService.notifyExceptionWithClose(error, $scope);
                          })
                          .finally(function () {
                              $scope.isLoading = false;
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


    }

    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewController', supplierPriceListPreviewController);
})(appControllers);
