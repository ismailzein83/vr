(function (appControllers) {

    "use strict";

    ratePlanSellingZonesWithDefaultratesController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_CountryAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ratePlanSellingZonesWithDefaultratesController($scope, BusinessProcess_BPTaskAPIService, VRNotificationService, VRNavigationService, VRCommon_CountryAPIService, UtilsService, VRUIUtilsService) {

        var taskId;
        var ownerId;
        var ownerType;
        var countryPreviewGridAPI;
        var countryGridReadyDeferred = UtilsService.createPromiseDeferred();
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
            $scope.scopeModel.msg = "Below countries have empty rates, they will be filled by default rate";
            $scope.scopeModel.onCountryWithDefaultRateGridReady = function (api) {
                countryPreviewGridAPI = api;
                countryGridReadyDeferred.resolve();
            };
            $scope.scopeModel.continueTask = function () {
                return executeTask(true);
            };
            $scope.scopeModel.stopTask = function () {
                return executeTask(false);
            };
        }

        function load() {

            $scope.scopeModel.isLoading = true;

            getTaskData().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }
        function getTaskData() {
            return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
                if (response == null || response.TaskData == null)
                    return;
                ownerType = response.TaskData.OwnerType;
                ownerId = response.TaskData.OwnerId;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountryWithDefaultRateGrid]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadCountryWithDefaultRateGrid() {
            var countryGridLoadDeferred = UtilsService.createPromiseDeferred();

            countryGridReadyDeferred.promise.then(function () {
                var countryGridPayload = {
                    OwnerType: ownerType,
                    OwnerId: ownerId
                };
                VRUIUtilsService.callDirectiveLoad(countryPreviewGridAPI, countryGridPayload, countryGridLoadDeferred);
            });

            return countryGridLoadDeferred.promise;
        }

        function executeTask(decision) {
            $scope.scopeModel.isLoading = true;

            var executionInformation = {
                $type: "TOne.WhS.Sales.BP.Arguments.Tasks.SellingZonesWithDefaultRatesTaskExecutionInformation, TOne.WhS.Sales.BP.Arguments",
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

    appControllers.controller('WhS_Sales_RatePlanSellingzonesWithDefaultRatesController', ratePlanSellingZonesWithDefaultratesController);
})(appControllers);
