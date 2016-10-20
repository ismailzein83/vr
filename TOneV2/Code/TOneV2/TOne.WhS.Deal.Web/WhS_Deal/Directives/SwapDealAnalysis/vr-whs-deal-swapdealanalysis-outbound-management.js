'use strict';

app.directive('vrWhsDealSwapdealanalysisOutboundManagement', [function () {
	return {
		restrict: 'E',
		scope: {
			onReady: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisOutboundMangement = new SwapDealAnalysisOutboundMangement($scope, ctrl, $attrs);
			swapDealAnalysisOutboundMangement.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisOutboundManagementTemplate.html'
	};

	function SwapDealAnalysisOutboundMangement($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var gridAPI;

		function initializeController()
		{
			$scope.scopeModel = {};

			$scope.scopeModel.outbounds = [];

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				defineAPI();
			};

			$scope.scopeModel.addOutbound = function () {

			};

			defineMenuActions();
		}

		function defineAPI() {
			var api = {};

			api.load = function () { };

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}

		function defineMenuActions() {
			$scope.scopeModel.menuActions = [{
				name: 'Edit',
				clicked: editOutbound
			}];
		}
		function editOutbound() {
			console.log('editOutbound');
		}
	}
}]);