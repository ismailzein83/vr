'use strict';

app.directive('vrWhsDealSwapdealanalysisRatecalcmethodOutboundFixedItemeditor', [function () {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var fixedOutboundRateCalcMethodItem = new FixedOutboundRateCalcMethodItem($scope, ctrl, $attrs);
			fixedOutboundRateCalcMethodItem.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDealAnalysis/Outbound/RateCalcMethod/Templates/FixedOutboundRateCalcMethodItemEditorTemplate.html'
	};

	function FixedOutboundRateCalcMethodItem($scope, ctrl, $attrs) {

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
					$type: 'TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundItemRateFixed, TOne.WhS.Deal.MainExtensions',
					Rate: $scope.scopeModel.rate
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);