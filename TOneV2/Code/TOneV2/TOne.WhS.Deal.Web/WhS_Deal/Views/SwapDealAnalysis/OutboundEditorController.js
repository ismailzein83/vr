(function (appControllers) {

	'use strict';

	OutboundEditorController.$inject = ['$scope', 'WhS_Deal_SwapDealAnalysisAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

	function OutboundEditorController($scope, WhS_Deal_SwapDealAnalysisAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService)
	{
		var isEditMode;

		var supplierId;
		var outboundEntity;

		var countrySelectorAPI;
		var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();
		var countrySelectedDeferred;

		var supplierZoneSelectorAPI;
		var supplierZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
				supplierId = parameters.carrierAccountId;
				outboundEntity = parameters.outboundEntity;
				settings = parameters.settings;
			}

			isEditMode = (outboundEntity != undefined);
		}
		function defineScope()
		{
			$scope.scopeModel = {};

			$scope.scopeModel.rateCalcMethods = [];

			$scope.scopeModel.onCountrySelectorReady = function (api) {
				countrySelectorAPI = api;
				countrySelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onCountrySelectionChanged = function ()
			{
				$scope.scopeModel.selectedSupplierZones.length = 0;
				var countryId = countrySelectorAPI.getSelectedIds();

				if (countryId == undefined)
					return;

				if (countrySelectedDeferred != undefined) {
					countrySelectedDeferred.resolve();
					return;
				}

				// Reload vr-whs-be-supplierzone-selector to reset its filter
				$scope.scopeModel.isLoading = true;

				loadSupplierZoneSelector(countryId).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				}).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};

			$scope.scopeModel.onSupplierZoneSelectorReady = function (api) {
				supplierZoneSelectorAPI = api;
				supplierZoneSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onRateCalcMethodSelectorReady = function (api) {
				rateCalcMethodSelectorAPI = api;
				rateCalcMethodSelectorReadyDeferred.resolve();
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
					OutboundItemRateCalcMethod: directiveAPI.getData(),
					SupplierId: supplierId,
					CountryId: countrySelectorAPI.getSelectedIds(),
					SupplierZoneIds: supplierZoneSelectorAPI.getSelectedIds()
				};

				WhS_Deal_SwapDealAnalysisAPIService.CalculateOutboundRate(input).then(function (response) {
					$scope.scopeModel.calculatedRate = response;
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				}).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};

			$scope.scopeModel.save = function () {
				return (isEditMode) ? updateOutbound() : insertOutbound();
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
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadSupplierZoneSection, loadRateCalcMethodSelector, loadDirective]).then(function () {
				outboundEntity = undefined;
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			if (isEditMode) {
				var outboundEntityName;
				if (outboundEntity != undefined)
					outboundEntityName = outboundEntity.Name;
				$scope.title = UtilsService.buildTitleForUpdateEditor(outboundEntityName, 'Outbound');
			}
			else
				$scope.title = UtilsService.buildTitleForAddEditor('Outbound');
		}
		function loadStaticSection() {
			if (outboundEntity == undefined)
				return;
			$scope.scopeModel.groupName = outboundEntity.Name;
			$scope.scopeModel.volume = outboundEntity.Volume;
			$scope.scopeModel.dealRate = outboundEntity.DealRate;
		}
		function loadSupplierZoneSection()
		{
			var promises = [];

			var loadCountrySelectorPromise = loadCountrySelector();
			promises.push(loadCountrySelectorPromise);

			if (isEditMode) {
				countrySelectedDeferred = UtilsService.createPromiseDeferred();

				var loadSupplierZoneSelectorDeferred = UtilsService.createPromiseDeferred();
				promises.push(loadSupplierZoneSelectorDeferred.promise);

				countrySelectedDeferred.promise.then(function ()
				{
					countrySelectedDeferred = undefined;
					loadSupplierZoneSelectorOnPageLoad().then(function () {
						loadSupplierZoneSelectorDeferred.resolve();
					}).catch(function (error) {
						loadSupplierZoneSelectorDeferred.reject(error);
					});
				});
			}

			return UtilsService.waitMultiplePromises(promises);
		}
		function loadCountrySelector() {
			var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

			countrySelectorReadyDeferred.promise.then(function () {
				var countrySelectorPayload;
				if (outboundEntity != undefined) {
					countrySelectorPayload = { selectedIds: outboundEntity.CountryId };
				}
				VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);
			});

			return countrySelectorLoadDeferred.promise;
		}
		function loadSupplierZoneSelectorOnPageLoad()
		{
			var countryId;
			var selectedIds;

			if (outboundEntity != undefined) {
				countryId = outboundEntity.CountryId;
				selectedIds = outboundEntity.SupplierZoneIds;
			}

			return loadSupplierZoneSelector(countryId, selectedIds);
		}
		function loadSupplierZoneSelector(countryId, selectedIds)
		{
			var supplierZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

			supplierZoneSelectorReadyDeferred.promise.then(function ()
			{
				var supplierZoneSelectorPayload = {
					supplierId: supplierId,
					selectedIds: selectedIds
				};
				supplierZoneSelectorPayload.filter = {
					CountryIds: [countryId]
				};
				VRUIUtilsService.callDirectiveLoad(supplierZoneSelectorAPI, supplierZoneSelectorPayload, supplierZoneSelectorLoadDeferred);
			});

			return supplierZoneSelectorLoadDeferred.promise;
		}
		function loadRateCalcMethodSelector() {
			if (settings == undefined || settings.outboundRateCalcMethods == undefined)
				return;
			for (var key in settings.outboundRateCalcMethods) {
				$scope.scopeModel.rateCalcMethods.push(settings.outboundRateCalcMethods[key]);
			}
			$scope.scopeModel.selectedRateCalcMethod = (outboundEntity != undefined) ?
				UtilsService.getItemByVal($scope.scopeModel.rateCalcMethods, outboundEntity.CalculationMethodId, 'CalculationMethodId') :
				UtilsService.getItemByVal($scope.scopeModel.rateCalcMethods, settings.defaultRateCalcMethodId, 'CalculationMethodId');
		}
		function loadDirective() {

			if (outboundEntity == undefined)
				return;

			var directiveLoadDeferred = UtilsService.createPromiseDeferred();

			directiveReadyDeferred.promise.then(function () {
				directiveReadyDeferred = undefined;
				VRUIUtilsService.callDirectiveLoad(directiveAPI, outboundEntity.ItemCalculationMethod, directiveLoadDeferred);
			});

			return directiveLoadDeferred.promise;
		}

		function insertOutbound() {
			var outboundObj = buildOutboundObjFromScope();
			if ($scope.onOutboundAdded != undefined)
				$scope.onOutboundAdded(outboundObj);
			$scope.modalContext.closeModal();
		}
		function updateOutbound() {
			var outboundObj = buildOutboundObjFromScope();
			if ($scope.onOutboundUpdated != undefined)
				$scope.onOutboundUpdated(outboundObj);
			$scope.modalContext.closeModal();
		}
		function buildOutboundObjFromScope() {
			var obj = {
				Name: $scope.scopeModel.groupName,
				CountryId: countrySelectorAPI.getSelectedIds(),
				SupplierZoneIds: supplierZoneSelectorAPI.getSelectedIds(),
				Volume: $scope.scopeModel.volume,
				DealRate: $scope.scopeModel.dealRate,
				CalculationMethodId: $scope.scopeModel.selectedRateCalcMethod.CalculationMethodId,
				ItemCalculationMethod: directiveAPI.getData()
			};
			return obj;
		}
	}

	appControllers.controller('WhS_Deal_OutboundEditorController', OutboundEditorController);

})(appControllers);