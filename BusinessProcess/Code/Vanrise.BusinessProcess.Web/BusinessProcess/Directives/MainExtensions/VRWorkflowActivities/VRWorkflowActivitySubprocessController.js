(function (appControllers) {

	"use strict";

	SubprocessEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService'];

	function SubprocessEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService) {


		var vrWorkflowSelectorAPI;
		var vrWorkflowSelectorReadyDeferred = UtilsService.createPromiseDeferred();
		var vrWorkflowSelectorSelectionChangedDeferred;

		var inArgumentsGridAPI;
		var inArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

		var outArgumentsGridAPI;
		var outArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

		var selectedVRWrkflow;

		var loadSelectedVRWorkflowPromise;

		var selectedVRWorkflowId;
		var inArguments;
		var outArguments;
		var workflowId;
		var isNew;
		var context;
		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined) {
				isNew = parameters.isNew;
				if (parameters.obj != undefined) {
					selectedVRWorkflowId = parameters.obj.VRWorkflowId;
					inArguments = parameters.obj.InArguments;
					outArguments = parameters.obj.OutArguments;
					workflowId = parameters.workflowId;
					context = parameters.context;
				}
			}
		}

		function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.context = context;
			$scope.scopeModel.hasInArguments = false;
			$scope.scopeModel.hasOutArguments = false;
			$scope.scopeModel.inArguments = [];
			$scope.scopeModel.outArguments = [];

			$scope.scopeModel.onVRWorkflowSelectorReady = function (api) {
				vrWorkflowSelectorAPI = api;
				vrWorkflowSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onInArgumentsGridReady = function (api) {
				inArgumentsGridAPI = api;
				inArgumentsGridReadyDeferred.resolve();
			};

			$scope.scopeModel.onOutArgumentsGridReady = function (api) {
				outArgumentsGridAPI = api;
				outArgumentsGridReadyDeferred.resolve();
			};

			$scope.scopeModel.onVRWorkflowSelectionChanged = function (selectedItem) {
				if (selectedItem == undefined)
					return;
				else {
					if (vrWorkflowSelectorSelectionChangedDeferred != undefined) {
						vrWorkflowSelectorSelectionChangedDeferred.resolve();
					}
					else {
						$scope.scopeModel.isLoading = true;
						var rootPromiseNode = {
							promises: [loadVRWorkflow(selectedItem.VRWorkflowId)],
							getChildNode: function () {
								var gridPromises = [];
								if ($scope.scopeModel.hasInArguments)
									gridPromises.push(loadInArgumentsGrid());

								if ($scope.scopeModel.hasOutArguments)
									gridPromises.push(loadOutArgumentsGrid());

								return {
									promises: gridPromises
								};
							}
						};
						UtilsService.waitPromiseNode(rootPromiseNode).then(function () { $scope.scopeModel.isLoading = false; });
					}
				}
			};

			$scope.scopeModel.close = function () {
				if ($scope.remove != undefined && isNew == true) {
					$scope.remove();
				}
				$scope.modalContext.closeModal();
			};

			$scope.modalContext.onModalHide = function () {
				if ($scope.remove != undefined && isNew == true) {
					$scope.remove();
				}
			};

			$scope.scopeModel.saveActivity = function () {
				return updateActivity();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {

			function setTitle() {
				$scope.title = "Edit Sub Process";
			}

			function loadVRWorkflowInfo() {
				var promises = [];
				if (selectedVRWorkflowId != undefined) {
					vrWorkflowSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

					var rootPromiseNode = {
						promises: [loadVRWorkflow(selectedVRWorkflowId)],
						getChildNode: function () {
							var gridPromises = [];
							if ($scope.scopeModel.hasInArguments)
								gridPromises.push(loadInArgumentsGrid(inArguments));

							if ($scope.scopeModel.hasOutArguments)
								gridPromises.push(loadOutArgumentsGrid(outArguments));

							return {
								promises: gridPromises
							};
						}
					};
					promises.push(UtilsService.waitPromiseNode(rootPromiseNode));
				}
				var loadVRWorkflowSelectorPromise = loadVRWorkflowSelector();
				promises.push(loadVRWorkflowSelectorPromise);
				return UtilsService.waitMultiplePromises(promises);
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle, loadVRWorkflowInfo]).then(function () {
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				vrWorkflowSelectorSelectionChangedDeferred = undefined;
				$scope.scopeModel.isLoading = false;
			});
		}

		function loadVRWorkflowSelector() {
			var vrWorkflowSelectorLoadDeferred = UtilsService.createPromiseDeferred();

			vrWorkflowSelectorReadyDeferred.promise.then(function () {
				var vrWorkflowSelectorPayload = {};

				if (selectedVRWorkflowId != undefined)
					vrWorkflowSelectorPayload.selectedIds = selectedVRWorkflowId;

				if (workflowId != undefined)
					vrWorkflowSelectorPayload.filter = { ExcludedIds: [workflowId] };

				VRUIUtilsService.callDirectiveLoad(vrWorkflowSelectorAPI, vrWorkflowSelectorPayload, vrWorkflowSelectorLoadDeferred);
			});

			return vrWorkflowSelectorLoadDeferred.promise;
		}

		function updateActivity() {
			$scope.scopeModel.isLoading = true;
			var updatedObject = buildObjFromScope();
			if ($scope.onActivityUpdated != undefined) {
				$scope.onActivityUpdated(updatedObject);
			}
			$scope.scopeModel.isLoading = false;
			isNew = false;
			$scope.modalContext.closeModal();
		}

		function buildObjFromScope() {
			var inArgumentsObject;
			if ($scope.scopeModel.hasInArguments) {
				inArgumentsObject = {};
				for (var x = 0; x < $scope.scopeModel.inArguments.length; x++) {
					var currentInArgument = $scope.scopeModel.inArguments[x];
					if (currentInArgument.value != undefined) {
						inArgumentsObject[currentInArgument.name] = currentInArgument.value;
					}
				}
			}

			var outArgumentsObject;
			if ($scope.scopeModel.hasOutArguments) {
				outArgumentsObject = {};
				for (var x = 0; x < $scope.scopeModel.outArguments.length; x++) {
					var currentOutArgument = $scope.scopeModel.outArguments[x];
					if (currentOutArgument.value != undefined) {
						outArgumentsObject[currentOutArgument.name] = currentOutArgument.value;
					}
				}
			}

			return {
				VRWorkflowName: ($scope.scopeModel.selectedWorkflow != undefined) ? $scope.scopeModel.selectedWorkflow.Name : null,
				VRWorkflowId: vrWorkflowSelectorAPI.getSelectedIds(),
				InArguments: inArgumentsObject,
				OutArguments: outArgumentsObject
			};
		}

		function loadVRWorkflow(vrWorkflowId) {
			$scope.scopeModel.hasInArguments = false;
			$scope.scopeModel.hasOutArguments = false;

			return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowEditorRuntime(vrWorkflowId).then(function (response) {
				selectedVRWrkflow = response.Entity;
				if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
					for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
						var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
						switch (currentArgumentDefinition.Direction) {
							case 0: $scope.scopeModel.hasInArguments = true; break;
							case 1: $scope.scopeModel.hasOutArguments = true; break;
							case 2: $scope.scopeModel.hasInArguments = true; $scope.scopeModel.hasOutArguments = true; break;
						}

					}
				}
			});
		}

		function loadInArgumentsGrid(selectedArguments) {
			$scope.scopeModel.inArguments = [];
			var inArgumentsGridLoadDeferred = UtilsService.createPromiseDeferred();
			inArgumentsGridReadyDeferred.promise.then(function () {
				if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
					for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
						var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
						if (currentArgumentDefinition.Direction != 1) {
							var argValue = tryGetArgumentValue(selectedArguments, currentArgumentDefinition.Name);
							$scope.scopeModel.inArguments.push({ name: currentArgumentDefinition.Name, value: argValue });
						}
					}
				}
				inArgumentsGridLoadDeferred.resolve();
			});

			return inArgumentsGridLoadDeferred.promise;
		}

		function tryGetArgumentValue(selectedArguments, argumentName) {
			if (selectedArguments == undefined)
				return;

			return selectedArguments[argumentName];
		}

		function loadOutArgumentsGrid(selectedArguments) {
			$scope.scopeModel.outArguments = [];
			var outArgumentsGridLoadDeferred = UtilsService.createPromiseDeferred();
			outArgumentsGridReadyDeferred.promise.then(function () {
				if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
					for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
						var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
						if (currentArgumentDefinition.Direction != 0) {
							var argValue = tryGetArgumentValue(selectedArguments, currentArgumentDefinition.Name);
							$scope.scopeModel.outArguments.push({ name: currentArgumentDefinition.Name, value: argValue });
						}
					}
				}
				outArgumentsGridLoadDeferred.resolve();
			});

			return outArgumentsGridLoadDeferred.promise;
		}
	}

	appControllers.controller('BusinessProcess_VR_WorkflowActivitySubprocessController', SubprocessEditorController);
})(appControllers);