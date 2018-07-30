//'use strict';

//app.directive('businessprocessVrWorkflowactivityForeach', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'
//	function (UtilsService, VRUIUtilsService, VRNotificationService) {

//		var directiveDefinitionObject = {
//			restrict: 'E',
//			scope: {
//				onReady: '=',
//				dragdropsetting: '='
//			},
//			controller: function ($scope, $element, $attrs) {
//				var ctrl = this;
//				var ctor = new workflowForeach(ctrl, $scope, $attrs);
//				ctor.initializeController();
//			},
//			controllerAs: 'ctrl',
//			bindToController: true,
//			compile: function (element, attrs) {

//			},
//			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowForEachTemplate.html'
//		};

//		function workflowForeach(ctrl, $scope, $attrs) {
//			var workflowContainerApi;
//			var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//			var context = {};
//			var activity;

//			this.initializeController = initializeController;
//			function initializeController() {
//				$scope.scopeModel = {};
//				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;
//				$scope.scopeModel.onWorkflowContainerReady = function (api) {
//					workflowContainerApi = api;
//					workflowContainerReadyPromiseDeferred.resolve();
//				};
//				defineAPI();
//			}

//			function defineAPI() {
//				var api = {};

//				api.load = function (payload) {
//					if (payload != undefined) {
//						if (payload.Settings != undefined) {
//							$scope.scopeModel.List = payload.Settings.List;
//							$scope.scopeModel.IterationVariableName = payload.Settings.IterationVariableName;
//							$scope.scopeModel.IterationVariableType = payload.Settings.IterationVariableType;
//							activity = payload.Settings.Activity;
//						}
//						if (payload.Context != null)
//							context = payload.Context;
//					}
//					return loadAllControls();

//					function loadAllControls() {
//						return UtilsService.waitMultipleAsyncOperations([loadWorkflowContainer])
//							.catch(function (error) {
//								VRNotificationService.notifyExceptionWithClose(error, $scope);
//							});
//					}

//					function loadWorkflowContainer() {
//						var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();
//						if (activity == undefined) {
//							activity = {
//								ConfigId: "9292B3BE-256F-400F-9BC6-A0423FA0B30F",
//								Editor: "businessprocess-vr-workflow-sequence",
//								Title: "Sequence"
//							}
//						}
//						workflowContainerReadyPromiseDeferred.promise.then(function () {
//							var payload = {
//								getWorkflowArguments: context.getWorkflowArguments,
//								vRWorkflowActivity: activity,
//								reserveVariableName: context.reserveVariableName,
//								reserveVariableNames: context.reserveVariableNames,
//								isVariableNameReserved: context.isVariableNameReserved,
//								eraseVariableName: context.eraseVariableName
//							};
//							VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, payload, workflowContainerLoadDeferred);
//						});
//						return workflowContainerLoadDeferred.promise;
//					}
//				};

//				api.getData = function () {
//					return {
//						$type:
//							"Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowForEachActivity, Vanrise.BusinessProcess.MainExtensions",
//						List: $scope.scopeModel.List,
//						IterationVariableName: $scope.scopeModel.IterationVariableName,
//						IterationVariableType: $scope.scopeModel.IterationVariableType,
//						Activity: (workflowContainerAPI != undefined) ? workflowContainerAPI.getData() : null
//					};
//				};

//				if (ctrl.onReady != null)
//					ctrl.onReady(api);
//			}
//		}
//		return directiveDefinitionObject;
//	}]);