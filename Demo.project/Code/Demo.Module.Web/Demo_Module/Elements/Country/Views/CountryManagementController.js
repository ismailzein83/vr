(function (appControllers) {
	'use strict';

	countryManagementController.$inject = ['$scope', 'Demo_Module_CountryAPIService','Demo_Module_CountryService', 'VRNotificationService'];

	function countryManagementController($scope, Demo_Module_CountryAPIService, Demo_Module_CountryService, VRNotificationService) {

		var gridAPI;

		defineScope();
		load();

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridAPI.load(getFilter());
			};

			$scope.scopeModel.search = function() {
				return gridAPI.load(getFilter());
			};

			$scope.scopeModel.addCountry = function () {
				var onCountryAdded = function (country) {
					if (gridAPI != undefined) {
						gridAPI.onCountryAdded(country);
					}
				};
				Demo_Module_CountryService.addCountry(onCountryAdded);
			};
		}

		function load() {

		}

		function getFilter() {
			return {
				Name: $scope.scopeModel.name
			};
		}
	}

	appControllers.controller("Demo_Module_CountryManagementController", countryManagementController);
})(appControllers);