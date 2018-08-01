//'use strict';

//app.directive('businessprocessVrWorkflowactivityCallhttpservice', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'BusinessProcess_VRWorkflowService',
//	function (UtilsService, VRUIUtilsService, VRNotificationService, BusinessProcess_VRWorkflowService) {

//		var directiveDefinitionObject = {
//			restrict: 'E',
//			scope: {
//				onReady: '=',
//			},
//			controller: function ($scope, $element, $attrs) {
//				var ctrl = this;
//				var ctor = new callHttpService(ctrl, $scope, $attrs);
//				ctor.initializeController();
//			},
//			controllerAs: 'ctrl',
//			bindToController: true,
//			compile: function (element, attrs) {

//			},
//			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowCallHttpServiceTemplate.html'
//		};

//		function callHttpService(ctrl, $scope, $attrs) {

//			var serviceName;
//			var connectionId;
//			var callHttpServiceMethod;
//			var actionPath;
//			var buildBodyLogic;
//			var callHttpServiceMessageFormat;
//			var responseLogic;
//			var errorLogic;
//			var isSucceeded;
//			var continueWorkflowIfCallFailed;
//			var headers;
//			var urlParameters;

//			this.initializeController = initializeController;
//			function initializeController() {
//				$scope.scopeModel = {};
//				defineAPI();
//			}

//			function defineAPI() {
//				var api = {};

//				api.load = function (payload) {
//					var editModeAction = {
//						name: "Edit",
//						clicked: openActivityEditor
//					};

//					if (payload != undefined) {
//						if (payload.Settings != undefined) {
//							serviceName = payload.Settings.serviceName;
//							connectionId = payload.Settings.connectionId;
//							callHttpServiceMethod = payload.Settings.callHttpServiceMethod;
//							actionPath = payload.Settings.actionPath;
//							buildBodyLogic = payload.Settings.buildBodyLogic;
//							callHttpServiceMessageFormat = payload.Settings.callHttpServiceMessageFormat;
//							responseLogic = payload.Settings.responseLogic;
//							errorLogic = payload.Settings.errorLogic;
//							isSucceeded = payload.Settings.isSucceeded;
//							continueWorkflowIfCallFailed = payload.Settings.continueWorkflowIfCallFailed;
//							headers = payload.Settings.Headers;
//							urlParameters = payload.Settings.URLParameters;
//						}

//						if (payload.SetMenuAction != undefined)
//							payload.SetMenuAction(editModeAction);
//					}

//					function openActivityEditor() {
//						var onActivityUpdated = function (updatedObject) {
//							serviceName = updatedObject.serviceName;
//							connectionId = updatedObject.connectionId;
//							callHttpServiceMethod = updatedObject.callHttpServiceMethod;
//							actionPath = updatedObject.actionPath;
//							buildBodyLogic = updatedObject.buildBodyLogic;
//							callHttpServiceMessageFormat = updatedObject.callHttpServiceMessageFormat;
//							responseLogic = updatedObject.responseLogic;
//							errorLogic = updatedObject.errorLogic;
//							isSucceeded = updatedObject.isSucceeded;
//							continueWorkflowIfCallFailed = updatedObject.continueWorkflowIfCallFailed;
//							headers = updatedObject.headers;
//							urlParameters = updatedObject.urlParameters;
//						};

//						BusinessProcess_VRWorkflowService.openCallHttpServiceEditor(buildObjectFromScope(), onActivityUpdated);
//					}
//				};

//				api.getData = function () {
//					return buildObjectFromScope();
//				};

//				if (ctrl.onReady != null)
//					ctrl.onReady(api);
//			}

//			function buildObjectFromScope() {
//				return {
//					$type:
//						"Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowCallHttpServiceActivity, Vanrise.BusinessProcess.MainExtensions",
//					ServiceName: serviceName,
//					ConnectionId: connectionId,
//					CallHttpServiceMethod: callHttpServiceMethod,
//					ActionPath: actionPath,
//					BuildBodyLogic: buildBodyLogic,
//					CallHttpServiceMessageFormat: callHttpServiceMessageFormat,
//					ResponseLogic: responseLogic,
//					ErrorLogic: errorLogic,
//					IsSucceeded: isSucceeded,
//					ContinueWorkflowIfCallFailed: continueWorkflowIfCallFailed,
//					Headers: headers,
//					URLParameters: urlParameters
//				};
//			}
//		}
//		return directiveDefinitionObject;
//	}]);