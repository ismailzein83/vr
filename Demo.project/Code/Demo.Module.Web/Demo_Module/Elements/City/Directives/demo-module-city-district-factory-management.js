(function (app) {

	"use strict";

	districtGridManagement.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_CityAPIService', 'Demo_Module_CityService'];

	function districtGridManagement(UtilsService, VRUIUtilsService, Demo_Module_CityAPIService, Demo_Module_CityService) {

		return {
			restrict: "E",
			scope: {
				onReady: "=",
				isrequired: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new DistrictGridManagement($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/Templates/FactoryGridTemplate.html"
		};

		function DistrictGridManagement($scope, ctrl, attrs) {
			this.initializeController = initializeController;

			var gridAPI;
			var gridDrillDownTabsObj;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.factories = [];
				$scope.scopeModel.isLoading = false;

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.addFactory = function () {
					var onFactoryAdded = function (factory) {
						$scope.scopeModel.factories.push(factory);
					};

					Demo_Module_CityService.addFactory(onFactoryAdded);
				};

				$scope.scopeModel.onDeleteRow = function (item) {
					var index = $scope.scopeModel.factories.indexOf(item);
					$scope.scopeModel.factories.splice(index, 1);
				}

				defineMenuActions();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.factories != undefined) {
						for (var i = 0; i < payload.factories.length; i++) {
							var currentFactory = {};
							currentFactory.entity = payload.factories[i];
							$scope.scopeModel.factories.push(currentFactory);
						}
					}
				};

				api.getData = function () {
					var factories = [];
					for (var i = 0; i < $scope.scopeModel.factories.length; i++) {
						var currentFactory = $scope.scopeModel.factories[i];
						factories.push(currentFactory.entity);
					}
					return factories;
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
					ctrl.onReady(api);
			}

			function defineMenuActions() {
				$scope.scopeModel.gridMenuActions = [{
					name: "Edit",
					clicked: editFactory,
				}];
			}

			function editFactory(factory) {
				var onFactoryUpdated = function (factoryUpdated) {
					var index = $scope.scopeModel.factories.indexOf(factory);
					$scope.scopeModel.factories[index] = factoryUpdated;
				};

				Demo_Module_CityService.editFactory(onFactoryUpdated, factory.entity);
			}
		}
	}

	app.directive("demoModuleCityDistrictFactoryManagement", districtGridManagement);
})(app);