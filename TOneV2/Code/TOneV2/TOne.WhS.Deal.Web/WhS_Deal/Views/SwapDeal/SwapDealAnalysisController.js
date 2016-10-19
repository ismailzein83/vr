(function (appControllers) {

	'use strict';

	SwapDealAnalysisController.$inject = ['$scope', 'WhS_Deal_SwapDealAnalysisTypeEnum', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

	function SwapDealAnalysisController($scope, WhS_Deal_SwapDealAnalysisTypeEnum, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService)
	{
		loadParameters();
		defineScope();
		load();

		function loadParameters()
		{
			
		}

		function defineScope()
		{
			$scope.scopeModel = {};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}
		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			$scope.title = 'Swap Deal Analysis';
		}
	}

	appControllers.controller('WhS_Deal_SwapDealAnalysisController', SwapDealAnalysisController);

})(appControllers);