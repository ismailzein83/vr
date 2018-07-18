'use strict';

app.directive('businessprocessVrWorkflowArgumentsGrid', ['BusinessProcess_VRWorkflowService', 'UtilsService', 'VRUIUtilsService', 'VRWorkflowArgumentDirectionEnum', 'BusinessProcess_VRWorkflowAPIService',
	function (BusinessProcess_VRWorkflowService, UtilsService, VRUIUtilsService, VRWorkflowArgumentDirectionEnum, BusinessProcess_VRWorkflowAPIService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new VrWorkflowArgumentsGridDirectiveCtor(ctrl, $scope, $attrs);
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
			templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowArgumentsGridTemplate.html"
		};

		function VrWorkflowArgumentsGridDirectiveCtor(ctrl, $scope, attrs) {
			this.initializeController = initializeController;

			var gridArgumentItem;

			var gridAPI;

			var directionSelectorAPI;
			var directionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var variableTypeDirectiveAPI;
			var variableTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

			var reserveVariableName;
			var reserveVariableNames;
			var isVariableNameReserved;
			var eraseVariableName;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.datasource = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
				};

				$scope.scopeModel.addVRWorkflowArgument = function () {
					var onVRWorkflowArgumentAdded = function (addedArgument) {
						extendVRWorkflowArgument(addedArgument);
						reserveVariableName(addedArgument.Name);
						getVRWorkflowArgumentTypeDescription(addedArgument).then(function () {
							$scope.scopeModel.datasource.push({ Entity: addedArgument });
						});
					};

					var vrWorkflowArgumentNames = [];
					angular.forEach($scope.scopeModel.datasource, function (val) {
						vrWorkflowArgumentNames.push(val.Entity.Name);
					});
					BusinessProcess_VRWorkflowService.addVRWorkflowArgument(vrWorkflowArgumentNames, onVRWorkflowArgumentAdded, isVariableNameReserved);
				};

				$scope.scopeModel.removeVRWorkflowArgument = function (dataItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.VRWorkflowArgumentId, 'Entity.VRWorkflowArgumentId');
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

					var vrWorkflowArguments;
					var vrWorkflowArgumentEditorRuntimeDict;

					if (payload != undefined) {
						reserveVariableName = payload.reserveVariableName;
						reserveVariableNames = payload.reserveVariableNames;
						isVariableNameReserved = payload.isVariableNameReserved;
						eraseVariableName = payload.eraseVariableName;
						vrWorkflowArguments = payload.vrWorkflowArguments;
						vrWorkflowArgumentEditorRuntimeDict = payload.vrWorkflowArgumentEditorRuntimeDict;
					}

					if (vrWorkflowArguments != undefined) {
						for (var i = 0; i < vrWorkflowArguments.length; i++) {
							gridArgumentItem = vrWorkflowArguments[i];
							extendVRWorkflowArgument(gridArgumentItem);

							var vrWorkflowArgumentEditorRuntime = vrWorkflowArgumentEditorRuntimeDict[gridArgumentItem.VRWorkflowArgumentId];
							gridArgumentItem.TypeDescription = vrWorkflowArgumentEditorRuntime.VRWorkflowVariableTypeDescription;

							$scope.scopeModel.datasource.push({ Entity: gridArgumentItem });
						}
					}

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					var vrWorkflowArguments;
					if ($scope.scopeModel.datasource != undefined) {
						vrWorkflowArguments = [];
						for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
							vrWorkflowArguments.push($scope.scopeModel.datasource[i].Entity);
						}
					}
					return vrWorkflowArguments;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function defineMenuActions() {
				var defaultMenuActions = [{
					name: "Edit",
					clicked: editVRWorkflowArgument
				}];

				$scope.scopeModel.gridMenuActions = function (dataItem) {
					return defaultMenuActions;
				};
			}

			function editVRWorkflowArgument(argumentObj) {
				var onArgumentUpdated = function (updatedArgument) {
					extendVRWorkflowArgument(updatedArgument);
					getVRWorkflowArgumentTypeDescription(updatedArgument).then(function () {
						eraseVariableName(argumentObj.Entity.Name);
						reserveVariableName(updatedArgument.Name);
						var index = $scope.scopeModel.datasource.indexOf(argumentObj);
						$scope.scopeModel.datasource[index] = { Entity: updatedArgument };
					});
				};

				var vrWorkflowArgumentNames = [];
				angular.forEach($scope.scopeModel.datasource, function (val) {
					vrWorkflowArgumentNames.push(val.Entity.Name);
				});
				BusinessProcess_VRWorkflowService.editVRWorkflowArgument(argumentObj.Entity, vrWorkflowArgumentNames, onArgumentUpdated, isVariableNameReserved);
			}

			function extendVRWorkflowArgument(vrWorkflowArgument) {
				var argumentDirectionObject = UtilsService.getEnum(VRWorkflowArgumentDirectionEnum, 'value', vrWorkflowArgument.Direction);
				if (argumentDirectionObject != undefined) {
					vrWorkflowArgument.DirectionDescription = argumentDirectionObject.description;
				}
			}

			function getVRWorkflowArgumentTypeDescription(vrWorkflowArgument) {
				return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowArgumentTypeDescription(vrWorkflowArgument.Type).then(function (response) {
					vrWorkflowArgument.TypeDescription = response;
				});
			}
		}

		return directiveDefinitionObject;
	}]);