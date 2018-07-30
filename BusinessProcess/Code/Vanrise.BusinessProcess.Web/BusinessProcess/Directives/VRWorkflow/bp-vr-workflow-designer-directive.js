'use strict';

app.directive('businessprocessVrWorkflowDesignerDirective', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_VRWorkflowAPIService', 'VRDragdropService', 'VRNotificationService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, BusinessProcess_VRWorkflowAPIService, VRDragdropService, VRNotificationService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new workflowDesigner(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowDesignerDirectiveTemplate.html'
		};

		function workflowDesigner(ctrl, $scope, $attrs) {

			var workflowContainerAPI;
			var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();
			var rootActivity;
			//var workflowArguments;
			var getWorkflowArguments;
			var reserveVariableName;
			var reserveVariableNames;
			var isVariableNameReserved;
			var eraseVariableName;

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.datasource = [];
				$scope.scopeModel.activityConfigs = [];

				$scope.scopeModel.dragdropGroupCorrelation = VRDragdropService.createCorrelationGroup();
				$scope.scopeModel.dragdropsetting = {
					groupCorrelation: $scope.scopeModel.dragdropGroupCorrelation,
					canReceive: true,
					canSend: true,
					copyOnSend: true,
					onItemReceived: function (itemAdded, dataSource, sourceList, itemAddedContext) {
						var vRWorkflowActivity = {};
						if (itemAdded.directiveAPI != null) {
							vRWorkflowActivity.Settings = itemAdded.directiveAPI.getData().Settings;
						}
						else {
							vRWorkflowActivity.Settings = {
								Editor: (itemAdded.Editor) ? itemAdded.Editor : itemAdded.Settings.Editor,
								Title: (itemAdded.Title) ? itemAdded.Title : itemAdded.Settings.Title
							};
						}
						vRWorkflowActivity.VRWorkflowActivityId = UtilsService.guid();

						vRWorkflowActivity.onDirectiveReady = function (api) {
							if (vRWorkflowActivity.directiveAPI != null)
								return;
							vRWorkflowActivity.directiveAPI = api;
							var setLoader = function (value) { $scope.x = value; };
							var context = (itemAddedContext != undefined) ? itemAddedContext : {
								getWorkflowArguments: getWorkflowArguments,
								reserveVariableName : reserveVariableName,
								reserveVariableNames : reserveVariableNames,
								isVariableNameReserved : isVariableNameReserved,
								eraseVariableName : eraseVariableName
							};
							var payload = {
								Context: context,
								VRWorkflowActivityId: vRWorkflowActivity.VRWorkflowActivityId,
								Settings: vRWorkflowActivity.Settings
							};
							VRUIUtilsService.callDirectiveLoad(vRWorkflowActivity.directiveAPI, payload);
						};

						return vRWorkflowActivity;
					},

					enableSorting: true
				};

				$scope.scopeModel.onWorkflowContainerReady = function (api) {
					workflowContainerAPI = api;
					workflowContainerReadyPromiseDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {

					if (payload != undefined) {
						rootActivity = payload.rootActivity;
						getWorkflowArguments = payload.getWorkflowArguments;
						reserveVariableName = payload.reserveVariableName;
						reserveVariableNames = payload.reserveVariableNames;
						isVariableNameReserved = payload.isVariableNameReserved;
						eraseVariableName = payload.eraseVariableName;
					}
					return loadAllControls();

					function loadAllControls() {
						return UtilsService.waitMultipleAsyncOperations([loadWorkflowContainer, loadWorkflowActivityExtensionConfigs])
							.catch(function (error) {
								VRNotificationService.notifyExceptionWithClose(error, $scope);
							});
					}

					function loadWorkflowContainer() {
						var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();
						if (rootActivity != undefined) {
							workflowContainerReadyPromiseDeferred.promise.then(function () {
								var payload = {
									getWorkflowArguments: getWorkflowArguments,
									vRWorkflowActivity: rootActivity,
									reserveVariableName: reserveVariableName,
									reserveVariableNames: reserveVariableNames,
									isVariableNameReserved: isVariableNameReserved,
									eraseVariableName: eraseVariableName
								};
								VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, payload, workflowContainerLoadDeferred);
							});
						}
						else {
							workflowContainerLoadDeferred.resolve();
						}
						return workflowContainerLoadDeferred.promise;
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
				};

				api.getData = function () {
					if (workflowContainerAPI != null)
						return workflowContainerAPI.getData();
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);