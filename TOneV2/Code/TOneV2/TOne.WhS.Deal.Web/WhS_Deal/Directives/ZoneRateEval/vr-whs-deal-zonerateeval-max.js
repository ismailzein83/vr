'use strict';

app.directive('vrWhsDealZonerateevalMax', [function () {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var maxZoneRateEval = new MaxZoneRateEval($scope, ctrl, $attrs);
			maxZoneRateEval.initializeController();
		},
		controllerAs: 'maxCtrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/ZoneRateEval/Templates/MaxZoneRateEvalTemplate.html'
	};

	function MaxZoneRateEval($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};
			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				if (payload != undefined) {
					$scope.scopeModel.maxRate = payload.MaxRate;
				}
			};

			api.getData = function () {
				return {
					$type: null,
					MaxRate: $scope.scopeModel.maxRate
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);