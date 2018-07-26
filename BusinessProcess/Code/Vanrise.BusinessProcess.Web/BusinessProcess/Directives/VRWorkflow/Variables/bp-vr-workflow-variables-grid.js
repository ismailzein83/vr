'use strict';

app.directive('businessprocessVrWorkflowVariablesGrid', ['BusinessProcess_VRWorkflowService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService',
	function (BusinessProcess_VRWorkflowService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				readOnly: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new VrWorkflowVariablesGridDirectiveCtor(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {
				return {
					pre: function ($scope, iElem, iAttrs, ctrl) {

					}
				};
			},
			templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Variables/Templates/VRWorkflowVariablesGridTemplate.html"
		};

		function VrWorkflowVariablesGridDirectiveCtor(ctrl, $scope, attrs) {
			this.initializeController = initializeController;

			var gridAPI;
			var variableTypeDirectiveAPI;
			var variableTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
			var reserveVariableName;
			var eraseVariableName;
			var isVariableNameReserved;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.datasource = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
				};

				$scope.scopeModel.addVRWorkflowVariable = function () {
					var onVRWorkflowVariableAdded = function (addedVariable) {
						getVRWorkflowVariableTypeDescription(addedVariable).then(function () {
							$scope.scopeModel.datasource.push({ Entity: addedVariable });
							reserveVariableName(addedVariable.Name);
						});
					};
					BusinessProcess_VRWorkflowService.addVRWorkflowVariable(onVRWorkflowVariableAdded, isVariableNameReserved);
				};

				$scope.scopeModel.removeVRWorkflowVariable = function (dataItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.VRWorkflowVariableId, 'Entity.VRWorkflowVariableId');
					$scope.scopeModel.datasource.splice(index, 1);
					eraseVariableName(dataItem.Entity.Name);
				};

				defineMenuActions();
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];

					var vrWorkflowVariables;
					var vRWorkflowVariablesTypeDescription;

					if (payload != undefined) {
						isVariableNameReserved = payload.isVariableNameReserved;
						reserveVariableName = payload.reserveVariableName;
						eraseVariableName = payload.eraseVariableName;
						vrWorkflowVariables = payload.vrWorkflowVariables;
						vRWorkflowVariablesTypeDescription = payload.vRWorkflowVariablesTypeDescription;
					}

					if (vrWorkflowVariables != undefined) {
						for (var i = 0; i < vrWorkflowVariables.length; i++) {
							var gridVariableItem = vrWorkflowVariables[i];

							var vRWorkflowVariableTypeDescription = vRWorkflowVariablesTypeDescription[gridVariableItem.VRWorkflowVariableId];
							gridVariableItem.TypeDescription = vRWorkflowVariableTypeDescription;
							$scope.scopeModel.datasource.push({ Entity: gridVariableItem });
						}
					}

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					var vrWorkflowVariables;
					if ($scope.scopeModel.datasource != undefined) {
						vrWorkflowVariables = [];
						for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
							vrWorkflowVariables.push($scope.scopeModel.datasource[i].Entity);
						}
					}
					return vrWorkflowVariables;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function defineMenuActions() {
				var defaultMenuActions = [{
					name: "Edit",
					clicked: editVRWorkflowVariable
				}];

				$scope.scopeModel.gridMenuActions = function (dataItem) {
					if (!ctrl.readOnly)
						return defaultMenuActions;
				};
			}

			function editVRWorkflowVariable(variableObj) {
				var onVRWorkflowVariableUpdated = function (updatedVariable) {
					getVRWorkflowVariableTypeDescription(updatedVariable).then(function () {
						var index = $scope.scopeModel.datasource.indexOf(variableObj);
						$scope.scopeModel.datasource[index] = { Entity: updatedVariable };
						eraseVariableName(variableObj.Entity.Name);
						reserveVariableName(updatedVariable.Name);
					});
				};
				BusinessProcess_VRWorkflowService.editVRWorkflowVariable(variableObj.Entity, onVRWorkflowVariableUpdated);
			}

			function getVRWorkflowVariableTypeDescription(vrWorkflowVariable) {
				return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowVariableTypeDescription(vrWorkflowVariable.Type).then(function (response) {
					vrWorkflowVariable.TypeDescription = response;
				});
			}
		}

		return directiveDefinitionObject;
	}]);