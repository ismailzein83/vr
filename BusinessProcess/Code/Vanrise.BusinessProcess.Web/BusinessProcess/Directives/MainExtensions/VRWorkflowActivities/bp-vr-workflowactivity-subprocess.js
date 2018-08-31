'use strict';

app.directive('businessprocessVrWorkflowactivitySubprocess', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				remove: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new workflowCtor(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowSubProcessTemplate.html'
		};

		function workflowCtor(ctrl, $scope, $attrs) {

			var selectedVRWorkflowId;
			var VRWorkflowId;
			var inArguments;
			var outArguments;
			var isNew;
			var context;

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.VRWorkflowName = "";
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var editModeAction = {
						name: "Edit",
						clicked: openActivityEditor
					};

					var promises = [];

					if (payload != undefined) {
						$scope.scopeModel.context = payload.Context;
						if (payload.Context != undefined)
							VRWorkflowId = payload.Context.vrWorkflowId;
						context = payload.Context;
						if (payload.Settings != undefined) {
							selectedVRWorkflowId = payload.Settings.VRWorkflowId;
							isNew = payload.Settings.IsNew;
							inArguments = payload.Settings.InArguments;
							outArguments = payload.Settings.OutArguments;
						}

						if (payload.SetMenuAction != undefined)
							payload.SetMenuAction(editModeAction);
					}

					if (selectedVRWorkflowId != undefined)
						//promises.push();
						BusinessProcess_VRWorkflowAPIService.GetVRWorkflowName(selectedVRWorkflowId).
							then(function (response) {
								$scope.scopeModel.VRWorkflowName = response;
							});

					function openActivityEditor() {
						var onActivityUpdated = function (updatedObject) {
							$scope.scopeModel.VRWorkflowName = updatedObject.VRWorkflowName;
							selectedVRWorkflowId = updatedObject.VRWorkflowId;
							inArguments = updatedObject.InArguments;
							outArguments = updatedObject.OutArguments;
							isNew = false;
						};

						BusinessProcess_VRWorkflowService.openSubProcessEditor(buildObjectFromScope(), onActivityUpdated, ctrl.remove, isNew, VRWorkflowId, context);
					}

					return UtilsService.waitMultiplePromises(promises).then(function () {
						if (isNew) {
							openActivityEditor();
						}
					});
				};

				api.getData = function () {
					return buildObjectFromScope();
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function buildObjectFromScope() {
				return {
					$type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSubProcessActivity, Vanrise.BusinessProcess.MainExtensions",
					VRWorkflowId: selectedVRWorkflowId,
					InArguments: inArguments,
					OutArguments: outArguments
				};
			}
		}
		return directiveDefinitionObject;
	}]);