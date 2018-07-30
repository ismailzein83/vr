"use strict";

app.directive('businessprocessVrWorkflowContainer', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {
		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "=",
				dragdropsetting: '=',
				//datasource: '='
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
							return (ctrl.dragdropsetting.canReceive && to.el.children.length < 1);
						}
					};

					ctrl.itemsSortable.onAdd = function (/**Event*/evt) {
						var obj = evt.model;
						if (ctrl.dragdropsetting.onItemReceived != undefined && typeof (ctrl.dragdropsetting.onItemReceived) == 'function')
							obj = ctrl.dragdropsetting.onItemReceived(evt.model, evt.models, evt.source);
						evt.models[evt.newIndex] = obj;
					};
				}

				var directiveConstructor = new DirectiveConstructor($scope, ctrl);
				directiveConstructor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowContainerTemplate.html'
		};

		function DirectiveConstructor($scope, ctrl) {
			//if (ctrl.datasource == undefined)
			//ctrl.datasource = [];
			this.initializeController = initializeController;
			var allVariableNames = [];
			//var workflowArguments;
			var getWorkflowArguments;
			var reserveVariableName;
			var reserveVariableNames;
			var isVariableNameReserved;
			var eraseVariableName;

			var compiled = false;
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.datasource = [];
				$scope.scopeModel.onRemove = function (vRWorkflowActivityId) {
					//for (var i = 0; i < ctrl.datasource.length; i++) {
					//	if (ctrl.datasource[i].VRWorkflowActivityId == vRWorkflowActivityId) {
					//		ctrl.datasource.splice(i, 1);
					//		break;
					//	}
					//}
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

			function extendDataItem(dataItem) {
				dataItem.onDirectiveReady = function (api) {
					dataItem.directiveAPI = api;
					var setLoader = function (value) { };
					var directivePayload = {
						Context: {
							getWorkflowArguments: getWorkflowArguments,
							reserveVariableName: reserveVariableName,
							reserveVariableNames: reserveVariableNames,
							eraseVariableName: eraseVariableName,
							isVariableNameReserved: isVariableNameReserved
						},
						VRWorkflowActivityId: dataItem.VRWorkflowActivityId,
						Settings: dataItem.Settings
					};
					VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, directivePayload, setLoader);
				};
			}

			function defineAPI() {

				var api = {};
				api.load = function (payload) {
					var workflowArguments;
					if (payload != undefined) {
						reserveVariableName = payload.reserveVariableName;
						reserveVariableNames = payload.reserveVariableNames;
						isVariableNameReserved = payload.isVariableNameReserved;
						eraseVariableName = payload.eraseVariableName;

						getWorkflowArguments = payload.getWorkflowArguments;
						if (getWorkflowArguments != undefined)
							workflowArguments = getWorkflowArguments();
						if (workflowArguments != undefined)
							allVariableNames = allVariableNames.concat(workflowArguments.map(a => a.Name));
						if (payload.vRWorkflowActivity != null) {
							extendDataItem(payload.vRWorkflowActivity);
							$scope.scopeModel.datasource = [payload.vRWorkflowActivity];
						}
					}
				};

				api.getData = function () {
					//if (ctrl.datasource.length > 0 && ctrl.datasource[0].directiveAPI != null) {
					//	return ctrl.datasource[0].directiveAPI.getData();
					//}
					if ($scope.scopeModel.datasource.length > 0 && $scope.scopeModel.datasource[0].directiveAPI != null) {
						return $scope.scopeModel.datasource[0].directiveAPI.getData();
					}
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);
