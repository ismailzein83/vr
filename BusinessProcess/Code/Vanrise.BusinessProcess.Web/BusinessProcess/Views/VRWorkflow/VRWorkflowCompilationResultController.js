VRWorkflowCompilationResult.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService','BusinessProcess_VRWorkflowAPIService'];

function VRWorkflowCompilationResult($scope, VRNavigationService, UtilsService, VRNotificationService, BusinessProcess_VRWorkflowAPIService) {
	var mainGridAPI;
	var workflowObj;
	loadParameters();
	defineScope();
	load();
	function loadParameters() {
		var parameters = VRNavigationService.getParameters($scope);
		if (parameters != null && parameters.errorMessages != undefined) {
			workflowObj = parameters.workflowObj;
			$scope.dataSrouce = [];
			for (var i = 0; i < parameters.errorMessages.length; i++) {
				$scope.dataSrouce.push({ Error: parameters.errorMessages[i] });
			}
		}
	}

	function defineScope() {
		$scope.close = function () {
			$scope.modalContext.closeModal();
		};
		$scope.export = function () {

			return BusinessProcess_VRWorkflowAPIService.ExportCompilationResult(workflowObj).then(function (response) {
				UtilsService.downloadFile(response.data, response.headers);
			});
		};
		$scope.title = 'Workflow Compilation Result';
		$scope.onMainGridReady = function (api) {
			mainGridAPI = api;
		};
	}

	function load() {
	}
}
appControllers.controller('BusinessProcess_VR_WorkflowCompilationResultController', VRWorkflowCompilationResult);
