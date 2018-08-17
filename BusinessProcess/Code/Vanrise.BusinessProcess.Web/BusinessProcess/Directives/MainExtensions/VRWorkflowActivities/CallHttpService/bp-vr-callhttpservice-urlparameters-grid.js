"use strict";

app.directive("businessprocessVrCallhttpserviceUrlparametersGrid", ["UtilsService",
	function (UtilsService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},

			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var urlParametersGrid = new URLParametersGrid($scope, ctrl, $attrs);
				urlParametersGrid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/CallHttpService/Templates/VRCallHttpServiceURLParameterGridTemplate.html'
		};

		function URLParametersGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.urlParameters = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.addRow = function (data) {
					$scope.scopeModel.urlParameters.push({ Entity: {} });
				};

				$scope.scopeModel.deleteRow = function (dataItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.urlParameters, dataItem.Entity.Name, 'Entity.Name');
					if (index > -1) {
						$scope.scopeModel.urlParameters.splice(index, 1);
					}
				};

				$scope.scopeModel.validateColumns = function () {
					//if ($scope.scopeModel.urlParameters == undefined || $scope.scopeModel.urlParameters.length == 0) {
					//	return 'Please, one record must be added at least.';
					//}

					var names = [];
					for (var i = 0; i < $scope.scopeModel.urlParameters.length; i++) {
						if ($scope.scopeModel.urlParameters[i].Name != undefined) {
							var name = $scope.scopeModel.urlParameters[i].Name.toLowerCase();
							if (UtilsService.contains(names, name))
								return 'Two or more URL parameters have the same name';
							names.push(name);
						}
					}
					return null;
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined) {
						$scope.scopeModel.getWorkflowArguments = payload.getWorkflowArguments;
						$scope.scopeModel.getParentVariables = payload.getParentVariables;

						if (payload.urlParameters != undefined) {
							for (var i = 0; i < payload.urlParameters.length; i++)
								$scope.scopeModel.urlParameters.push({
									Entity: payload.urlParameters[i]
								});
						}
					}
				};

				api.getData = function () {
					return $scope.scopeModel.urlParameters.map(a => a.Entity);
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);