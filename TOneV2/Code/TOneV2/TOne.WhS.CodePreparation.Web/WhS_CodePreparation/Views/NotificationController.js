(function (appControllers) {

    'use strict';

    NotificationController.$inject = ['$scope', 'WhS_BE_CarrierAccountAPIService', 'WhS_Sales_CustomerSelectionTypeEnum', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function NotificationController($scope, WhS_BE_CarrierAccountAPIService, WhS_Sales_CustomerSelectionTypeEnum, BusinessProcess_BPTaskAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var taskId;
        var sellingNumberPlanId;

        var gridReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.customers = [];

            $scope.scopeModel.onGridReady = function (api) {
            	gridReadyDeferred.resolve();
            };

            $scope.scopeModel.selectAll = function () {
            	toggleSelection(true);
            };
            $scope.scopeModel.deselectAll = function () {
            	toggleSelection(false);
            };

            $scope.scopeModel.sendMail = function () {
            	var customerIds = getSelectedCustomerIds();
                return executeTask(customerIds, true);
            };
            $scope.scopeModel.skip = function () {
                return executeTask(undefined, false);
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            getSellingNumberPlanId().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }

        function getSellingNumberPlanId() {
            return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
                if (response != null && response.TaskData != null) {
                    sellingNumberPlanId = response.TaskData.SellingNumberPlanId;
                }
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = 'Notify Customers';
        }
        function loadGrid() {

        	var promises = [];
        	promises.push(gridReadyDeferred.promise);

        	var getCustomersPromise = WhS_BE_CarrierAccountAPIService.GetCustomersBySellingNumberPlanId(sellingNumberPlanId).then(function (response) {
        		if (response != null) {
        			for (var i = 0; i < response.length; i++) {
        				$scope.scopeModel.customers.push(response[i]);
        			}
        			toggleSelection(true);
        		}
        	});
        	promises.push(getCustomersPromise);

        	return UtilsService.waitMultiplePromises(promises);
        }

        function executeTask(customerIds, taskAction) {

            $scope.scopeModel.isLoading = true;

            var executionInformation = {
                $type: "TOne.WhS.CodePreparation.BP.Arguments.Tasks.NotificationTaskExecutionInforamtion, TOne.WhS.CodePreparation.BP.Arguments",
                CustomerIds: customerIds,
                Decision: taskAction
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: taskId,
                ExecutionInformation: executionInformation
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function getSelectedCustomerIds() {
        	var customerIds = [];
        	for (var i = 0; i < $scope.scopeModel.customers.length; i++) {
        		if ($scope.scopeModel.customers[i].isSelected)
        			customerIds.push($scope.scopeModel.customers[i].CarrierAccountId);
        	}
        	return customerIds;
        }
        function toggleSelection(toggleValue) {
        	for (var i = 0; i < $scope.scopeModel.customers.length; i++)
        		$scope.scopeModel.customers[i].isSelected = toggleValue;
        }
    }

    appControllers.controller('WhS_CP_NotificationController', NotificationController);

})(appControllers);