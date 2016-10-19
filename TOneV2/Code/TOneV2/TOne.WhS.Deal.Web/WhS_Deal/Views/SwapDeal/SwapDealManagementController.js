(function (appControllers) {

	'use strict';

	DealAnalysisManagementController.$inject = ['$scope', 'WhS_Deal_SwapDealService', 'UtilsService', 'VRNotificationService'];

	function DealAnalysisManagementController($scope, WhS_Deal_SwapDealService, UtilsService, VRNotificationService) {

		var gridAPI;

		defineScope();
		load();

		function defineScope()
		{
			$scope.scopeModel = {};

			$scope.scopeModel.search = function () {

			};

			$scope.scopeModel.analyze = function () {
				var onSwapDealAnalyzed = function () { };
				WhS_Deal_SwapDealService.analyzeSwapDeal(onSwapDealAnalyzed);
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls()
		{
			UtilsService.waitMultipleAsyncOperations([]).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
	}

	appControllers.controller('WhS_Deal_SwapDealManagementController', DealAnalysisManagementController);

})(appControllers);