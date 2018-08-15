(function (appControllers) {

	"use strict";

	ForEachEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService'];

	function ForEachEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService) {

		var iterationVariableTypeSelectorAPI;
		var iterationVariableTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var workflowContainerAPI;
		var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var dragdropsetting;
		var list;
		var iterationVariableName;
		var iterationVariableType;
		var activity;
		var context;
		var getChildContext;
		var isNew;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined) {
				dragdropsetting = parameters.dragdropsetting;
				context = parameters.context;
				isNew = parameters.isNew;
				getChildContext = parameters.getChildContext;
				if (parameters.foreachObj != undefined) {
					list = parameters.foreachObj.List;
					iterationVariableName = parameters.foreachObj.IterationVariableName;
					iterationVariableType = parameters.foreachObj.IterationVariableType;
					activity = parameters.foreachObj.Activity;
				}
			}
		}

		function defineScope() {
			$scope.scopeModel = {};
			$scope.modalContext.onModalHide = function () {
				if ($scope.remove != undefined && isNew == true) {
					$scope.remove();
				}
			};
			$scope.scopeModel.activityConfigs = [];
			$scope.scopeModel.onIterationVariableTypeSelectorReady = function (api) {
				iterationVariableTypeSelectorAPI = api;
				iterationVariableTypeSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onWorkflowContainerReady = function (api) {
				workflowContainerAPI = api;
				workflowContainerReadyPromiseDeferred.resolve();
			};

			$scope.scopeModel.saveActivity = function () {
				return updateActivity();
			};

			$scope.scopeModel.close = function () {
				if ($scope.remove != undefined && isNew == true) {
					$scope.remove();
				}
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {

			function setTitle() {
				$scope.title = "Edit ForEach";
			}

			function loadStaticData() {
				$scope.scopeModel.dragdropsetting = dragdropsetting;
				$scope.scopeModel.list = list;
				$scope.scopeModel.iterationVariableName = iterationVariableName;
				$scope.scopeModel.context = context;
			}

			function loadVariableTypeSelector() {
				var variableTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

				iterationVariableTypeSelectorReadyDeferred.promise.then(function () {

					var variableTypeDirectivePayload = { selectIfSingleItem: true };
					if (iterationVariableType != undefined) {
						variableTypeDirectivePayload.variableType = iterationVariableType;
					}
					VRUIUtilsService.callDirectiveLoad(iterationVariableTypeSelectorAPI, variableTypeDirectivePayload, variableTypeDirectiveLoadDeferred);
				});

				return variableTypeDirectiveLoadDeferred.promise;
			}

			function loadWorkflowContainer() {
				var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();

				workflowContainerReadyPromiseDeferred.promise.then(function () {
					var payload = {
						vRWorkflowActivity: activity,
						getChildContext: getChildContext,
						inEditor: true
					};
					VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, payload, workflowContainerLoadDeferred);
				});
				return workflowContainerLoadDeferred.promise;
			}

			function loadWorkflowActivityExtensionConfigs() {
				return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowActivityExtensionConfigs().then(function (response) {
					if (response != null) {
						for (var i = 0; i < response.length; i++) {
							$scope.scopeModel.activityConfigs.push(response[i]);
						}
					}
				});
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVariableTypeSelector, loadWorkflowActivityExtensionConfigs, loadWorkflowContainer]).then(function () {
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function updateActivity() {
			$scope.scopeModel.isLoading = true;
			var updatedObject = {
				List: $scope.scopeModel.list,
				IterationVariableName: $scope.scopeModel.iterationVariableName,
				IterationVariableType: iterationVariableTypeSelectorAPI.getData(),
				Activity: (workflowContainerAPI != undefined) ? workflowContainerAPI.getData() : null
			};

			if ($scope.onActivityUpdated != undefined) {
				$scope.onActivityUpdated(updatedObject);
			}
			isNew = false;
			$scope.scopeModel.isLoading = false;
			$scope.modalContext.closeModal();
		}
	}

	appControllers.controller('BusinessProcess_VR_WorkflowActivityForEachController', ForEachEditorController);
})(appControllers);