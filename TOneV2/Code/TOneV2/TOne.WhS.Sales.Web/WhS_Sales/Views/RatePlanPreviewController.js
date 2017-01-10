﻿(function (appControllers) {

	'use strict';

	RatePlanPreviewController.$inject = ['$scope', 'WhS_Sales_RatePlanPreviewAPIService', 'WhS_BE_SalePriceListOwnerTypeEnum', 'BusinessProcess_BPTaskAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

	function RatePlanPreviewController($scope, WhS_Sales_RatePlanPreviewAPIService, WhS_BE_SalePriceListOwnerTypeEnum, BusinessProcess_BPTaskAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

		var taskId;
		var processInstanceId;
		var ownerType;

		var ratePreviewGridAPI;
		var ratePreviewGridReadyDeferred = UtilsService.createPromiseDeferred();

		var saleZoneRoutingProductPreviewGridAPI;
		var saleZoneRoutingProductGridReadyDeferred = UtilsService.createPromiseDeferred();

		var currentDefaultServiceViewerAPI;
		var currentDefaultServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

		var newDefaultServiceViewerAPI;
		var newDefaultServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

		var summaryServiceViewerAPI;
		var summaryServiceViewerReadyDeferred = UtilsService.createPromiseDeferred();

		var newCountryGridAPI;
		var newCountryGridReadyDeferred = UtilsService.createPromiseDeferred();

		var changedCountryGridAPI;
		var changedCountryGridReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				taskId = parameters.TaskId;
			}
		}
		function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.defaultServicePreview = {};

			$scope.scopeModel.onRatePreviewGridReady = function (api) {
				ratePreviewGridAPI = api;
				ratePreviewGridReadyDeferred.resolve();
			};

			$scope.scopeModel.onSaleZoneRoutingProductPreviewGridReady = function (api) {
				saleZoneRoutingProductPreviewGridAPI = api;
				saleZoneRoutingProductGridReadyDeferred.resolve();
			};

			$scope.scopeModel.onCurrentDefaultServiceViewerReady = function (api) {
				currentDefaultServiceViewerAPI = api;
				currentDefaultServiceViewerReadyDeferred.resolve();
			};

			$scope.scopeModel.onNewDefaultServiceViewerReady = function (api) {
				newDefaultServiceViewerAPI = api;
				newDefaultServiceViewerReadyDeferred.resolve();
			};

			$scope.scopeModel.onSummaryServiceViewerReady = function (api) {
				summaryServiceViewerAPI = api;
				summaryServiceViewerReadyDeferred.resolve();
			};

			$scope.scopeModel.onNewCountryPreviewGridReady = function (api) {
				newCountryGridAPI = api;
				newCountryGridReadyDeferred.resolve();
			};

			$scope.scopeModel.onChangedCountryPreviewGridReady = function (api) {
				changedCountryGridAPI = api;
				changedCountryGridReadyDeferred.resolve();
			};

			$scope.scopeModel.save = function () {
				return executeTask(true);
			};
			$scope.scopeModel.close = function () {
				return executeTask(false);
			};
		}
		function load() {
			$scope.scopeModel.isLoading = true;

			getTaskData().then(function () {
				loadAllControls();
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
				$scope.scopeModel.isLoading = false;
			});
		}

		function getTaskData() {
			return BusinessProcess_BPTaskAPIService.GetTask(taskId).then(function (response) {
				if (response == null)
					return;
				processInstanceId = response.ProcessInstanceId;
				if (response.TaskData == null)
					return;
				ownerType = response.TaskData.OwnerType;
				if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
					$scope.scopeModel.showCountrySummary = true;
					$scope.scopeModel.showCountryTab = true;
				}
			});
		}
		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([loadSummary, loadDefaultRoutingProductPreview, loadDefaultServicePreview, loadRatePreviewGrid, loadSaleZoneRoutingProductPreviewGrid, loadNewCountryPreviewGrid, loadChangedCountryPreviewGrid]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function loadSummary() {

			var promises = [];

			var query = { ProcessInstanceId: processInstanceId };
			var getRatePlanPreviewSummaryPromise = WhS_Sales_RatePlanPreviewAPIService.GetRatePlanPreviewSummary(query);
			promises.push(getRatePlanPreviewSummaryPromise);

			var summaryServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();
			promises.push(summaryServiceViewerLoadDeferred.promise);

			getRatePlanPreviewSummaryPromise.then(function (response) {
				if (response == undefined || response == null)
					summaryServiceViewerLoadDeferred.resolve();
				else {
					$scope.scopeModel.numberOfNewRates = response.NumberOfNewRates;
					$scope.scopeModel.numberOfIncreasedRates = response.NumberOfIncreasedRates;
					$scope.scopeModel.numberOfDecreasedRates = response.NumberOfDecreasedRates;
					$scope.scopeModel.numberOfClosedRates = response.NumberOfClosedRates;

					$scope.scopeModel.nameOfNewDefaultRoutingProduct = response.NameOfNewDefaultRoutingProduct;
					$scope.scopeModel.nameOfClosedDefaultRoutingProudct = response.NameOfClosedDefaultRoutingProudct;

					$scope.scopeModel.numberOfNewSaleZoneRoutingProducts = response.NumberOfNewSaleZoneRoutingProducts;
					$scope.scopeModel.numberOfClosedSaleZoneRoutingProducts = response.NumberOfClosedSaleZoneRoutingProducts;

					$scope.scopeModel.numberOfNewSaleZoneServices = response.NumberOfNewSaleZoneServices;
					$scope.scopeModel.numberOfClosedSaleZoneServices = response.NumberOfClosedSaleZoneServices;

					$scope.scopeModel.numberOfChangedCountries = response.NumberOfChangedCountries;

					if (response.NewDefaultServices == null)
						summaryServiceViewerLoadDeferred.resolve();
					else {
						$scope.scopeModel.showSummaryServiceViewer = true;
						summaryServiceViewerReadyDeferred.promise.then(function () {
							var summaryServiceViewerPayload = {
								selectedIds: UtilsService.getPropValuesFromArray(response.NewDefaultServices, 'ServiceId')
							};
							VRUIUtilsService.callDirectiveLoad(summaryServiceViewerAPI, summaryServiceViewerPayload, summaryServiceViewerLoadDeferred);
						});
					}

					if (response.ClosedDefaultServiceEffectiveOn != null)
						$scope.scopeModel.closedDefaultServiceEffectiveOn = UtilsService.getShortDate(new Date(response.ClosedDefaultServiceEffectiveOn));
				}
			});

			return UtilsService.waitMultiplePromises(promises);
		}
		function loadDefaultRoutingProductPreview() {
			var input = { ProcessInstanceId: processInstanceId };
			return WhS_Sales_RatePlanPreviewAPIService.GetDefaultRoutingProductPreview(input).then(function (response) {
				if (response != null) {
					$scope.scopeModel.currentDefaultRoutingProductName = response.CurrentDefaultRoutingProductName;
					if (response.IsCurrentDefaultRoutingProductInherited === true)
						$scope.scopeModel.currentDefaultRoutingProductName += ' (Inherited)';
					$scope.scopeModel.newDefaultRoutingProductName = response.NewDefaultRoutingProductName;
					$scope.scopeModel.effectiveOn = UtilsService.getShortDate(new Date(response.EffectiveOn));
				}
			});
		}
		function loadDefaultServicePreview() {
			var promises = [];

			var input = { ProcessInstanceId: processInstanceId };
			var getDefaultServicePreviewPromise = WhS_Sales_RatePlanPreviewAPIService.GetDefaultServicePreview(input);
			promises.push(getDefaultServicePreviewPromise);

			var loadDefaultServiceViewersDeferred = UtilsService.createPromiseDeferred();
			promises.push(loadDefaultServiceViewersDeferred.promise);

			getDefaultServicePreviewPromise.then(function (response) {
				var serviceViewerPromises = [];

				if (response != null) {
					$scope.scopeModel.defaultServicePreview.effectiveOn = UtilsService.getShortDate(new Date(response.EffectiveOn));
					if (response.EffectiveUntil != null)
						$scope.scopeModel.defaultServicePreview.effectiveUntil = UtilsService.getShortDate(new Date(response.EffectiveUntil));

					if (response.CurrentServices != null) {
						$scope.scopeModel.showCurrentDefaultServiceViewer = true;

						var currentDefaultServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();
						serviceViewerPromises.push(currentDefaultServiceViewerLoadDeferred.promise);

						currentDefaultServiceViewerReadyDeferred.promise.then(function () {
							var currentDefaultServiceViewerPayload = {
								selectedIds: UtilsService.getPropValuesFromArray(response.CurrentServices, 'ServiceId')
							};
							VRUIUtilsService.callDirectiveLoad(currentDefaultServiceViewerAPI, currentDefaultServiceViewerPayload, currentDefaultServiceViewerLoadDeferred);
						});
					}

					if (response.NewServices != null) {
						$scope.scopeModel.showNewDefaultServiceViewer = true;

						var newDefaultServiceViewerLoadDeferred = UtilsService.createPromiseDeferred();
						serviceViewerPromises.push(newDefaultServiceViewerLoadDeferred.promise);

						newDefaultServiceViewerReadyDeferred.promise.then(function () {
							var newDefaultServiceViewerPayload = {
								selectedIds: UtilsService.getPropValuesFromArray(response.NewServices, 'ServiceId')
							};
							VRUIUtilsService.callDirectiveLoad(newDefaultServiceViewerAPI, newDefaultServiceViewerPayload, newDefaultServiceViewerLoadDeferred);
						});
					}
				}

				UtilsService.waitMultiplePromises(serviceViewerPromises).then(function () {
					loadDefaultServiceViewersDeferred.resolve();
				}).catch(function (error) {
					loadDefaultServiceViewersDeferred.reject(error);
				});
			});

			return UtilsService.waitMultiplePromises(promises);
		}
		function loadRatePreviewGrid() {
			var ratePreviewGridLoadDeferred = UtilsService.createPromiseDeferred();

			ratePreviewGridReadyDeferred.promise.then(function () {
				var ratePreviewGridPayload = {
					ProcessInstanceId: processInstanceId,
					ZoneName: null
				};
				VRUIUtilsService.callDirectiveLoad(ratePreviewGridAPI, ratePreviewGridPayload, ratePreviewGridLoadDeferred);
			});

			return ratePreviewGridLoadDeferred.promise;
		}
		function loadSaleZoneRoutingProductPreviewGrid() {
			var saleZoneRoutingProductGridLoadDeferred = UtilsService.createPromiseDeferred();

			saleZoneRoutingProductGridReadyDeferred.promise.then(function () {
				var saleZoneRoutingProductPreviewGridPayload = {
					ProcessInstanceId: processInstanceId
				};
				VRUIUtilsService.callDirectiveLoad(saleZoneRoutingProductPreviewGridAPI, saleZoneRoutingProductPreviewGridPayload, saleZoneRoutingProductGridLoadDeferred);
			});

			return saleZoneRoutingProductGridLoadDeferred.promise;
		}
		function loadNewCountryPreviewGrid() {
			var newCountryGridLoadDeferred = UtilsService.createPromiseDeferred();
			newCountryGridReadyDeferred.promise.then(function () {
				var newCountryGridPayload = {
					ProcessInstanceId: processInstanceId
				};
				VRUIUtilsService.callDirectiveLoad(newCountryGridAPI, newCountryGridPayload, newCountryGridLoadDeferred);
			});
			return newCountryGridLoadDeferred.promise;
		}
		function loadChangedCountryPreviewGrid() {
			var changedCountryGridLoadDeferred = UtilsService.createPromiseDeferred();
			changedCountryGridReadyDeferred.promise.then(function () {
				var changedCountryGridPayload = {
					ProcessInstanceId: processInstanceId
				};
				VRUIUtilsService.callDirectiveLoad(changedCountryGridAPI, changedCountryGridPayload, changedCountryGridLoadDeferred);
			});
			return changedCountryGridLoadDeferred.promise;
		}

		function executeTask(decision) {
			$scope.scopeModel.isLoading = true;

			var executionInformation = {
				$type: "TOne.WhS.Sales.BP.Arguments.Tasks.PreviewTaskExecutionInformation, TOne.WhS.Sales.BP.Arguments",
				Decision: decision
			};

			var input = {
				$type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
				TaskId: taskId,
				ExecutionInformation: executionInformation
			};

			return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
				$scope.modalContext.closeModal();
			}).catch(function (error) {
				VRNotificationService.notifyException(error);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
	}

	appControllers.controller('WhS_Sales_RatePlanPreviewController', RatePlanPreviewController);

})(appControllers);