"use strict"

app.directive("demoModuleCityGrid", ["Demo_Module_CityAPIService", "Demo_Module_CityService", "VRNotificationService",
	function (Demo_Module_CityAPIService, Demo_Module_CityService, VRNotificationService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var cityGrid = new CityGrid($scope, ctrl, $attrs);
				cityGrid.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/Templates/CityGridTemplate.html"
		};

		function CityGrid($scope, ctrl) {
			this.initializeController = initializeController;

			var countryId;

			var gridAPI;

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.cities = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object
					return Demo_Module_CityAPIService.GetFilteredCities(dataRetrievalInput).then(function (response) {
						onResponseReady(response);
					}).catch(function (error) {
						VRNotificationService.notifyException(error, $scope);
					});
				};

				defineMenuActions();
			};
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var query;

					if (payload != undefined) {
						query = payload.query;
						countryId = payload.countryId;

						if (payload.hideCountryColumn != undefined) {
							$scope.scopeModel.hideCountryColumn = payload.hideCountryColumn;
						}
					}

					return gridAPI.retrieveData(query);
				};

				api.onCityAdded = function (city) {
					gridAPI.itemAdded(city);
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
					ctrl.onReady(api);
			}

			function defineMenuActions() {
				$scope.scopeModel.gridMenuActions = [{
					name: "Edit",
					clicked: editCity
				}];
			};

			function editCity(city) {
				var onCityUpdated = function (city) {
					gridAPI.itemUpdated(city);
				};
				var countryIdItem = countryId != undefined ? { CountryId: countryId } : undefined;
				Demo_Module_CityService.editCity(onCityUpdated, city.CityId, countryIdItem);
			};
		};

		return directiveDefinitionObject;
	}]);
