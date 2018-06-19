(function (appControllers) {
	"use strict";
	receivedPricelistErrorDetailsController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'WhS_SupPL_PreviewChangeTypeEnum', 'WhS_SupPL_PreviewGroupedBy', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];
	function receivedPricelistErrorDetailsController($scope, BusinessProcess_BPTaskAPIService, WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

		defineScope();
		loadParameters();
		load();

		function defineScope() {
			setTitle();
			$scope.scopeModal = {};
			$scope.scopeModal.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters !== undefined && parameters !== null) {
				$scope.scopeModal.errorDetails = parameters.ErrorDetails;
			}
		}

		function load() {
		}

		function setTitle() {
			$scope.title = 'Received Pricelist Errors';
		}

	}
	appControllers.controller('WhS_SupPL_ReceivedPricelistErrorDetailsController', receivedPricelistErrorDetailsController);
})(appControllers);