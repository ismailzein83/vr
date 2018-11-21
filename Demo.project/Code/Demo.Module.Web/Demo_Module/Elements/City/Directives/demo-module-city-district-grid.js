(function (app) {
	'use strict';

	districtGridDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_CityAPIService'];

	function districtGridDirective(UtilsService, VRUIUtilsService, Demo_Module_CityAPIService) {

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
				var ctor = new DistrictGrid($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/Templates/DistrictGridTemplate.html"
		};

		function DistrictGrid($scope, ctrl, attrs) {
			this.initializeController = initializeController;

			var gridAPI;
			var gridDrillDownTabsObj;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.districts = [];
				$scope.scopeModel.isLoading = false;

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownDefinitions(), gridAPI);
					defineAPI();
				};

				$scope.scopeModel.onDeleteRow = function (item) {
					var index = $scope.scopeModel.districts.indexOf(item);
					$scope.scopeModel.districts.splice(index, 1);
				};

				$scope.scopeModel.validateDistricts = function () {
					if ($scope.scopeModel.districts.length == 0) {
						return "You should add at least one District.";
					}
					else {
						if (!isDistrictsTypesValid())
							return "You should define Districts Types";
						else
							return null;
					}
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.districts != undefined) {
						for (var i = 0; i < payload.districts.length; i++) {
							var currentDistrict = { entity: payload.districts[i] };
							gridDrillDownTabsObj.setDrillDownExtensionObject(currentDistrict);
							$scope.scopeModel.districts.push(currentDistrict);
						}
					}
				};

				api.getData = function () {
					var districts = [];
					for (var i = 0; i < $scope.scopeModel.districts.length; i++) {
						var item = $scope.scopeModel.districts[i];
						var currentDistrict = item.entity;
						currentDistrict.DistrictId = UtilsService.guid();
						currentDistrict.Settings = item.districtSettingsAPI != undefined ? item.districtSettingsAPI.getData() : item.entity.Settings;
						districts.push(currentDistrict);
					}
					return districts;
				};

				api.addDistrict = function (district) {
					if (district == undefined) {
						district = { entity: { Name: undefined, Population: undefined } };
					}
					gridDrillDownTabsObj.setDrillDownExtensionObject(district);
					$scope.scopeModel.districts.push(district);
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
					ctrl.onReady(api);
			}

			function isDistrictsTypesValid() {
				var isValid = true;
				for (var i = 0; i < $scope.scopeModel.districts.length; i++) {
					var item = $scope.scopeModel.districts[i];
					var districtSettings = item.districtSettingsAPI != undefined ? item.districtSettingsAPI.getData() : item.entity.Settings;
					if (districtSettings == undefined) {
						isValid = false;
						break;
					}
				}
				return isValid;
			}

			function buildDrillDownDefinitions() {
				var drillDownDefinitions = [];
				drillDownDefinitions.push(buildDistrictDrillDownDefinition());
				return drillDownDefinitions;
			}

			function buildDistrictDrillDownDefinition() {
				var drillDownDefinition = {};

				drillDownDefinition.title = "Settings";
				drillDownDefinition.directive = "demo-module-city-districtsettings";

				drillDownDefinition.loadDirective = function (directiveAPI, districtItem) {
					districtItem.districtSettingsAPI = directiveAPI;
					var payload = {
						districtSettings: districtItem.entity.Settings
					};
					return districtItem.districtSettingsAPI.load(payload);
				};

				return drillDownDefinition;
			}
		}
	}

	app.directive("demoModuleCityDistrictGrid", districtGridDirective);
})(app);