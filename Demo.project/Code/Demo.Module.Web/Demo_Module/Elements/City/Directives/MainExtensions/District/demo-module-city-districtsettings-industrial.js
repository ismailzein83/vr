"use strict";

app.directive("demoModuleCityDistrictsettingsIndustrial", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
	function (UtilsService, VRNotificationService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "=",
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new IndustrialDistrict($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/MainExtensions/District/Templates/IndustrialDistrictTemplate.html"
		};

		function IndustrialDistrict($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var districtSettings;

			var factoryGridAPI;
			var factoryGridReadyDeferred = UtilsService.createPromiseDeferred();

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onFactoryGridReady = function (api) {
					factoryGridAPI = api;
					factoryGridReadyDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];

					if (payload != undefined && payload.districtSettings != undefined) {
						districtSettings = payload.districtSettings;
						$scope.scopeModel.numberOfFactories = districtSettings != undefined ? districtSettings.FactoriesNumber : undefined;

						var loadDirectivePromise = loadFactoryGrid();
						promises.push(loadDirectivePromise);
					}

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Demo.Module.MainExtension.District.IndustrialDistrict, Demo.Module.MainExtension",
						FactoriesNumber: $scope.scopeModel.numberOfFactories,
						Factories: factoryGridAPI.getData()
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function loadFactoryGrid() {
				var directiveLoadDeferred = UtilsService.createPromiseDeferred();

				factoryGridReadyDeferred.promise.then(function () {

					var directivePayload = {
						factories: districtSettings.Factories
					};
					VRUIUtilsService.callDirectiveLoad(factoryGridAPI, directivePayload, directiveLoadDeferred);
				});

				return directiveLoadDeferred.promise;
			}
		}

		return directiveDefinitionObject;
	}
]);