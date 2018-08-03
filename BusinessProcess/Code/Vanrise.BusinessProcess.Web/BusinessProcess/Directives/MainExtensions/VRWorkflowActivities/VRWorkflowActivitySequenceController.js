(function (appControllers) {

	"use strict";

	SequenceEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService'];

	function SequenceEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService) {

		var context;
		var sequenceSettings;
		var dragdropsetting;

		var sequenceDirectiveAPI;
		var sequenceDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined) {
				context = parameters.context;
				sequenceSettings = parameters.settings;
				dragdropsetting = parameters.dragdropsetting;
			}
		}

		function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.activityConfigs = [];
			$scope.scopeModel.onSequenceDirectiveReady = function (api) {
				sequenceDirectiveAPI = api;
				sequenceDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.saveActivity = function () {
				return updateActivity();
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
				$scope.title = "Edit Sequence";
			}

			function loadStaticData() {
				$scope.scopeModel.dragdropsetting = dragdropsetting;
			}

			function loadSequenceDirective() {
				var sequenceDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

				sequenceDirectiveReadyDeferred.promise.then(function () {
					var payload = {
						Context: context,
						Settings: sequenceSettings
					};
					VRUIUtilsService.callDirectiveLoad(sequenceDirectiveAPI, payload, sequenceDirectiveLoadDeferred);
				});
				return sequenceDirectiveLoadDeferred.promise;
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

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadWorkflowActivityExtensionConfigs, loadSequenceDirective]).then(function () {
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function updateActivity() {
			$scope.scopeModel.isLoading = true;
			var updatedObject = sequenceDirectiveAPI.getData();
			if ($scope.onActivityUpdated != undefined) {
				$scope.onActivityUpdated(updatedObject);
			}
			$scope.scopeModel.isLoading = false;
			$scope.modalContext.closeModal();
		}
	}

	appControllers.controller('BusinessProcess_VR_WorkflowActivitySequenceController', SequenceEditorController);
})(appControllers);