'use strict';

app.directive('vrWhsDealZonerateevalAvg', [function () {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var avgZoneRateEval = new AvgZoneRateEval($scope, ctrl, $attrs);
			avgZoneRateEval.initializeController();
		},
		controllerAs: 'avgCtrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/ZoneRateEval/Templates/AvgZoneRateEvalTemplate.html'
	};

	function AvgZoneRateEval($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};
			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				
			};

			api.getData = function () {
				
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);