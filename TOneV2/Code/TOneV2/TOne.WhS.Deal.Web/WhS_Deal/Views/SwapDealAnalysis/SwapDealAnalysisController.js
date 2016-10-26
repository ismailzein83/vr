(function (appControllers) {

	'use strict';

	SwapDealAnalysisController.$inject = ['$scope', 'WhS_Deal_DealAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

	function SwapDealAnalysisController($scope, WhS_Deal_DealAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {

		var swapDealAnalysisId;
		var swapDealAnalysisEntity;

		var settingsAPI;
		var settingsReadyDeferred = UtilsService.createPromiseDeferred();
		var settingsLoadDeferred = UtilsService.createPromiseDeferred();

		var outboundManagementAPI;
		var outboundManagementReadyDeferred = UtilsService.createPromiseDeferred();

		var resultAPI;
		var resultReadyDeferred = UtilsService.createPromiseDeferred();

		var isEditMode;

		var swapDealSettings;
		var swapDealSettingsLoadDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				swapDealAnalysisId = parameters.swapDealAnalysisId;
			}

			isEditMode = (swapDealAnalysisId != undefined);
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

			if (isEditMode) {
				getSwapDealAnalysis().then(function () {
					loadAllControls();
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				});
			}
			else {
				loadAllControls();
			}
		}

		function getSwapDealAnalysis() {
			console.log('NotImplementedException');
		}
		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadSwapDealSettings, loadSettings, loadOutboundManagement]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			if (isEditMode) {
				var swapDealAnalysisName;
				if (swapDealAnalysisEntity != undefined)
					swapDealAnalysisName = swapDealAnalysisEntity.Name;
				$scope.title = UtilsService.buildTitleForUpdateEditor(swapDealAnalysisName, 'Swap Deal Analyasis');
			}
			else
				$scope.title = UtilsService.buildTitleForAddEditor('Swap Deal Analysis');
		}
		function loadSwapDealSettings() {
			return WhS_Deal_DealAPIService.GetSwapDealSettingData().then(function (response) {
				swapDealSettings = response;
				swapDealSettingsLoadDeferred.resolve();
			});
		}
		function loadSettings()
		{
			settingsReadyDeferred.promise.then(function () {
				var settingsPayload;
				if (swapDealAnalysisEntity != undefined) {
					settingsPayload = swapDealAnalysisEntity.Settings;
				}
				VRUIUtilsService.callDirectiveLoad(settingsAPI, settingsPayload, settingsLoadDeferred);
			});

			return settingsLoadDeferred.promise;
		}
		function loadOutboundManagement()
		{
			var outboundManagementLoadDeferred = UtilsService.createPromiseDeferred();

			UtilsService.waitMultiplePromises([swapDealSettingsLoadDeferred.promise, settingsLoadDeferred.promise, outboundManagementReadyDeferred.promise]).then(function () {
				var outboundManagementPayload = {
					context: {
						settingsAPI: settingsAPI
					},
					settings: {
						defaultRateCalcMethodId: swapDealSettings.DefaultCalculationMethodId,
						outboundRateCalcMethods: swapDealSettings.OutboundCalculationMethods
					}
				};
				VRUIUtilsService.callDirectiveLoad(outboundManagementAPI, outboundManagementPayload, outboundManagementLoadDeferred);
			});

			return outboundManagementLoadDeferred.promise;
		}
	}

	appControllers.controller('WhS_Deal_SwapDealAnalysisController', SwapDealAnalysisController);

})(appControllers);