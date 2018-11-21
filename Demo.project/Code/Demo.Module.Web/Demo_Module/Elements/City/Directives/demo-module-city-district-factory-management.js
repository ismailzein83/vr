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
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/Templates/FactoryManagementTemplate.html"
		};

		function DistrictGridManagement($scope, ctrl, attrs) {
			this.initializeController = initializeController;

			var context;

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
				};

				$scope.scopeModel.validateFactories = function () {
					if (context != undefined) {
						var factoriesNumber = context.getFactoriesNumber();
						if ($scope.scopeModel.factories.length != factoriesNumber)
							return "You should add " + factoriesNumber + "  Factory.";
					}
					return null;
				};

				defineMenuActions();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined) {
						context = payload.context;
						if (payload.factories != undefined) {
							for (var i = 0; i < payload.factories.length; i++) {
								var currentFactory = {};
								currentFactory.Entity = payload.factories[i];
								$scope.scopeModel.factories.push(currentFactory);
							}
						}
					}
				};

				api.getData = function () {
					var factories = [];
					for (var i = 0; i < $scope.scopeModel.factories.length; i++) {
						var currentFactory = $scope.scopeModel.factories[i];
						factories.push(currentFactory.Entity);
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

				Demo_Module_CityService.editFactory(onFactoryUpdated, factory.Entity);
			}
		}
	}

	app.directive("demoModuleCityDistrictFactoryManagement", districtGridManagement);
})(app);