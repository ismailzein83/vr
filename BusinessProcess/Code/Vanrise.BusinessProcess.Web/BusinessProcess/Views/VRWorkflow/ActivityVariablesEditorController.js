(function (appControllers) {

	"use strict";

	VRWorkflowVariableEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService'];

	function VRWorkflowVariableEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

		var vrWorkflowArgumentEditorRuntimeDict;

		var variables;
		var variablesTypeDescription;
		var activityVariableGridAPI;
		var activityVariableGridReadyDeferred = UtilsService.createPromiseDeferred();

		var parentVariables;
		var parentVariablesTypeDescription;
		var parentActivitiesVariableGridAPI;
		var parentActivitiesVariableGridReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined) {
				variables = parameters.Variables;
				parentVariables = parameters.ParentVariables;
			}
		}

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.onActivityVariablesGridReady = function (api) {
				activityVariableGridAPI = api;
				activityVariableGridReadyDeferred.resolve();
			};

			$scope.scopeModel.onParentActivitiesVariablesGridReady = function (api) {
				parentActivitiesVariableGridAPI = api;
				parentActivitiesVariableGridReadyDeferred.resolve();
			};

			$scope.scopeModel.saveVariables = function () {
				if ($scope.onSaveVariables != undefined && activityVariableGridAPI != undefined)
					$scope.onSaveVariables(activityVariableGridAPI.getData());
				$scope.modalContext.closeModal();
			};

			$scope.scopeModel.close = function () {
				$scope.reserveVariableNames(variables);
				var gridVariables = activityVariableGridAPI.getData();
				
				if (gridVariables != undefined && gridVariables.length > 0 && $scope.eraseVariableName != undefined) {
					for (var i = 0; i < gridVariables.length; i++) {
						if (variables.indexOf(gridVariables[i]) < 0)
							$scope.eraseVariableName(gridVariables[i].Name)
					}
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
				$scope.title = UtilsService.buildTitleForAddEditor('Variables');
			}

			function loadActivityVariablesGrid() {
				var activityVariablesGridLoadDeferred = UtilsService.createPromiseDeferred();
				var promises = [];
				promises.push(activityVariableGridReadyDeferred.promise);
				promises.push(BusinessProcess_VRWorkflowAPIService.GetVRWorkflowVariablesTypeDescription(variables).then(function (response) { variablesTypeDescription = response; }));

				UtilsService.waitMultiplePromises(promises).then(function () {
					var activityVariablesGridPayload = {
						vrWorkflowVariables: variables,
						reserveVariableName: $scope.reserveVariableName,
						eraseVariableName: $scope.eraseVariableName,
						isVariableNameReserved: $scope.isVariableNameReserved,
						vRWorkflowVariablesTypeDescription: variablesTypeDescription
						//vrWorkflowArgumentEditorRuntimeDict: vrWorkflowArgumentEditorRuntimeDict
					};
					VRUIUtilsService.callDirectiveLoad(activityVariableGridAPI, activityVariablesGridPayload, activityVariablesGridLoadDeferred);
				});

				return activityVariablesGridLoadDeferred.promise;
			}

			function loadParentActivityVariablesGrid() {
				var parentActivityVariablesGridLoadDeferred = UtilsService.createPromiseDeferred();
				var promises = [];
				promises.push(parentActivitiesVariableGridReadyDeferred.promise);
				promises.push(BusinessProcess_VRWorkflowAPIService.GetVRWorkflowVariablesTypeDescription(parentVariables).then(function (response) { parentVariablesTypeDescription = response; }));

				UtilsService.waitMultiplePromises(promises).then(function () {
					var parentActivityVariablesGridPayload = {
						vrWorkflowVariables: parentVariables,
						vRWorkflowVariablesTypeDescription: parentVariablesTypeDescription
						//vrWorkflowArgumentEditorRuntimeDict: vrWorkflowArgumentEditorRuntimeDict
					};
					VRUIUtilsService.callDirectiveLoad(parentActivitiesVariableGridAPI, parentActivityVariablesGridPayload, parentActivityVariablesGridLoadDeferred);
				});

				return parentActivityVariablesGridLoadDeferred.promise;
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadActivityVariablesGrid, loadParentActivityVariablesGrid]).then(function () {
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
	}
	appControllers.controller('BusinessProcess_VRWorkflow_ActivityVariablesEditorController', VRWorkflowVariableEditorController);
})(appControllers);