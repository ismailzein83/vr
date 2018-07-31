'use strict';

app.directive('businessprocessVrWorkflowactivityForeach', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService',
	function (UtilsService, VRUIUtilsService, VRNotificationService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				dragdropsetting: '='
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

			var variableTypeSelectorAPI;
			var variableTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var iterationVariableType;

			var context = {};
			var activity;

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;

				$scope.scopeModel.onWorkflowContainerReady = function (api) {
					workflowContainerAPI = api;
					workflowContainerReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onVariableTypeSelectorReady = function (api) {
					variableTypeSelectorAPI = api;
					variableTypeSelectorReadyDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				ctrl.getChildContext = function () {
					var childContext = {};

					if (context != undefined) {
						childContext.getWorkflowArguments = context.getWorkflowArguments;
						childContext.reserveVariableName = context.reserveVariableName;
						childContext.reserveVariableNames = context.reserveVariableNames;
						childContext.eraseVariableName = context.eraseVariableName;
						childContext.isVariableNameReserved = context.isVariableNameReserved;
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
					var promises = [];
					if (payload != undefined) {
						if (payload.Settings != undefined) {
							$scope.scopeModel.List = payload.Settings.List;
							$scope.scopeModel.IterationVariableName = payload.Settings.IterationVariableName;
							$scope.scopeModel.IterationVariableType = payload.Settings.IterationVariableType;
							iterationVariableType = payload.Settings.IterationVariableType;
							activity = payload.Settings.Activity;
						}
						if (payload.Context != null) {
							$scope.scopeModel.context = payload.Context;
							context = payload.Context;
						}
					}

					promises.push(loadWorkflowContainer());
					promises.push(loadVariableTypeDirective());
					UtilsService.waitMultiplePromises(promises);

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

					function loadVariableTypeDirective() {
						var variableTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

						variableTypeSelectorReadyDeferred.promise.then(function () {

							var variableTypeDirectivePayload = { selectIfSingleItem: true };
							if (iterationVariableType != undefined && iterationVariableType != undefined) {
								variableTypeDirectivePayload.variableType = iterationVariableType;
							}
							VRUIUtilsService.callDirectiveLoad(variableTypeSelectorAPI, variableTypeDirectivePayload, variableTypeDirectiveLoadDeferred);
						});

						return variableTypeDirectiveLoadDeferred.promise;
					}
				};

				api.getData = function () {
					return {
						$type:
							"Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowForEachActivity, Vanrise.BusinessProcess.MainExtensions",
						List: $scope.scopeModel.List,
						IterationVariableName: $scope.scopeModel.IterationVariableName,
						IterationVariableType: variableTypeSelectorAPI.getData(),
						Activity: (workflowContainerAPI != undefined) ? workflowContainerAPI.getData() : null
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);