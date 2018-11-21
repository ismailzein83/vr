(function (app) {
	'use strict';

	districtManagementDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'Demo_Module_CityAPIService'];

	function districtManagementDirective(UtilsService, VRUIUtilsService, Demo_Module_CityAPIService) {

		return {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new DistrictManagement($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/Demo_Module/Elements/City/Directives/Templates/DistrictManagementTemplate.html"
		};

		function DistrictManagement($scope, ctrl, attrs) {
			this.initializeController = initializeController;

			var gridAPI;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.isLoading = false;

				$scope.scopeModel.onDistrictGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.addDistrict = function () {
					gridAPI.addDistrict();
				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					gridAPI.load(payload);
				};

				api.getData = function () {
					return gridAPI.getData();
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
					ctrl.onReady(api);
			}
		}
	}

	app.directive("demoModuleCityDistrictManagement", districtManagementDirective);
})(app);