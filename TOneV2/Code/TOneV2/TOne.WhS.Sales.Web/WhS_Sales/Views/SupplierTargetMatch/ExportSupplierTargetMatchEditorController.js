(function (appControllers) {

	"use strict";

	exportSupplierTargetMatchEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Sales_SupplierTargetMatchAPIService'];

	function exportSupplierTargetMatchEditorController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
		var gridAPI;
		defineScope();

		function defineScope() {
			$scope.scopeModel = {};
			$scope.title = "Supplier Target Match Export";
			$scope.scopeModel.defaultVolume = 200;
			$scope.scopeModel.defaultASR = 20;
			$scope.scopeModel.defaultACD = 2;
			$scope.scopeModel.exportSupplierTargetMatch = function () {
				var exportSettings = {
					DefaultVolume: $scope.scopeModel.defaultVolume,
					DefaultASR: $scope.scopeModel.defaultASR,
					DefaultACD: $scope.scopeModel.defaultACD,
					IncludeACD_ASR: $scope.scopeModel.withACD_ASR
				};
				if ($scope.onExportSupplierTargetMatch != undefined) {
					$scope.scopeModel.isLoading = true;
					$scope.onExportSupplierTargetMatch(exportSettings).then().finally(function () {
						$scope.scopeModel.isLoading = false;
						$scope.modalContext.closeModal();
					});
				}
			};
		}
	}

	appControllers.controller('WhS_Sales_ExportSupplierTargetMatchEditorController', exportSupplierTargetMatchEditorController);
})(appControllers);