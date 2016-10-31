'use strict';

app.directive('vrWhsDealSwapdealanalysisRatecalcmethodOutboundCustomersEditor', [function () {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var customersOutboundRateCalcMethod = new CustomersOutboundRateCalcMethod($scope, ctrl, $attrs);
			customersOutboundRateCalcMethod.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDealAnalysis/Outbound/RateCalcMethod/Templates/CustomersOutboundRateCalcMethodEditorTemplate.html'
	};

	function CustomersOutboundRateCalcMethod($scope, ctrl, $attrs) {

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
					$type: 'TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundRateCustomers, TOne.WhS.Deal.MainExtensions',
					Title: $scope.scopeModel.title
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);