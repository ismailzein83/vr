app.service("Demo_Module_CountryService", ['VRModalService', 'VRNotificationService',
	function (VRModalService, VRNotificationService) {

		function addCountry(onCountryAdded) {
			var settings = {};
			var parameters = {};

			settings.onScopeReady = function (modalScope) {
				modalScope.onCountryAdded = onCountryAdded;
			};

			VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Country/Views/CountryEditor.html', parameters, settings)
		};

		function editCountry(countryId, onCountryUpdated) {
			var parameters = {
				countryId: countryId
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onCountryUpdated = onCountryUpdated;
			};

			VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Country/Views/CountryEditor.html', parameters, settings);
		};

		return {
			addCountry: addCountry,
			editCountry: editCountry
		};
	}]);