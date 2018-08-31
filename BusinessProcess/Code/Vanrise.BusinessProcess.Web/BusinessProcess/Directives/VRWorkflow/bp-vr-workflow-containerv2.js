"use strict";

app.directive('businessprocessVrWorkflowContainerv2', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {
		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "=",
				dragdropsetting: '=',
				singleActivity: '=',
				acceptEmptyContainer: '='
			},

			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				ctrl.itemsSortable = { handle: '.handeldrag', animation: 100 };
				ctrl.itemsSortable.sort = true;
				if (ctrl.dragdropsetting != undefined && typeof (ctrl.dragdropsetting) == 'object') {
					ctrl.itemsSortable.group = {
						name: ctrl.dragdropsetting.groupCorrelation.getGroupName(),
						pull: true,
						put: function (to) {
							if (ctrl.singleActivity != undefined && ctrl.singleActivity)
								return (ctrl.dragdropsetting.canReceive && to.el.children.length < 1);
							else return ctrl.dragdropsetting.canReceive;
						}
					};

					ctrl.itemsSortable.onAdd = function (/**Event*/evt) {
						var itemAddedContext = (ctrl.getChildContext != undefined) ? ctrl.getChildContext() : undefined;
						var obj = evt.model;
						if (ctrl.dragdropsetting.onItemReceived != undefined && typeof (ctrl.dragdropsetting.onItemReceived) == 'function')
							obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models, evt.source, itemAddedContext);
						evt.models[evt.newIndex] = obj;
					};
				}

				var directiveConstructor = new DirectiveConstructor($scope, ctrl);
				directiveConstructor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowContainerV2Template.html'
		};

		function DirectiveConstructor($scope, ctrl) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.isVRWorkflowActivityDisabled = false;
				$scope.scopeModel.datasource = [];

				$scope.scopeModel.isValid = function () {
					if ($scope.scopeModel.isVRWorkflowActivityDisabled)
						return null;

					if (ctrl.acceptEmptyContainer)
						return null;

					if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
						return "You should add at least one enabled activity.";

					for (var x = 0; x < $scope.scopeModel.datasource.length; x++) {
						var item = $scope.scopeModel.datasource[x];
						if (item.directiveAPI != null && item.directiveAPI.getActivityStatus != undefined) {
							var isDisabled = item.directiveAPI.getActivityStatus();
							if (!isDisabled)
								return null;
						}
					}
					return "You should add at least one enabled activity.";
				};

				$scope.scopeModel.onRemove = function (vRWorkflowActivityId) {
					for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
						if ($scope.scopeModel.datasource[i].VRWorkflowActivityId == vRWorkflowActivityId) {
							$scope.scopeModel.datasource.splice(i, 1);
							break;
						}
					}
				};
				$scope.scopeModel.dragdropsetting = ctrl.dragdropsetting;
				defineAPI();
			}

			function defineAPI() {

				var api = {};
				api.load = function (payload) {
					var promises = [];
					function extendDataItem(dataItem) {
						var promiseDeferred = UtilsService.createPromiseDeferred();
						promises.push(promiseDeferred.promise);
						dataItem.onDirectiveReady = function (api) {
							if (dataItem.directiveAPI != null)
								return;
							dataItem.directiveAPI = api;
							var setLoader = function (value) { };
							var directivePayload = {
								Context: (ctrl.getChildContext != undefined) ? ctrl.getChildContext() : undefined,
								VRWorkflowActivityId: dataItem.VRWorkflowActivityId,
								Settings: dataItem.Settings
							};
							VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, directivePayload, promiseDeferred);
						};
					}

					if (payload != undefined) {
						ctrl.getChildContext = payload.getChildContext;

						if (payload.vRWorkflowActivity != null) {
							extendDataItem(payload.vRWorkflowActivity);
							$scope.scopeModel.datasource = [payload.vRWorkflowActivity];
						}

						if (payload.vRWorkflowActivities != null && payload.vRWorkflowActivities.length > 0) {
							$scope.scopeModel.datasource = [];
							for (var i = 0; i < payload.vRWorkflowActivities.length; i++) {
								{
									var currentActivity = payload.vRWorkflowActivities[i];
									extendDataItem(currentActivity);
									$scope.scopeModel.datasource.push(currentActivity);
								}
							}
						}
					}
					return UtilsService.waitMultiplePromises(promises);
				};
				api.getData = function () {
					var activities = [];
					for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
						var item = $scope.scopeModel.datasource[i];
						if (item.directiveAPI != null)
							activities.push(item.directiveAPI.getData());
					}
					if (activities.length == 0)
						activities = undefined;

					if (ctrl.singleActivity && activities != undefined && activities.length > 0)
						return activities[0];

					return activities;
				};

				api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
					$scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
					for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
						var item = $scope.scopeModel.datasource[i];
						if (item.directiveAPI != null && item.directiveAPI.changeActivityStatus != undefined) {
							item.directiveAPI.changeActivityStatus(isVRWorkflowActivityDisabled);
						}
					}
				};

				api.getActivityStatus = function () {
					return $scope.scopeModel.isVRWorkflowActivityDisabled;
				};

				api.isActivityValid = function () {
					if ($scope.scopeModel.isValid() != null)
						return false;

					var result = true;
					for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
						var item = $scope.scopeModel.datasource[i];
						if (item.directiveAPI != null && item.directiveAPI.isActivityValid != undefined) {
							result = result && item.directiveAPI.isActivityValid();
						}
					}
					return result;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);
