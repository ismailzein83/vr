(function (appControllers) {

	'use strict';

	VRWorkflowArgumentEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_VRWorkflowAPIService'];

	function VRWorkflowArgumentEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, BusinessProcess_VRWorkflowService, BusinessProcess_VRWorkflowAPIService) {

		var vrWorkflowArgumentEntity;
		var vrWorkflowArgumentNames = []; //for validation
		var isEditMode;

		var argumentDirectionSelectorAPI;
		var argumentDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var variableTypeSelectorAPI;
		var variableTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined) {
				vrWorkflowArgumentEntity = parameters.vrWorkflowArgumentEntity;
				vrWorkflowArgumentNames = parameters.vrWorkflowArgumentNames;
			}

			isEditMode = (vrWorkflowArgumentEntity != undefined);
		}

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.onArgumentDirectionSelectorReady = function (api) {
				argumentDirectionSelectorAPI = api;
				argumentDirectionSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onVariableTypeSelectorReady = function (api) {
			    variableTypeSelectorAPI = api;
			    variableTypeSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.isArgumentNameValid = function () {
				var argumentName = ($scope.scopeModel.name != undefined) ? $scope.scopeModel.name.toLowerCase() : null;
				if (isEditMode && argumentName == vrWorkflowArgumentEntity.Name.toLowerCase())
					return null;

				if ($scope.isVariableNameReserved != undefined && $scope.isVariableNameReserved(argumentName))
					return 'Same argument name already exists';

				//if (UtilsService.contains(vrWorkflowArgumentNames, argumentName))
				//    return 'Same argument name already exists';

				return null;
			};

			$scope.scopeModel.save = function () {
				return isEditMode ? updateVRWorkflowArgument() : addVRWorkflowArgument();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {

			function setTitle() {
				if (isEditMode && vrWorkflowArgumentEntity != undefined)
					$scope.title = UtilsService.buildTitleForUpdateEditor(vrWorkflowArgumentEntity.Name, 'Workflow Argument');
				else
					$scope.title = UtilsService.buildTitleForAddEditor('Workflow Argument');
			}

			function loadStaticData() {
				if (vrWorkflowArgumentEntity != undefined) {
					$scope.scopeModel.name = vrWorkflowArgumentEntity.Name;
				}
			}

			function loadArgumentDirectionSelector() {
				var argumentDirectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

				argumentDirectionSelectorReadyDeferred.promise.then(function () {
					var argumentDirectionSelectorPayload;
					if (vrWorkflowArgumentEntity != undefined) {
						argumentDirectionSelectorPayload = { selectedIds: vrWorkflowArgumentEntity.Direction };
					}
					VRUIUtilsService.callDirectiveLoad(argumentDirectionSelectorAPI, argumentDirectionSelectorPayload, argumentDirectionSelectorLoadDeferred);
				});

				return argumentDirectionSelectorLoadDeferred.promise;
			}

			function loadVariableTypeDirective() {
				var variableTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

				variableTypeSelectorReadyDeferred.promise.then(function () {

					var variableTypeDirectivePayload = { selectIfSingleItem: true };
					if (vrWorkflowArgumentEntity != undefined && vrWorkflowArgumentEntity.Type != undefined) {
					    variableTypeDirectivePayload.variableType = vrWorkflowArgumentEntity.Type;
					}
					VRUIUtilsService.callDirectiveLoad(variableTypeSelectorAPI, variableTypeDirectivePayload, variableTypeDirectiveLoadDeferred);
				});

				return variableTypeDirectiveLoadDeferred.promise;
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadArgumentDirectionSelector, loadVariableTypeDirective]).then(function () {

			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			});
		}

		function addVRWorkflowArgument() {
			var vrWorkflowArgumentObj = buildVRWorkflowArgumentObjFromScope();
			if ($scope.onVRWorkflowArgumentAdded != undefined) {
				$scope.onVRWorkflowArgumentAdded(vrWorkflowArgumentObj);
			}
			$scope.modalContext.closeModal();
		}

		function updateVRWorkflowArgument() {
			var vrWorkflowArgumentObj = buildVRWorkflowArgumentObjFromScope();
			if ($scope.onVRWorkflowArgumentUpdated != undefined) {
				$scope.onVRWorkflowArgumentUpdated(vrWorkflowArgumentObj);
			}
			$scope.modalContext.closeModal();
		}

		function buildVRWorkflowArgumentObjFromScope() {
			return {
				VRWorkflowArgumentId: vrWorkflowArgumentEntity != undefined ? vrWorkflowArgumentEntity.VRWorkflowArgumentId : UtilsService.guid(),
				Name: $scope.scopeModel.name,
				Direction: argumentDirectionSelectorAPI.getSelectedIds(),
				Type: variableTypeSelectorAPI.getData()
			};
		}
	}

	appControllers.controller('VR_Workflow_VRWorkflowArgumentEditorController', VRWorkflowArgumentEditorController);
})(appControllers);