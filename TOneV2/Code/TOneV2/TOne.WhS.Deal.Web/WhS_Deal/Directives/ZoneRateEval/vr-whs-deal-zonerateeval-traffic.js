'use strict';

app.directive('vrWhsDealZonerateevalTraffic', ['WhS_Deal_TimePeriodTypeEnum', 'UtilsService', function (WhS_Deal_TimePeriodTypeEnum, UtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var trafficZoneRateEval = new TrafficZoneRateEval($scope, ctrl, $attrs);
			trafficZoneRateEval.initializeController();
		},
		controllerAs: 'trafficCtrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/ZoneRateEval/Templates/TrafficZoneRateEvalTemplate.html'
	};

	function TrafficZoneRateEval($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.timePeriods = UtilsService.getArrayEnum(WhS_Deal_TimePeriodTypeEnum);

			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				if (payload != undefined) {
					$scope.scopeModel.last = payload.Last;
					$scope.scopeModel.selectedTimePeriod = UtilsService.getItemByVal($scope.scopeModel.timePeriods, payload.TimePeriod, 'value');
				}
			};

			api.getData = function () {
				return {
					$type: null,
					Last: $scope.scopeModel.last,
					TimePeriod: $scope.scopeModel.selectedTimePeriod.value
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);