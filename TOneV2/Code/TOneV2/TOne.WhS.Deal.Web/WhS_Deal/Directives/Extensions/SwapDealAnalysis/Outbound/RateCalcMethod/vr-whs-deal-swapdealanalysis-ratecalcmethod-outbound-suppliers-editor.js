'use strict';

app.directive('vrWhsDealSwapdealanalysisRatecalcmethodOutboundSuppliersEditor', [function () {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var suppliersOutboundRateCalcMethod = new SuppliersOutboundRateCalcMethod($scope, ctrl, $attrs);
			suppliersOutboundRateCalcMethod.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDealAnalysis/Outbound/RateCalcMethod/Templates/SuppliersOutboundRateCalcMethodEditorTemplate.html'
	};

	function SuppliersOutboundRateCalcMethod($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};
			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				if (payload != undefined) {
					$scope.scopeModel.title = payload.Title;
				}
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateSuppliers, TOne.WhS.Deal.MainExtensions',
					Title: $scope.scopeModel.title
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);