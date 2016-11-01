(function (appControllers) {

	'use strict';

	EmailController.$inject = ['$scope', 'WhS_BE_CustomerSellingProductAPIService', 'WhS_Sales_CustomerSelectionTypeEnum', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

	function EmailController($scope, WhS_BE_CustomerSellingProductAPIService, WhS_Sales_CustomerSelectionTypeEnum, BusinessProcess_BPTaskAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

		var taskId;
		var sellingProductId;

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

			$scope.scopeModel.customerSelectionTypes = UtilsService.getArrayEnum(WhS_Sales_CustomerSelectionTypeEnum);
			$scope.scopeModel.selectedCustomerSelectionType = WhS_Sales_CustomerSelectionTypeEnum.All;

			$scope.scopeModel.onSelectionChanged = function () {
				if ($scope.scopeModel.selectedCustomerSelectionType == undefined)
					return;
				$scope.scopeModel.showGrid = ($scope.scopeModel.selectedCustomerSelectionType.value != WhS_Sales_CustomerSelectionTypeEnum.All.value)
			};

			$scope.scopeModel.save = function () {
				return executeTask(true);
			};
			$scope.scopeModel.close = function () {
				return executeTask(false);
			};
		}
		function load() {
			$scope.scopeModel.isLoading = true;

			getSellingProductId().then(function () {
				loadAllControls();
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
				$scope.scopeModel.isLoading = false;
			});
		}

		function getSellingProductId() {
			return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
				if (response != null && response.TaskData != null) {
					sellingProductId = response.TaskData.SellingProductId;
				}
			});
		}
		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([loadGrid]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function loadGrid() {
			return WhS_BE_CustomerSellingProductAPIService.GetCustomerNamesBySellingProductId(sellingProductId).then(function (response) {
				if (response != null) {
					for (var i = 0; i < response.length; i++) {
						$scope.scopeModel.customers.push({
							Customer: response[i]
						});
					}
				}
			});
		}

		function executeTask(decision) {

			$scope.scopeModel.isLoading = true;

			var executionInformation = {
				$type: "TOne.WhS.Sales.BP.Arguments.Tasks.EmailTaskExecutionInfo, TOne.WhS.Sales.BP.Arguments",
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

	appControllers.controller('WhS_Sales_EmailController', EmailController);

})(appControllers);