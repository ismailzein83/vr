'use strict';

app.directive('businessprocessVrWorkflowactivityForeach', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'BusinessProcess_VRWorkflowService',
	function (UtilsService, VRUIUtilsService, VRNotificationService, BusinessProcess_VRWorkflowService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				dragdropsetting: '=',
				remove: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new workflowForeach(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowForEachTemplate.html'
		};

		function workflowForeach(ctrl, $scope, $attrs) {
			var workflowContainerAPI;
			var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var iterationVariableType;

			var context = {};
			var isNew;
			var activity;

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

				$scope.scopeModel.onWorkflowContainerReady = function (api) {
					workflowContainerAPI = api;
					workflowContainerReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.validateForEach = function () {
					var erorrMessage = "";
					if ($scope.scopeModel.IterationVariableName == undefined || $scope.scopeModel.IterationVariableName == "")
						erorrMessage += "Iteration variable name should be entered. ";

					if ($scope.scopeModel.List == undefined || $scope.scopeModel.List == "")
						erorrMessage += "List name should be entered. ";

					if (iterationVariableType == undefined)
						erorrMessage += "Variable type should be selected. ";

					if (erorrMessage.length > 0)
						return erorrMessage;

					return null;
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				ctrl.getChildContext = function () {
					var childContext = {};

					if (context != undefined) {
						childContext.inEditor = context.inEditor;
						childContext.vrWorkflowId = context.vrWorkflowId;
						childContext.getWorkflowArguments = context.getWorkflowArguments;
						childContext.addToList = context.addToList;
						childContext.removeFromList = context.removeFromList;
						childContext.reserveVariableName = context.reserveVariableName;
						childContext.reserveVariableNames = context.reserveVariableNames;
						childContext.eraseVariableName = context.eraseVariableName;
						childContext.isVariableNameReserved = context.isVariableNameReserved;
						childContext.doesActivityhaveErrors = context.doesActivityhaveErrors;
						childContext.showErrorWarningIcon = context.showErrorWarningIcon;
						childContext.getParentVariables = function () {
							var parentVars = [];
							if (context.getParentVariables != undefined)
								parentVars = parentVars.concat(context.getParentVariables());
							return parentVars;
						};
					}
					return childContext;
				};

				api.load = function (payload) {

					var editModeAction = {
						name: "Edit",
						clicked: openActivityEditor
					};

					var promises = [];
					if (payload != undefined) {
						if (payload.Settings != undefined) {
							isNew = payload.Settings.IsNew;
							$scope.scopeModel.List = payload.Settings.List;
							$scope.scopeModel.IterationVariableName = payload.Settings.IterationVariableName;
							iterationVariableType = payload.Settings.IterationVariableType;
							activity = payload.Settings.Activity;
						}
						if (payload.Context != null) {
							$scope.scopeModel.context = payload.Context;
							context = payload.Context;
						}
						if (payload.SetMenuAction != undefined)
							payload.SetMenuAction(editModeAction);
					}

					promises.push(loadWorkflowContainer());
					return UtilsService.waitMultiplePromises(promises).then(function () {
						if (isNew)
							openActivityEditor();
					});

					function openActivityEditor() {
						var onActivityUpdated = function (foreachObj) {
							var promises = [];
							isNew = false;
							$scope.scopeModel.List = foreachObj.List;
							$scope.scopeModel.IterationVariableName = foreachObj.IterationVariableName;
							iterationVariableType = foreachObj.IterationVariableType;
							activity = foreachObj.Activity;
							promises.push(loadWorkflowContainer());
							UtilsService.waitMultiplePromises(promises);
						};
						var updateContext = ctrl.getChildContext();
						updateContext.inEditor = true;
						BusinessProcess_VRWorkflowService.openForeachEditor(ctrl.dragdropsetting, buildObjectFromScope(), ctrl.getChildContext, updateContext, onActivityUpdated, ctrl.remove, isNew);
					}

					function loadWorkflowContainer() {
						var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();

						if (activity == undefined) {
							activity = {
								Settings: {
									Editor: "businessprocess-vr-workflowactivity-sequence",
									Title: "Sequence"
								},
								VRWorkflowActivityId: UtilsService.guid()
							};
						}
						workflowContainerReadyPromiseDeferred.promise.then(function () {
							var payload = {
								vRWorkflowActivity: activity,
								getChildContext: ctrl.getChildContext
							};
							VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, payload, workflowContainerLoadDeferred);
						});
						return workflowContainerLoadDeferred.promise;
					}
				};

				api.getData = function () {
					return buildObjectFromScope();
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function buildObjectFromScope() {
				return {
					$type:
						"Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowForEachActivity, Vanrise.BusinessProcess.MainExtensions",
					List: $scope.scopeModel.List,
					IterationVariableName: $scope.scopeModel.IterationVariableName,
					IterationVariableType: iterationVariableType,
					Activity: (workflowContainerAPI != undefined) ? workflowContainerAPI.getData() : null
				};
			}
		}
		return directiveDefinitionObject;
	}]);