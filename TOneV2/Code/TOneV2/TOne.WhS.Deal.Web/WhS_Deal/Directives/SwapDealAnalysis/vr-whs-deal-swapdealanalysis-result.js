'use strict';

app.directive('vrWhsDealSwapdealanalysisResult', ['WhS_Deal_SwapDealAnalysisTypeEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_Deal_SwapDealAnalysisTypeEnum, UtilsService, VRUIUtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisResult = new SwapDealAnalysisResult($scope, ctrl, $attrs);
			swapDealAnalysisResult.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisResultTemplate.html'
	};

	function SwapDealAnalysisResult($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {
			$scope.scopeModel = {};
			defineAPI();
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var result;

				if (payload != undefined) {
					result = payload.Result;
				}

				if (result != undefined) {
					$scope.scopeModel.dealPeriodInDays = result.DealPeriodInDays;
					$scope.scopeModel.totalCostRevenue = result.TotalCostRevenue;
					$scope.scopeModel.totalSaleRevenue = result.TotalSaleRevenue;
					$scope.scopeModel.totalCostMargin = result.TotalCostMargin;
					$scope.scopeModel.totalSaleMargin = result.TotalSaleMargin;
					$scope.scopeModel.overallProfit = result.OverallProfit;
					$scope.scopeModel.margins = result.Margins;
					$scope.scopeModel.overallRevenue = result.OverallRevenue;
				}
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);