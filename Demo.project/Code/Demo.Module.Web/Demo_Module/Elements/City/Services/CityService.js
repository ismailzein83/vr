"use strict";

app.service("Demo_Module_CityService", ['VRModalService', 'VRNotificationService',
	function (VRModalService, VRNotificationService) {

		function addCity(onCityAdded, countryIdItem) {

			var parameters = {
				countryIdItem: countryIdItem
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onCityAdded = onCityAdded;
			};
			VRModalService.showModal('/Client/Modules/Demo_Module/Elements/City/Views/CityEditor.html', parameters, settings);
		};

		function editCity(onCityUpdated, cityId, countryIdItem) {

			var parameters = {
				cityId: cityId,
				countryIdItem: countryIdItem
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onCityUpdated = onCityUpdated;
			};
			VRModalService.showModal('/Client/Modules/Demo_Module/Elements/City/Views/CityEditor.html', parameters, settings);
		};

		function addFactory(onFactoryAdded) {

			var parameters = {
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onFactoryAdded = onFactoryAdded;
			};
			VRModalService.showModal('/Client/Modules/Demo_Module/Elements/City/Views/FactoryEditor.html', parameters, settings);
		};

		function editFactory(onFactoryUpdated, factoryItem) {

			var parameters = {
				factoryEntity: factoryItem
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onFactoryUpdated = onFactoryUpdated;
			};
			VRModalService.showModal('/Client/Modules/Demo_Module/Elements/City/Views/FactoryEditor.html', parameters, settings);
		};

		return {
			addCity: addCity,
			editCity: editCity,
			addFactory: addFactory,
			editFactory: editFactory
		};
	}]);