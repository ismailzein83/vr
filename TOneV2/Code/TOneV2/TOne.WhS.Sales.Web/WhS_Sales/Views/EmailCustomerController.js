(function (appControllers) {

	'use strict';

	EmailCustomerController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'VRNavigationService', 'VRNotificationService'];

	function EmailCustomerController($scope, BusinessProcess_BPTaskAPIService, VRNavigationService, VRNotificationService) {

		var taskId;

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