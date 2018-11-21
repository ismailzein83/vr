(function (appControllers) {
	'use strict';

	cityManagementController.$inject = ['$scope', 'Demo_Module_CityService', 'Demo_Module_CityAPIService'];

	function cityManagementController($scope, Demo_Module_CityService, Demo_Module_CityAPIService) {

		var gridAPI;
		var countrySelectorAPI;
		defineScope();
		load();

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridAPI.load(getFilter());
			};

			$scope.scopeModel.onCountrySelectorReady = function (api) {
				countrySelectorAPI = api;
				countrySelectorAPI.load();
			};

			$scope.scopeModel.search = function () {
				return gridAPI.load(getFilter());
			};

			$scope.scopeModel.addCity = function () {
				var onCityAdded = function (city) {
					if (gridAPI != undefined) {
						gridAPI.onCityAdded(city);
					}
				};
				Demo_Module_CityService.addCity(onCityAdded);
			};
		}

		function load() {

		}

		function getFilter() {
			return {
				query: {
					Name: $scope.scopeModel.name,
					CountryIds : countrySelectorAPI.getSelectedIds()
				}
			};
		}
	}
	appControllers.controller("Demo_Module_CityManagementController", cityManagementController);
})(appControllers);