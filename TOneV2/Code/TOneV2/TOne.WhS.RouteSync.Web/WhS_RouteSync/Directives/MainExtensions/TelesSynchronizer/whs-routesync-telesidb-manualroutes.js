"use strict";
app.directive("whsRoutesyncTelesidbManualroutes", ["UtilsService",
	function (UtilsService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var manualRoutesDirective = new ManualRoutesDirective($scope, ctrl, $attrs);
				manualRoutesDirective.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/TelesSynchronizer/Templates/TelesIdbManualRoutesTemplate.html'
		};

		function ManualRoutesDirective($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;
			var routePrefs;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.manualRoutes = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;
					if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
						ctrl.onReady(getDirectiveAPI());
				};

				$scope.scopeModel.addRoute = function () {
					$scope.scopeModel.manualRoutes.push({});
				};

				$scope.scopeModel.removeRoute = function (dataItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.manualRoutes, dataItem.Pref, 'Pref');
					if (index > -1) {
						$scope.scopeModel.manualRoutes.splice(index, 1);
					}
				};

				$scope.scopeModel.validateRoutes = function () {
					routePrefs = [];
					if ($scope.scopeModel.manualRoutes != undefined && $scope.scopeModel.manualRoutes.length > 0) {
						for (var i = 0; i < $scope.scopeModel.manualRoutes.length; i++) {
							var pref = $scope.scopeModel.manualRoutes[i].Pref;
							if (pref != undefined) {
								pref = pref.toLowerCase();
								if (isPrefAlreadyExist(pref))
									return 'Two or more manual routes have the same Pref';
								else
									routePrefs.push(pref);
							}
						}
					}
					return null;
				};

				function isPrefAlreadyExist(pref) {
					if (routePrefs != undefined && routePrefs.length > 0) {
						for (var i = 0; i < routePrefs.length; i++) {
							if (routePrefs[i] == pref)
								return true;
						}
					}
					return false;
				}

			}

			function getDirectiveAPI() {
				var directiveAPI = {};

				directiveAPI.load = function (payload) {
					if (payload != undefined && payload.manualRoutes != undefined)
						$scope.scopeModel.manualRoutes = payload.manualRoutes;
				};

				directiveAPI.getData = function () {
					return $scope.scopeModel.manualRoutes;
				};

				return directiveAPI;
			}
		}

		return directiveDefinitionObject;
	}]);