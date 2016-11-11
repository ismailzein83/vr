(function (appControllers) {

	'use strict';

	InboundEditorController.$inject = ['$scope', 'WhS_Deal_SwapDealAnalysisAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

	function InboundEditorController($scope, WhS_Deal_SwapDealAnalysisAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {
		var isEditMode;

		var customerId;
		var sellingNumberPlanId;
		var inboundEntity;

		var countrySelectorAPI;
		var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();
		var countrySelectedDeferred;

		var saleZoneSelectorAPI;
		var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var rateCalcMethodSelectorAPI;
		var rateCalcMethodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var directiveAPI;
		var directiveReadyDeferred;

		var settings;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				customerId = parameters.carrierAccountId;
				sellingNumberPlanId = parameters.sellingNumberPlanId;
				inboundEntity = parameters.inboundEntity;
				settings = parameters.settings;
			}

			isEditMode = (inboundEntity != undefined);
		}
		function defineScope() {

			$scope.scopeModel = {};

			$scope.scopeModel.rateCalcMethods = [];

			$scope.scopeModel.onCountrySelectorReady = function (api) {
				countrySelectorAPI = api;
				countrySelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onCountrySelectionChanged = function () {

				$scope.scopeModel.selectedSaleZones.length = 0;

				var countryId = countrySelectorAPI.getSelectedIds();

				if (countryId == undefined)
					return;

				if (countrySelectedDeferred != undefined) {
					countrySelectedDeferred.resolve();
					return;
				}

				// Reload vr-whs-be-salezone-selector to reset its filter
				$scope.scopeModel.isLoading = true;

				loadSaleZoneSelector(countryId).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				}).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};

			$scope.scopeModel.onSaleZoneSelectorReady = function (api) {
				saleZoneSelectorAPI = api;
				saleZoneSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onRateCalcMethodSelectorReady = function (api) {
				rateCalcMethodSelectorAPI = api;
				rateCalcMethodSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onRateCalcMethodSelectionChanged = function () {
				$scope.scopeModel.calculatedRate = undefined;
			};

			$scope.scopeModel.onDirectiveReady = function (api) {
				directiveAPI = api;
				var setLoader = function (value) {
					$scope.scopeModel.isLoading = value;
				};
				VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
			};

			$scope.scopeModel.calculateRate = function () {

				$scope.scopeModel.isLoading = true;

				var input = {
					InboundItemRateCalcMethod: directiveAPI.getData(),
					CustomerId: customerId,
					CountryId: countrySelectorAPI.getSelectedIds(),
					SaleZoneIds: saleZoneSelectorAPI.getSelectedIds()
				};

				WhS_Deal_SwapDealAnalysisAPIService.CalculateInboundRate(input).then(function (response) {
					$scope.scopeModel.calculatedRate = response;
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				}).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};

			$scope.scopeModel.save = function () {
				return (isEditMode) ? updateInbound() : insertInbound();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}
		function load() {
			$scope.scopeModel.isLoading = true;

			if (isEditMode) {
				directiveReadyDeferred = UtilsService.createPromiseDeferred();
			}

			loadAllControls();
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadSaleZoneSection, loadRateCalcMethodSelector, loadDirective]).then(function () {
				inboundEntity = undefined;
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			if (isEditMode) {
				var inboundEntityName;
				if (inboundEntity != undefined)
					inboundEntityName = inboundEntity.Name;
				$scope.title = UtilsService.buildTitleForUpdateEditor(inboundEntityName, 'Inbound');
			}
			else
				$scope.title = UtilsService.buildTitleForAddEditor('Inbound');
		}
		function loadStaticSection() {
			if (inboundEntity == undefined)
				return;
			$scope.scopeModel.groupName = inboundEntity.Name;
			$scope.scopeModel.volume = inboundEntity.Volume;
			$scope.scopeModel.dealRate = inboundEntity.DealRate;
		}
		function loadSaleZoneSection() {
			var promises = [];

			var loadCountrySelectorPromise = loadCountrySelector();
			promises.push(loadCountrySelectorPromise);

			if (isEditMode) {
				countrySelectedDeferred = UtilsService.createPromiseDeferred();

				var loadSaleZoneSelectorDeferred = UtilsService.createPromiseDeferred();
				promises.push(loadSaleZoneSelectorDeferred.promise);

				countrySelectedDeferred.promise.then(function () {
					countrySelectedDeferred = undefined;
					loadSaleZoneSelectorOnPageLoad().then(function () {
						loadSaleZoneSelectorDeferred.resolve();
					}).catch(function (error) {
						loadSaleZoneSelectorDeferred.reject(error);
					});
				});
			}

			return UtilsService.waitMultiplePromises(promises);
		}
		function loadCountrySelector() {
			var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

			countrySelectorReadyDeferred.promise.then(function () {
				var countrySelectorPayload;
				if (inboundEntity != undefined) {
					countrySelectorPayload = { selectedIds: inboundEntity.CountryId };
				}
				VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);
			});

			return countrySelectorLoadDeferred.promise;
		}
		function loadSaleZoneSelectorOnPageLoad() {
			var countryId;
			var selectedIds;

			if (inboundEntity != undefined) {
				countryId = inboundEntity.CountryId;
				selectedIds = inboundEntity.SaleZoneIds;
			}

			return loadSaleZoneSelector(countryId, selectedIds);
		}
		function loadSaleZoneSelector(countryId, selectedIds) {
			var saleZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

			saleZoneSelectorReadyDeferred.promise.then(function () {
				var saleZoneSelectorPayload = {
					customerId: customerId,
					sellingNumberPlanId: sellingNumberPlanId,
					selectedIds: selectedIds
				};
				saleZoneSelectorPayload.filter = {
					CountryIds: [countryId]
				};
				VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, saleZoneSelectorLoadDeferred);
			});

			return saleZoneSelectorLoadDeferred.promise;
		}
		function loadRateCalcMethodSelector() {
			if (settings == undefined || settings.inboundRateCalcMethods == undefined)
				return;
			for (var key in settings.inboundRateCalcMethods) {
				$scope.scopeModel.rateCalcMethods.push(settings.inboundRateCalcMethods[key]);
			}
			$scope.scopeModel.selectedRateCalcMethod = (inboundEntity != undefined) ?
				UtilsService.getItemByVal($scope.scopeModel.rateCalcMethods, inboundEntity.CalculationMethodId, 'CalculationMethodId') :
				UtilsService.getItemByVal($scope.scopeModel.rateCalcMethods, settings.defaultRateCalcMethodId, 'CalculationMethodId');
		}
		function loadDirective() {

			if (inboundEntity == undefined)
				return;

			var directiveLoadDeferred = UtilsService.createPromiseDeferred();

			directiveReadyDeferred.promise.then(function () {
				directiveReadyDeferred = undefined;
				VRUIUtilsService.callDirectiveLoad(directiveAPI, inboundEntity.ItemCalculationMethod, directiveLoadDeferred);
			});

			return directiveLoadDeferred.promise;
		}

		function insertInbound() {
			var inboundObj = buildInboundObjFromScope();
			if ($scope.onInboundAdded != undefined)
				$scope.onInboundAdded(inboundObj);
			$scope.modalContext.closeModal();
		}
		function updateInbound() {
			var inboundObj = buildInboundObjFromScope();
			if ($scope.onInboundUpdated != undefined)
				$scope.onInboundUpdated(inboundObj);
			$scope.modalContext.closeModal();
		}
		function buildInboundObjFromScope() {
			var obj = {
				Name: $scope.scopeModel.groupName,
				CountryId: countrySelectorAPI.getSelectedIds(),
				SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
				Volume: $scope.scopeModel.volume,
				DealRate: $scope.scopeModel.dealRate,
				CalculationMethodId: $scope.scopeModel.selectedRateCalcMethod.CalculationMethodId,
				ItemCalculationMethod: directiveAPI.getData()
			};
			return obj;
		}
	}

	appControllers.controller('WhS_Deal_InboundEditorController', InboundEditorController);

})(appControllers);