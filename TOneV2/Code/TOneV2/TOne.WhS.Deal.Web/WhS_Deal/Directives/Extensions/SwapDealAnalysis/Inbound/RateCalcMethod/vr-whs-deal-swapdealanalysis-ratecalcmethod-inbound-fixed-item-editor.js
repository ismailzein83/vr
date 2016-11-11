'use strict';

app.directive('vrWhsDealSwapdealanalysisRatecalcmethodInboundFixedItemeditor', [function () {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var fixedInboundRateCalcMethodItem = new FixedInboundRateCalcMethodItem($scope, ctrl, $attrs);
			fixedInboundRateCalcMethodItem.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDealAnalysis/Inbound/RateCalcMethod/Templates/FixedInboundRateCalcMethodItemEditorTemplate.html'
	};

	function FixedInboundRateCalcMethodItem($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};
			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				if (payload != undefined) {
					$scope.scopeModel.rate = payload.Rate;
				}
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Deal.MainExtensions.SwapDealAnalysisInboundItemRateFixed, TOne.WhS.Deal.MainExtensions',
					Rate: $scope.scopeModel.rate
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);