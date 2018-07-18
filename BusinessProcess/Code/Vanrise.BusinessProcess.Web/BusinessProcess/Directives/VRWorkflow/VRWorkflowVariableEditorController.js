(function (appControllers) {

	'use strict';

	VRWorkflowVariableEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_VRWorkflowAPIService'];

	function VRWorkflowVariableEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, BusinessProcess_VRWorkflowService, BusinessProcess_VRWorkflowAPIService) {

		var vrWorkflowVariableEntity;
		var isEditMode;

		var argumentVariableTypeDirectiveAPI;
		var argumentVariableTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined) {
				vrWorkflowVariableEntity = parameters.vrWorkflowVariableEntity;
			}

			isEditMode = (vrWorkflowVariableEntity != undefined);
		}

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.onArgumentVariableTypeDirectiveReady = function (api) {
				argumentVariableTypeDirectiveAPI = api;
				argumentVariableTypeDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.isVariableNameValid = function () {
				var variableName = ($scope.scopeModel.name != undefined) ? $scope.scopeModel.name.toLowerCase() : null;
				
				if (isEditMode && variableName == vrWorkflowVariableEntity.Name.toLowerCase())
					return null;

				if ($scope.isVariableNameReserved != undefined && $scope.isVariableNameReserved(variableName))
					return 'Same variable name already exists';

				return null;
			};

			$scope.scopeModel.save = function () {
				return isEditMode ? updateVRWorkflowVariable() : addVRWorkflowVariable();
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
				if (isEditMode && vrWorkflowVariableEntity != undefined)
					$scope.title = UtilsService.buildTitleForUpdateEditor(vrWorkflowVariableEntity.Name, 'Workflow Variable');
				else
					$scope.title = UtilsService.buildTitleForAddEditor('Workflow Variable');
			}

			function loadStaticData() {
				if (vrWorkflowVariableEntity != undefined) {
					$scope.scopeModel.name = vrWorkflowVariableEntity.Name;
				}
			}

			function loadArgumentVariableTypeDirective() {
				var argumentVariableTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

				argumentVariableTypeDirectiveReadyDeferred.promise.then(function () {
					var argumentVariableTypeDirectivePayload;
					if (vrWorkflowVariableEntity != undefined && vrWorkflowVariableEntity.Type != undefined) {
						argumentVariableTypeDirectivePayload = { argumentVariableType: vrWorkflowVariableEntity.Type };
					}
					VRUIUtilsService.callDirectiveLoad(argumentVariableTypeDirectiveAPI, argumentVariableTypeDirectivePayload, argumentVariableTypeDirectiveLoadDeferred);
				});

				return argumentVariableTypeDirectiveLoadDeferred.promise;
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadArgumentVariableTypeDirective]).then(function () {

			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			});
		}

		function addVRWorkflowVariable() {
			var vrWorkflowVariableObj = buildVRWorkflowVariableObjFromScope();
			if ($scope.onVRWorkflowVariableAdded != undefined) {
				$scope.onVRWorkflowVariableAdded(vrWorkflowVariableObj);
			}
			$scope.modalContext.closeModal();
		}

		function updateVRWorkflowVariable() {
			var vrWorkflowVariableObj = buildVRWorkflowVariableObjFromScope();
			if ($scope.onVRWorkflowVariableUpdated != undefined) {
				$scope.onVRWorkflowVariableUpdated(vrWorkflowVariableObj);
			}
			$scope.modalContext.closeModal();
		}

		function buildVRWorkflowVariableObjFromScope() {
			return {
				VRWorkflowVariableId: vrWorkflowVariableEntity != undefined ? vrWorkflowVariableEntity.VRWorkflowVariableId : UtilsService.guid(),
				Name: $scope.scopeModel.name,
				Type: argumentVariableTypeDirectiveAPI.getData()
			};
		}
	}

	appControllers.controller('VR_Workflow_VRWorkflowVariableEditorController', VRWorkflowVariableEditorController);
})(appControllers);