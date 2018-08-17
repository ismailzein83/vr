"use strict";

app.directive("businessprocessVrCallhttpserviceHeadersGrid", ["UtilsService",
	function (UtilsService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},

			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var headersGrid = new HeadersGrid($scope, ctrl, $attrs);
				headersGrid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/CallHttpService/Templates/VRCallHttpServiceHeadersGridTemplate.html'
		};

		function HeadersGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.headers = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.addRow = function (data) {
					$scope.scopeModel.headers.push({ Entity: {} });
				};

				$scope.scopeModel.deleteRow = function (dataItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.headers, dataItem.Entity.Key, 'Entity.Key');
					if (index > -1) {
						$scope.scopeModel.headers.splice(index, 1);
					}
				};

				$scope.scopeModel.validateColumns = function () {
					//if ($scope.scopeModel.headers == undefined || $scope.scopeModel.headers.length == 0) {
					//	return 'Please, one record must be added at least.';
					//}

					var keys = [];
					for (var i = 0; i < $scope.scopeModel.headers.length; i++) {
						if ($scope.scopeModel.headers[i].Entity.Key != undefined) {
							var key = $scope.scopeModel.headers[i].Entity.Key.toLowerCase();
							if (UtilsService.contains(keys, key))
								return 'Two or more headers have the same Key';
							keys.push(key);
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

						if (payload.headers != undefined) {
							for (var i = 0; i < payload.headers.length; i++)
								$scope.scopeModel.headers.push({
									Entity: payload.headers[i]
								});
						}
					}
				};

				api.getData = function () {
					return $scope.scopeModel.headers.map(a => a.Entity);
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);