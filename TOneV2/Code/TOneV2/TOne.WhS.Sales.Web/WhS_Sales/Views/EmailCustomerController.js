(function (appControllers) {

	'use strict';

	EmailCustomerController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRNavigationService', 'VRNotificationService'];

	function EmailCustomerController($scope, BusinessProcess_BPTaskAPIService, UtilsService, VRNavigationService, VRNotificationService) {

		var taskId;
		var taskData;

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
			$scope.title = 'Notify Customer';

			$scope.scopeModel.sendMail = function () {
				return executeTask(true);
			};
			$scope.scopeModel.skip = function () {
				return executeTask(false);
			};
		}
		function load() {
			$scope.scopeModel.isLoading = true;

			getTaskData().then(function () {
				loadAllControls();
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			});
		}

		function getTaskData() {
			return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
				if (response != null) {
					taskData = response.TaskData;
				}
			});
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData]).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			if (taskData == undefined || taskData.CustomerName == undefined)
				return;
			$scope.title += (': ' + taskData.CustomerName);
		}
		function loadStaticData() {
			if (taskData == undefined)
				return;
			$scope.scopeModel.customerName = taskData.CustomerName;
		}

		function executeTask(decision) {

			$scope.scopeModel.isLoading = true;

			var executionInformation = {
				$type: "TOne.WhS.Sales.BP.Arguments.Tasks.EmailCustomerTaskExecutionInformation, TOne.WhS.Sales.BP.Arguments",
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
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
	}

	appControllers.controller('WhS_Sales_EmailCustomerController', EmailCustomerController);

})(appControllers);