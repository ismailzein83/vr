(function (app) {

	"use strict";

	citySettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_CityAPIService'];

	function citySettingsDirective(UtilsService, VRUIUtilsService, Demo_Module_CityAPIService) {
		return {
			restrict: "E",
			scope: {
				onReady: "=",
				normalColNum: "@",
				label: "@",
				customvalidate: "=",
				isrequired: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new CitySettings($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/Templates/CitySettingsTemplate.html"
		};

		function CitySettings($scope, ctrl, attrs) {
			this.initializeController = initializeController;

			var selectorAPI;

			var cityTypeDirectiveAPI;
			var cityTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

			var cityDistrictsDirectiveAPI;
			var cityDistrictsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onCityTypeDirectiveReady = function (api) {
					cityTypeDirectiveAPI = api;
					cityTypeDirectiveReadyDeferred.resolve();
				};

				$scope.scopeModel.onDistrictManagementReady = function (api) {
					cityDistrictsDirectiveAPI = api;
					cityDistrictsDirectiveReadyDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {

					var promises = [];

					var citySettings;

					if (payload != undefined) {
						citySettings = payload.citySettings;
						if (citySettings != undefined) {
							$scope.scopeModel.population = citySettings.Population;
						}
					}

					var loadCityTypeDirectivePromise = loadCityTypeDirective();
					promises.push(loadCityTypeDirectivePromise);

					var loadCityDistrictGridPromise = loadCityDistrictGrid();
					promises.push(loadCityDistrictGridPromise);


					function loadCityTypeDirective() {
						var cityTypeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

						cityTypeDirectiveReadyDeferred.promise.then(function () {

							var cityTypePayload = {};
							if (citySettings != undefined) {
								cityTypePayload = { cityType: citySettings.CityType };
							}
							VRUIUtilsService.callDirectiveLoad(cityTypeDirectiveAPI, cityTypePayload, cityTypeDirectiveLoadDeferred);
						});

						return cityTypeDirectiveLoadDeferred.promise;
					}

					function loadCityDistrictGrid() {
						var districtGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

						cityDistrictsDirectiveReadyDeferred.promise.then(function () {

							var districtPayload = {};
							if (citySettings != undefined) {
								districtPayload = { districts: citySettings.Districts };
							}
							VRUIUtilsService.callDirectiveLoad(cityDistrictsDirectiveAPI, districtPayload, districtGridLoadPromiseDeferred);
						});

						return districtGridLoadPromiseDeferred.promise;
					}

					return UtilsService.waitMultiplePromises(promises);
				}

				api.getData = function () {
					var data = {};
					data.Population = $scope.scopeModel.population;

					if (cityTypeDirectiveAPI != undefined) {
						data.CityType = cityTypeDirectiveAPI.getData();
					}

					if (cityDistrictsDirectiveAPI != undefined) {
						data.Districts = cityDistrictsDirectiveAPI.getData();
					}

					return data;
				};

				if (ctrl.onReady != null) {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive("demoModuleCityCitysettings", citySettingsDirective);
})(app);