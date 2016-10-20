(function (appControllers) {

	'use strict';

	SwapDealAnalysisController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

	function SwapDealAnalysisController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {

		var settingsAPI;
		var settingsReadyDeferred = UtilsService.createPromiseDeferred();

		var outboundManagementAPI;
		var outboundManagementReadyDeferred = UtilsService.createPromiseDeferred();

		var resultAPI;
		var resultReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {

		}
		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.onSettingsReady = function (api) {
				settingsAPI = api;
				settingsReadyDeferred.resolve();
			};

			$scope.scopeModel.onOutboundManagementReady = function (api) {
				outboundManagementAPI = api;
				outboundManagementReadyDeferred.resolve();
			};

			$scope.scopeModel.onResultReady = function (api) {
				resultAPI = api;
				resultReadyDeferred.resolve();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}
		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadSettings, loadOutboundManagement]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			$scope.title = 'Swap Deal Analysis Definition';
		}
		function loadSettings()
		{
			var settingsLoadDeferred = UtilsService.createPromiseDeferred();

			settingsReadyDeferred.promise.then(function () {
				var settingsPayload = undefined;
				VRUIUtilsService.callDirectiveLoad(settingsAPI, settingsPayload, settingsLoadDeferred);
			});

			return settingsLoadDeferred.promise;
		}
		function loadOutboundManagement()
		{
			var outboundManagementLoadDeferred = UtilsService.createPromiseDeferred();

			outboundManagementReadyDeferred.promise.then(function () {
				var outboundManagementPayload = undefined;
				VRUIUtilsService.callDirectiveLoad(outboundManagementAPI, outboundManagementPayload, outboundManagementLoadDeferred);
			});

			return outboundManagementLoadDeferred.promise;
		}
	}

	appControllers.controller('WhS_Deal_SwapDealAnalysisController', SwapDealAnalysisController);

})(appControllers);