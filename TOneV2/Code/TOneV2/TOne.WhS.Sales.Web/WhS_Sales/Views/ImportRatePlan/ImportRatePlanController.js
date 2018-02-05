(function (appControllers) {

	'use strict';

	ImportRatePlanController.$inject = ['$scope', 'WhS_Sales_RatePlanAPIService', 'UtilsService', 'VRNavigationService', 'VRNotificationService'];

	function ImportRatePlanController($scope, WhS_Sales_RatePlanAPIService, UtilsService, VRNavigationService, VRNotificationService) {

		var ownerType;
		var ownerId;

		var resultFileId;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters) {
				ownerType = parameters.ownerType;
				ownerId = parameters.ownerId;
			}
		}
		function defineScope() {
			$scope.title = 'Import Rate Plan';
			$scope.scopeModel = {};
			$scope.scopeModel.header = true;

			$scope.scopeModel.upload = function () {
				$scope.scopeModel.isLoading = true;
				var inputRatePlanInput = {
					OwnerType: ownerType,
					OwnerId: ownerId,
					FileId: $scope.scopeModel.file.fileId,
					HeaderRowExists: $scope.scopeModel.header
				};
				return WhS_Sales_RatePlanAPIService.ImportRatePlan(inputRatePlanInput).then(function (response) {
					if (response != undefined) {
						resultFileId = response.FileId;
						if (resultFileId == null) {
							VRNotificationService.showSuccess("Rate plan has been successfully imported");
							if ($scope.onRatePlanImported != undefined)
								$scope.onRatePlanImported();
							$scope.modalContext.closeModal();
						}
						else {
							$scope.scopeModel.showResult = true;
						}
					}
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				}).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};
			$scope.scopeModel.download = function () {
				return WhS_Sales_RatePlanAPIService.DownloadImportRatePlanTemplate().then(function (response) {
					if (response != undefined) {
						UtilsService.downloadFile(response.Content, response.Headers);
					}
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				});
			};
			$scope.scopeModel.downloadResult = function () {
				if (resultFileId != undefined) {
					return WhS_Sales_RatePlanAPIService.DownloadImportRatePlanResult(resultFileId).then(function (response) {
						if (response != undefined) {
							UtilsService.downloadFile(response.data, response.headers);
						}
					});
				}
			};
		}
		function load() {

		}
	}

	appControllers.controller('WhS_Sales_ImportRatePlanController', ImportRatePlanController);

})(appControllers);