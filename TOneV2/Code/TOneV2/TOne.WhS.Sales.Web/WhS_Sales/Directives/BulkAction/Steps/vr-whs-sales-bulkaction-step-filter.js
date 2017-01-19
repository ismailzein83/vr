'use strict';

app.directive('vrWhsSalesBulkactionStepFilter', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var filterBulkActionStep = new FilterBulkActionStep($scope, ctrl, $attrs);
			filterBulkActionStep.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Sales/Directives/BulkAction/Steps/Templates/FilterBulkActionStepTemplate.html'
	};

	function FilterBulkActionStep($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var zoneFilterSelectiveAPI;

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.onZoneFilterSelectiveReady = function (api) {
				zoneFilterSelectiveAPI = api;
				defineAPI();
			};
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				return zoneFilterSelectiveAPI.load(payload);
			};

			api.getData = function () {
				return {
					zoneFilter: zoneFilterSelectiveAPI.getData()
				};
			};

			if (ctrl.onReady != null) {
				ctrl.onReady(api);
			}
		}
	}
}]);