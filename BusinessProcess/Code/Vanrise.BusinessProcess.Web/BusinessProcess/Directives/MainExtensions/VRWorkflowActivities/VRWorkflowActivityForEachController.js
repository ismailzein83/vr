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

			$scope.scopeModel.isVariableNameValid = function () {
				if ($scope.scopeModel.iterationVariableName == undefined)
					return null;

				var variableName = $scope.scopeModel.iterationVariableName.toLowerCase();

				if (iterationVariableName != undefined && variableName == iterationVariableName.toLowerCase())
					return null;

				if (context.isVariableNameReserved != undefined && context.isVariableNameReserved(variableName))
					return 'Same variable name already exists';

				return null;
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

			//function getContainerGetChildContext() {
			//	var containerChildContext = getChildContext();
			//	containerChildContext.inEditor = true;
			//	return containerChildContext;
			//}
			function getContainerGetChildContext() {
				var containerChildContext = getChildContext();
				var newContainerChildContext = {};
				newContainerChildContext.addToList = containerChildContext.addToList;
				newContainerChildContext.doesActivityhaveErrors = containerChildContext.doesActivityhaveErrors;
				newContainerChildContext.eraseVariableName = containerChildContext.eraseVariableName;
				newContainerChildContext.getWorkflowArguments = containerChildContext.getWorkflowArguments;
				newContainerChildContext.isVariableNameReserved = containerChildContext.isVariableNameReserved;
				newContainerChildContext.removeFromList = containerChildContext.removeFromList;
				newContainerChildContext.reserveVariableName = containerChildContext.reserveVariableName;
				newContainerChildContext.reserveVariableNames = containerChildContext.reserveVariableNames;
				newContainerChildContext.showErrorWarningIcon = containerChildContext.showErrorWarningIcon;
				newContainerChildContext.vrWorkflowId = containerChildContext.vrWorkflowId;
				newContainerChildContext.inEditor = true;
				newContainerChildContext.getParentVariables = function () {
					var parentVariables = containerChildContext.getParentVariables();

					if (parentVariables != undefined && parentVariables.length > 0) {
						if (iterationVariableName != $scope.scopeModel.iterationVariableName) {
							if (iterationVariableName != undefined && iterationVariableName.length > 0) {
								var variableIndex = UtilsService.getItemIndexByVal(parentVariables, iterationVariableName, "Name");
								if (variableIndex >= 0)
									parentVariables.splice(variableIndex, 1);
							}

							if ($scope.scopeModel.iterationVariableName != undefined && $scope.scopeModel.iterationVariableName.length > 0)
								parentVariables.unshift({ Name: $scope.scopeModel.iterationVariableName });
						}
					}

					else parentVariables = [{ Name: $scope.scopeModel.iterationVariableName }];
					return parentVariables;
				};
				return newContainerChildContext;
			}

			function loadWorkflowContainer() {
				var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();

				workflowContainerReadyPromiseDeferred.promise.then(function () {
					var payload = {
						vRWorkflowActivity: activity,
						getChildContext: getContainerGetChildContext
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
			var oldIterationVariableName = (iterationVariableName != undefined) ? iterationVariableName.toLowerCase() : undefined;
			var newIterationVariableName = ($scope.scopeModel.iterationVariableName != undefined) ? $scope.scopeModel.iterationVariableName.toLowerCase() : undefined;

			if (oldIterationVariableName != newIterationVariableName) {
				if (context.reserveVariableName != undefined && newIterationVariableName != undefined)
					context.reserveVariableName(newIterationVariableName);

				if (context.eraseVariableName != undefined && oldIterationVariableName != undefined)
					context.eraseVariableName(oldIterationVariableName);
			}

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