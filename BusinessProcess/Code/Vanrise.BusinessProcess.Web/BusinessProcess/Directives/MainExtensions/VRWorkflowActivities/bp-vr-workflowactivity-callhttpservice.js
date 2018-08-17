'use strict';

app.directive('businessprocessVrWorkflowactivityCallhttpservice', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'BusinessProcess_VRWorkflowService',
	function (UtilsService, VRUIUtilsService, VRNotificationService, BusinessProcess_VRWorkflowService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				remove: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new callHttpService(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowCallHttpServiceTemplate.html'
		};

		function callHttpService(ctrl, $scope, $attrs) {

			var serviceName;
			var connectionId;
			var callHttpServiceMethod;
			var actionPath;
			var buildBodyLogic;
			var callHttpServiceMessageFormat;
			var callHttpRetrySettings;
			var responseLogic;
			var errorLogic;
			var isSucceeded;
			var continueWorkflowIfCallFailed;
			var headers;
			var urlParameters;
			var isNew;
			var context;

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var editModeAction = {
						name: "Edit",
						clicked: openActivityEditor
					};

					if (payload != undefined) {
						if (payload.Settings != undefined) {
							isNew = payload.Settings.IsNew;
							serviceName = payload.Settings.ServiceName;
							$scope.scopeModel.serviceName = serviceName;
							connectionId = payload.Settings.ConnectionId;
							callHttpServiceMethod = payload.Settings.Method;
							actionPath = payload.Settings.ActionPath;
							buildBodyLogic = payload.Settings.BuildBodyLogic;
							callHttpServiceMessageFormat = payload.Settings.MessageFormat;
							callHttpRetrySettings = payload.Settings.RetrySettings;
							responseLogic = payload.Settings.ResponseLogic;
							errorLogic = payload.Settings.ErrorLogic;
							isSucceeded = payload.Settings.IsSucceeded;
							continueWorkflowIfCallFailed = payload.Settings.ContinueWorkflowIfCallFailed;
							headers = payload.Settings.Headers;
							urlParameters = payload.Settings.URLParameters;
						}

						if (payload.Context != null)
							context = payload.Context;

						if (payload.SetMenuAction != undefined)
							payload.SetMenuAction(editModeAction);

						if (isNew) {
							openActivityEditor();
						}
					}

					function openActivityEditor() {
						var onActivityUpdated = function (updatedObject) {
							$scope.scopeModel.serviceName = updatedObject.serviceName;
							serviceName = updatedObject.serviceName;
							connectionId = updatedObject.connectionId;
							callHttpServiceMethod = updatedObject.callHttpServiceMethod;
							actionPath = updatedObject.actionPath;
							buildBodyLogic = updatedObject.buildBodyLogic;
							callHttpServiceMessageFormat = updatedObject.callHttpServiceMessageFormat;
							callHttpRetrySettings = updatedObject.callHttpRetrySettings;
							responseLogic = updatedObject.responseLogic;
							errorLogic = updatedObject.errorLogic;
							isSucceeded = updatedObject.isSucceeded;
							continueWorkflowIfCallFailed = updatedObject.continueWorkflowIfCallFailed;
							headers = updatedObject.headers;
							urlParameters = updatedObject.urlParameters;
							isNew = false;
						};

						BusinessProcess_VRWorkflowService.openCallHttpServiceEditor(buildObjectFromScope(), context, onActivityUpdated, ctrl.remove, isNew);
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
						"Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceActivity, Vanrise.BusinessProcess.MainExtensions",
					ServiceName: serviceName,
					ConnectionId: connectionId,
					Method: callHttpServiceMethod,
					ActionPath: actionPath,
					BuildBodyLogic: buildBodyLogic,
					MessageFormat: callHttpServiceMessageFormat,
					RetrySettings: callHttpRetrySettings,
					ResponseLogic: responseLogic,
					ErrorLogic: errorLogic,
					IsSucceeded: isSucceeded,
					ContinueWorkflowIfCallFailed: continueWorkflowIfCallFailed,
					Headers: headers,
					URLParameters: urlParameters
				};
			}
		}
		return directiveDefinitionObject;
	}]);