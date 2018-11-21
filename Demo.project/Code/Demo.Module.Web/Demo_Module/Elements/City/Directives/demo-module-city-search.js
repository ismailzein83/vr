"use strict";

app.directive("demoModuleCitySearch", ['Demo_Module_CityService',
	function (Demo_Module_CityService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new CitySearch($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/Templates/CitySearchTemplate.html"
		};

		function CitySearch($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var countryId;

			var gridAPI;

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.addCity = function () {
					var onCityAdded = function (obj) {
						gridAPI.onCityAdded(obj);
					};
					var countryIdItem = { CountryId: countryId };
					Demo_Module_CityService.addCity(onCityAdded, countryIdItem);
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined) {
						countryId = payload.countryId;
					}
					return gridAPI.load(getGridPayload());
				};

				api.onCityAdded = function (cityObject) {
					gridAPI.onCityAdded(cityObject);
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
					ctrl.onReady(api);
			}

			function getGridPayload() {
				var payload = {
					query: { CountryIds: [countryId] },
					countryId: countryId,
					hideCountryColumn: true
				};
				return payload;
			}
		}

		return directiveDefinitionObject;
	}]);
