'use strict';

app.directive('businessprocessVrWorkflowactivitySequence', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				isrequired: '=',
				normalColNum: '@',
				dragdropsetting: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var ctor = new workflowSequence(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowSequenceTemplate.html'
		};

		function workflowSequence(ctrl, $scope, $attrs) {
			var context;
			var variables;
			var activities;
			var workflowContainerAPI;
			var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();
			var activitySettings;
			var vRWorkflowActivityId;
			var disableActivity;

			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.isVRWorkflowActivityDisabled = false;
				$scope.scopeModel.datasource = [];
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

				$scope.scopeModel.onWorkflowContainerReady = function (api) {
					workflowContainerAPI = api;
					workflowContainerReadyPromiseDeferred.resolve();
				};

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
						childContext.doesActivityhaveErrors = context.doesActivityhaveErrors;
						childContext.showErrorWarningIcon = context.showErrorWarningIcon;
						childContext.isVariableNameReserved = context.isVariableNameReserved;
						childContext.getParentVariables = function () {
							var parentVars = [];
							if (context.getParentVariables != undefined)
								parentVars = parentVars.concat(context.getParentVariables());
							if (variables != undefined)
								parentVars = parentVars.concat(variables);
							return parentVars;
						};
					}
					return childContext;
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];
					if (payload != undefined) {
						vRWorkflowActivityId = payload.VRWorkflowActivityId;
						disableActivity = payload.DisableActivity;
						context = payload.Context;
						if (payload.Settings != undefined) {
							activitySettings = payload.Settings;
							$scope.scopeModel.isVRWorkflowActivityDisabled = false;
							variables = payload.Settings.Variables;
						}

						if (context != undefined && context.reserveVariableNames != undefined && variables != undefined && variables.length > 0)
							context.reserveVariableNames(variables);

						if (payload.Settings != undefined && payload.Settings.Activities != undefined && payload.Settings.Activities.length > 0) {
							activities = payload.Settings.Activities;
						}
						promises.push(loadWorkflowContainer());

						if (payload.SetMenuAction != undefined) {
							payload.SetMenuAction(getVariableEditorAction());
							if (!payload.DisableEdit)
								payload.SetMenuAction(getOpenEditorAction());
						}
					}
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {

					return {
						$type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSequenceActivity, Vanrise.BusinessProcess.MainExtensions",
						Activities: (workflowContainerAPI != undefined) ? workflowContainerAPI.getData() : null,
						Variables: variables
					};
				};

				api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
					$scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
					if (workflowContainerAPI != undefined && workflowContainerAPI.changeActivityStatus != undefined)
						workflowContainerAPI.changeActivityStatus(isVRWorkflowActivityDisabled);
				};

				api.getActivityStatus = function () {
					return $scope.scopeModel.isVRWorkflowActivityDisabled;
				};

				api.isActivityValid = function () {
					if (workflowContainerAPI != null && workflowContainerAPI.isActivityValid != undefined)
						return workflowContainerAPI.isActivityValid();

					return true;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function getVariableEditorAction() {
				return {
					name: "Variables",
					clicked: openVariablesEditor
				};
			}
			function getOpenEditorAction() {
				return {
					name: "Edit",
					clicked: openActivityEditor
				};
			}

			function openActivityEditor() {
				var onActivityUpdated = function (sequenceSettings) {
					var promises = [];
					if (sequenceSettings == undefined || sequenceSettings.Activities == undefined || sequenceSettings.Activities.length < 1) {
						activities = [];
					}
					else {
						activities = sequenceSettings.Activities;
					}

					if (sequenceSettings == undefined || sequenceSettings.Variables == undefined || sequenceSettings.Variables.length < 1) {
						variables = [];
					}
					else {
						variables = sequenceSettings.Variables;
					}

					if (sequenceSettings != undefined && sequenceSettings.IsDisabled && !activitySettings.IsDisabled && disableActivity != undefined)
						disableActivity();

					var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();
					var payload = {
						vRWorkflowActivities: activities,
						getChildContext: ctrl.getChildContext
					};
					VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, payload, workflowContainerLoadDeferred);

					promises.push(workflowContainerLoadDeferred);
				};

				var activities = (workflowContainerAPI != undefined) ? workflowContainerAPI.getData() : null;

				var settings = {
					Activities: activities,
					Variables: variables
				};
				if (activitySettings != undefined) {
					settings.Title = activitySettings.Title;
					settings.IsDisabled = activitySettings.IsDisabled;
					settings.Editor = activitySettings.Editor;
					settings.ConfigId = activitySettings.ConfigId;
				}

				var updateContext = ctrl.getChildContext();
				updateContext.inEditor = true;
				BusinessProcess_VRWorkflowService.openSequenceEditor(ctrl.dragdropsetting, updateContext, settings, onActivityUpdated, vRWorkflowActivityId);
			}

			function openVariablesEditor() {
				var onSaveVariables = function (activityVariables) {
					variables = activityVariables;
				};

				if (context != undefined)
					BusinessProcess_VRWorkflowService.openVariablesEditor(onSaveVariables, variables, (context.getParentVariables != undefined) ? context.getParentVariables() : undefined, context.reserveVariableName, context.eraseVariableName, context.isVariableNameReserved, context.reserveVariableNames);
				else
					BusinessProcess_VRWorkflowService.openVariablesEditor(onSaveVariables, variables, undefined, undefined, undefined, undefined);
			}

			function loadWorkflowContainer() {
				var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();

				workflowContainerReadyPromiseDeferred.promise.then(function () {
					var payload = {
						vRWorkflowActivities: activities,
						getChildContext: ctrl.getChildContext
					};
					VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, payload, workflowContainerLoadDeferred);
				});
				return workflowContainerLoadDeferred.promise;
			}
		}

		return directiveDefinitionObject;
	}]);