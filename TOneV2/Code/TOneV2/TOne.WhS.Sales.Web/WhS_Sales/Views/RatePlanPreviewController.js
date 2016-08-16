(function (appControllers) {

    'use strict';

    RatePlanPreviewController.$inject = ['$scope', 'WhS_Sales_RatePlanPreviewAPIService', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function RatePlanPreviewController($scope, WhS_Sales_RatePlanPreviewAPIService, BusinessProcess_BPTaskAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var taskId;
        var processInstanceId;

        var ratePreviewGridAPI;
        var ratePreviewGridReadyDeferred = UtilsService.createPromiseDeferred();

        var saleZoneRoutingProductPreviewGridAPI;
        var saleZoneRoutingProductGridReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                taskId = parameters.TaskId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onRatePreviewGridReady = function (api) {
                ratePreviewGridAPI = api;
                ratePreviewGridReadyDeferred.resolve();
            };

            $scope.scopeModel.onSaleZoneRoutingProductPreviewGridReady = function (api) {
                saleZoneRoutingProductPreviewGridAPI = api;
                saleZoneRoutingProductGridReadyDeferred.resolve();
            };

            //$scope.scopeModel.validateDefaultRoutingProduct = function () {
            //    if ($scope.scopeModel.currentDefaultRoutingProductName == null && $scope.scopeModel.newDefaultRoutingProductName == null)
            //        return 'Rate plan does not have a default routing product';
            //    return null;
            //};

            $scope.scopeModel.save = function () {
                return executeTask(true);
            };
            $scope.scopeModel.close = function () {
                return executeTask(false);
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            getProcessInstanceId().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getProcessInstanceId() {
            return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
                if (response != null) {
                    processInstanceId = response.ProcessInstanceId;
                }
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSummary, loadDefaultRoutingProductPreview, loadRatePreviewGrid, loadSaleZoneRoutingProductPreviewGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadSummary() {
            var query = {
                ProcessInstanceId: processInstanceId
            };
            return WhS_Sales_RatePlanPreviewAPIService.GetRatePlanPreviewSummary(query).then(function (response) {
                if (response != null) {
                    $scope.scopeModel.numberOfNewRates = response.NumberOfNewRates;
                    $scope.scopeModel.numberOfIncreasedRates = response.NumberOfIncreasedRates;
                    $scope.scopeModel.numberOfDecreasedRates = response.NumberOfDecreasedRates;
                    $scope.scopeModel.numberOfClosedRates = response.NumberOfClosedRates;

                    $scope.scopeModel.nameOfNewDefaultRoutingProduct = response.NameOfNewDefaultRoutingProduct;
                    $scope.scopeModel.nameOfClosedDefaultRoutingProudct = response.NameOfClosedDefaultRoutingProudct;

                    $scope.scopeModel.numberOfNewSaleZoneRoutingProducts = response.NumberOfNewSaleZoneRoutingProducts;
                    $scope.scopeModel.numberOfClosedSaleZoneRoutingProducts = response.NumberOfClosedSaleZoneRoutingProducts;
                }
            });
        }
        function loadDefaultRoutingProductPreview() {
            var input = { ProcessInstanceId: processInstanceId };
            return WhS_Sales_RatePlanPreviewAPIService.GetDefaultRoutingProductPreview(input).then(function (response) {
                if (response != null) {
                    $scope.scopeModel.currentDefaultRoutingProductName = response.CurrentDefaultRoutingProductName;
                    if (response.IsCurrentDefaultRoutingProductInherited === true)
                        $scope.scopeModel.currentDefaultRoutingProductName += ' (Inherited)';
                    $scope.scopeModel.newDefaultRoutingProductName = response.NewDefaultRoutingProductName;
                    $scope.scopeModel.effectiveOn = new Date(response.EffectiveOn).toDateString();
                }
            });
        }
        function loadRatePreviewGrid() {
            var ratePreviewGridLoadDeferred = UtilsService.createPromiseDeferred();

            ratePreviewGridReadyDeferred.promise.then(function () {
                var ratePreviewGridPayload = {
                    ProcessInstanceId: processInstanceId,
                    ZoneName: null
                };
                VRUIUtilsService.callDirectiveLoad(ratePreviewGridAPI, ratePreviewGridPayload, ratePreviewGridLoadDeferred);
            });

            return ratePreviewGridLoadDeferred.promise;
        }
        function loadSaleZoneRoutingProductPreviewGrid() {
            var saleZoneRoutingProductGridLoadDeferred = UtilsService.createPromiseDeferred();

            saleZoneRoutingProductGridReadyDeferred.promise.then(function () {
                var saleZoneRoutingProductPreviewGridPayload = {
                    ProcessInstanceId: processInstanceId
                };
                VRUIUtilsService.callDirectiveLoad(saleZoneRoutingProductPreviewGridAPI, saleZoneRoutingProductPreviewGridPayload, saleZoneRoutingProductGridLoadDeferred);
            });

            return saleZoneRoutingProductGridLoadDeferred.promise;
        }

        function executeTask(decision) {
            $scope.scopeModel.isLoading = true;

            var executionInformation = {
                $type: "TOne.WhS.Sales.BP.Arguments.Tasks.PreviewTaskExecutionInformation, TOne.WhS.Sales.BP.Arguments",
                Decision: decision
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: taskId,
                ExecutionInformation: executionInformation
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('WhS_Sales_RatePlanPreviewController', RatePlanPreviewController);

})(appControllers);