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

			api.load = function (payload)
			{
				if (payload != undefined) {
					
				}
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);