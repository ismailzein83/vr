"use strict";

app.directive("businessprocessVrWorkflowActivityDirective", ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService) {
		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				dragdropsetting: '=',
				onRemove: '=',
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var directiveConstructor = new DirectiveConstructor($scope, ctrl);
				directiveConstructor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowActivityDirectiveTemplate.html'
		};

		function DirectiveConstructor($scope, ctrl) {
			var vRWorkflowActivityId;
			var vRWorkflowActivitySettings;
			var context;
			var directiveWraperAPI;
			var directiveWraperReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;
				$scope.scopeModel.onDirectiveWraperReady = function (api) {
					directiveWraperAPI = api;
					directiveWraperReadyPromiseDeferred.resolve();
				};
				$scope.scopeModel.actions = [];
				defineAPI();
			}

			function defineAPI() {
				//function openVariablesEditor() {
				//var onSaveVariables = function (activityVariables) {

				//vRWorkflowActivitySettings.Variables = activityVariables;
				//};
				//BusinessProcess_VRWorkflowService.openVariablesEditor(onSaveVariables, vRWorkflowActivitySettings.Variables, context.parentVariables);
				//}

				var api = {};
				api.load = function (payload) {
					var setMenuAction = function (menuAction) {
						$scope.scopeModel.actions.push(menuAction);
					};
					//console.log("Activity load ");
					var loadPromiseDeferred = UtilsService.createPromiseDeferred();
					if (payload != undefined && payload.Settings != null) {
						//console.log("Activity directive load directive with id ", payload.VRWorkflowActivityId);
						$scope.scopeModel.onRemove = function () { ctrl.onRemove(payload.VRWorkflowActivityId); };
						$scope.scopeModel.editor = payload.Settings.Editor;
						$scope.scopeModel.header = payload.Settings.Title;
						//if (payload.Settings.Title == "Sequence") {
						//$scope.scopeModel.actions = [{
						//	name: "Variables",
						//	clicked: openVariablesEditor
						//}];
						//}
						vRWorkflowActivityId = payload.VRWorkflowActivityId;
						vRWorkflowActivitySettings = payload.Settings;
						context = payload.Context;
						directiveWraperReadyPromiseDeferred.promise.then(function () {
							//console.log("Activity directive load after promise directive with id ", vRWorkflowActivityId);
							var payload = {
								Context: context,
								SetMenuAction: setMenuAction,
								Settings: vRWorkflowActivitySettings
							};
							directiveWraperAPI.load(payload);
							loadPromiseDeferred.resolve();
						});
					}
					else {
						loadPromiseDeferred.resolve();
					}
					return loadPromiseDeferred.promise;
				};

				api.getData = function () {
					//console.log("Activity directive try to get directive data with id ", vRWorkflowActivityId);
					var settings = {};
					if (directiveWraperAPI != null)
						settings = directiveWraperAPI.getData();
					//if (vRWorkflowActivitySettings.Variables != undefined)
					//settings.Variables = vRWorkflowActivitySettings.Variables
					settings.Title = $scope.scopeModel.header;
					settings.Editor = $scope.scopeModel.editor;
					return {
						VRWorkflowActivityId: vRWorkflowActivityId,
						Settings: settings
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}


		}

		return directiveDefinitionObject;
	}]);
