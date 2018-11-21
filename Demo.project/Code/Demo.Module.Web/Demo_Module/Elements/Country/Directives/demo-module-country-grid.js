'use strict';

app.directive("demoModuleCountryGrid", ['UtilsService', 'VRNotificationService', 'Demo_Module_CountryAPIService', 'Demo_Module_CountryService', 'VRUIUtilsService',
	function (UtilsService, VRNotificationService, Demo_Module_CountryAPIService, Demo_Module_CountryService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var countryGrid = new CountryGrid($scope, ctrl, $attrs);
				countryGrid.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/Country/Directives/Templates/CountryGridTemplate.html"
		}

		function CountryGrid($scope, ctrl) {
			this.initializeController = initializeController;

			var gridAPI;
			var gridDrillDownTabsObj;


			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.countries = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownDefinitions(), gridAPI);
					defineAPI();
				}

				$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return Demo_Module_CountryAPIService.GetFilteredCountries(dataRetrievalInput).then(function (response) {
						if (response && response.Data) {
							for (var i = 0; i < response.Data.length; i++) {
								gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
							}
						}
						onResponseReady(response);
					}).catch(function (error) {
						VRNotificationService.notifyException(error, $scope);
					});
				};

				defineMenuActions();
			}

			function buildDrillDownDefinitions() {
				var drillDownDefinitions = [];
				drillDownDefinitions.push(buildCityDrillDownDefinition());
				return drillDownDefinitions;
			}

			function buildCityDrillDownDefinition() {
				var drillDownDefinition = {};

				drillDownDefinition.title = "City";
				drillDownDefinition.directive = "demo-module-city-search";

				drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {
					countryItem.cityGridAPI = directiveAPI;
					var payload = {
						countryId: countryItem.CountryId
					};
					return countryItem.cityGridAPI.load(payload);
				};

				return drillDownDefinition;
			}

			function defineAPI() {
				var api = {};

				api.load = function (query) {
					return gridAPI.retrieveData(query);
				};

				api.onCountryAdded = function (country) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(country);
					gridAPI.itemAdded(country);
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
					ctrl.onReady(api);
			}

			function defineMenuActions() {
				$scope.scopeModel.gridMenuActions = [{
					name: "Edit",
					clicked: editCountry
				}];
			}

			function editCountry(country) {
				var onCountryUpdated = function (country) {
					gridDrillDownTabsObj.setDrillDownExtensionObject(country);
					gridAPI.itemUpdated(country);
				};
				Demo_Module_CountryService.editCountry(country.CountryId, onCountryUpdated);
			}
		}

		return directiveDefinitionObject;
	}]);