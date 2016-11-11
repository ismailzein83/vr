(function (appControllers) {

	'use strict';

	SwapDealAnalysisController.$inject = ['$scope', 'WhS_Deal_SwapDealAnalysisAPIService', 'WhS_Deal_SwapDealAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

	function SwapDealAnalysisController($scope, WhS_Deal_SwapDealAnalysisAPIService, WhS_Deal_SwapDealAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {

		var swapDealAnalysisId;
		var swapDealAnalysisEntity;

		var settingsAPI;
		var settingsReadyDeferred = UtilsService.createPromiseDeferred();
		var settingsLoadDeferred = UtilsService.createPromiseDeferred();

		var inboundManagementAPI;
		var inboundManagementReadyDeferred = UtilsService.createPromiseDeferred();

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

			$scope.scopeModel.onInboundManagementReady = function (api) {
				inboundManagementAPI = api;
				inboundManagementReadyDeferred.resolve();
			};

			$scope.scopeModel.onOutboundManagementReady = function (api) {
				outboundManagementAPI = api;
				outboundManagementReadyDeferred.resolve();
			};

			$scope.scopeModel.onResultReady = function (api) {
				resultAPI = api;
				resultReadyDeferred.resolve();
			};

			$scope.scopeModel.validateTabs = function () {
				if (settingsAPI == undefined || settingsAPI.getCarrierAccountId() == undefined) {
					$scope.scopeModel.areTabsVisible = false;
					return 'Select a carrier';
				}
				$scope.scopeModel.areTabsVisible = true;
				return null;
			};

			$scope.scopeModel.analyze = function () {

				$scope.scopeModel.isLoading = true;
				var promises = [];

				var analyzeDealPromise = analyzeDeal();
				promises.push(analyzeDealPromise);

				var loadInboundManagementDeferred = UtilsService.createPromiseDeferred();
				promises.push(loadInboundManagementDeferred.promise);

				var loadOutboundManagementDeferred = UtilsService.createPromiseDeferred();
				promises.push(loadOutboundManagementDeferred.promise);

				var loadResultDeferred = UtilsService.createPromiseDeferred();
				promises.push(loadResultDeferred.promise);

				analyzeDealPromise.then(function (response) {

					if (response == null) {
						loadInboundManagementDeferred.resolve();
						loadOutboundManagementDeferred.resolve();
						return;
					}
					
					var inboundManagementData = {
						Inbounds: response.Inbounds,
						Summary: {
							TotalSaleMargin: response.TotalSaleMargin,
							TotalSaleRevenue: response.TotalSaleRevenue
						}
					};
					loadInboundManagement(inboundManagementData).then(function () {
						loadInboundManagementDeferred.resolve();
					}).catch(function (error) {
						loadInboundManagementDeferred.reject(error, $scope);
					});

					var outboundManagementData = {
						Outbounds: response.Outbounds,
						Summary: {
							TotalCostMargin: response.TotalCostMargin,
							TotalCostRevenue: response.TotalCostRevenue
						}
					};
					loadOutboundManagement(outboundManagementData).then(function () {
						loadOutboundManagementDeferred.resolve();
					}).catch(function (error) {
						loadOutboundManagementDeferred.reject(error, $scope);
					});

					var resultData = {
						DealPeriodInDays: response.DealPeriodInDays,
						TotalCostRevenue: response.TotalCostRevenue,
						TotalSaleRevenue: response.TotalSaleRevenue,
						TotalCostMargin: response.TotalCostMargin,
						TotalSaleMargin: response.TotalSaleMargin,
						OverallProfit: response.OverallProfit,
						Margins: response.Margins,
						OverallRevenue: response.OverallRevenue
					};
					loadResult(resultData).then(function () {
						loadResultDeferred.resolve();
					}).catch(function (error) {
						loadResultDeferred.reject(error);
					});
				});

				UtilsService.waitMultiplePromises(promises).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
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
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadSwapDealSettings, loadSettings, loadInboundManagement, loadOutboundManagement, loadResult]).catch(function (error) {
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
			return WhS_Deal_SwapDealAPIService.GetSwapDealSettingData().then(function (response) {
				swapDealSettings = response;
				swapDealSettingsLoadDeferred.resolve();
			});
		}
		function loadSettings()
		{
			settingsReadyDeferred.promise.then(function () {
				var settingsPayload = {};
				settingsPayload.context = {};
				settingsPayload.context.setSellingNumberPlanId = setSellingNumberPlanId;
				settingsPayload.context.clearAnalysis = clearAnalysis;
				if (swapDealAnalysisEntity != undefined)
					settingsPayload.Settings = swapDealAnalysisEntity.Settings;
				VRUIUtilsService.callDirectiveLoad(settingsAPI, settingsPayload, settingsLoadDeferred);
			});

			return settingsLoadDeferred.promise;
		}
		function loadInboundManagement(data) {
			var inboundManagementLoadDeferred = UtilsService.createPromiseDeferred();

			UtilsService.waitMultiplePromises([swapDealSettingsLoadDeferred.promise, settingsLoadDeferred.promise, inboundManagementReadyDeferred.promise]).then(function () {
				var inboundManagementPayload = {
					context: {
						settingsAPI: settingsAPI,
						clearResult: clearResult
					},
					settings: {
						defaultRateCalcMethodId: swapDealSettings.DefaultInboundRateCalcMethodId,
						inboundRateCalcMethods: swapDealSettings.InboundCalculationMethods
					}
				};
				if (data != undefined) {
					inboundManagementPayload.Inbounds = data.Inbounds;
					inboundManagementPayload.Summary = data.Summary;
				}
				VRUIUtilsService.callDirectiveLoad(inboundManagementAPI, inboundManagementPayload, inboundManagementLoadDeferred);
			});

			return inboundManagementLoadDeferred.promise;
		}
		function loadOutboundManagement(data)
		{
			var outboundManagementLoadDeferred = UtilsService.createPromiseDeferred();

			UtilsService.waitMultiplePromises([swapDealSettingsLoadDeferred.promise, settingsLoadDeferred.promise, outboundManagementReadyDeferred.promise]).then(function () {
				var outboundManagementPayload = {
					context: {
						settingsAPI: settingsAPI,
						clearResult: clearResult
					},
					settings: {
						defaultRateCalcMethodId: swapDealSettings.DefaultCalculationMethodId,
						outboundRateCalcMethods: swapDealSettings.OutboundCalculationMethods
					}
				};
				if (data != undefined) {
					outboundManagementPayload.Outbounds = data.Outbounds;
					outboundManagementPayload.Summary = data.Summary;
				}
				VRUIUtilsService.callDirectiveLoad(outboundManagementAPI, outboundManagementPayload, outboundManagementLoadDeferred);
			});

			return outboundManagementLoadDeferred.promise;
		}
		function loadResult(result) {
			var resultLoadDeferred = UtilsService.createPromiseDeferred();

			resultReadyDeferred.promise.then(function () {
				var resultPayload = {
					Result: result
				};
				VRUIUtilsService.callDirectiveLoad(resultAPI, resultPayload, resultLoadDeferred);
			});

			return resultLoadDeferred.promise;
		}

		function analyzeDeal() {

			var settingsData = settingsAPI.getData();
			var inboundManagementData = inboundManagementAPI.getData();
			var outboundManagementData = outboundManagementAPI.getData();

			var analysisSettings = {};

			if (settingsData != undefined) {
				analysisSettings.CarrierAccountId = settingsData.CarrierAccountId;
				analysisSettings.FromDate = settingsData.FromDate;
				analysisSettings.ToDate = settingsData.ToDate;
			}

			if (inboundManagementData != undefined)
				analysisSettings.Inbounds = inboundManagementData.Inbounds;

			if (outboundManagementData != undefined)
				analysisSettings.Outbounds = outboundManagementData.Outbounds;

			return WhS_Deal_SwapDealAnalysisAPIService.AnalyzeDeal(analysisSettings);
		}
		function setSellingNumberPlanId(sellingNumberPlanId) {
			inboundManagementAPI.setSellingNumberPlanId(sellingNumberPlanId);
		}
		function clearAnalysis() {
			inboundManagementAPI.clear();
			outboundManagementAPI.clear();
			clearResult();
		}
		function clearResult() {
			loadResult({});
		}
	}

	appControllers.controller('WhS_Deal_SwapDealAnalysisController', SwapDealAnalysisController);

})(appControllers);