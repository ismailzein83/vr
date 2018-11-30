"use strict";
SupplierRatesManagementController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRUIUtilsService", "VRDateTimeService","CP_WhS_TOneAccountService"];

function SupplierRatesManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, VRDateTimeService, CP_WhS_TOneAccountService) {
	var carrierAccountSelectorAPI;
	var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

	var supplierRateGridAPI;
	var supplierRateReadyPromiseDeffered = UtilsService.createPromiseDeferred();

	var countryDirectiveApi;
	var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

	defineScope();
	load();

	function defineScope() {
		$scope.scopeModel = {};

		$scope.scopeModel.effectiveOn = VRDateTimeService.getNowDateTime();

		$scope.scopeModel.onCarrierAccountsSelectorReady = function (api) {
			carrierAccountSelectorAPI = api;
			carrierAccountSelectorReadyPromiseDeferred.resolve();
		};

		$scope.scopeModel.onSupplierRateGridReady = function (api) {
			supplierRateGridAPI = api;
			supplierRateReadyPromiseDeffered.resolve();
		};

		$scope.scopeModel.searchClicked = function () {
			var filter = getFilter();
			return supplierRateGridAPI.load(filter);
		};

		$scope.scopeModel.onCountryReady = function (api) {
			countryDirectiveApi = api;
			countryReadyPromiseDeferred.resolve();
		};
	}

	function load() {
		$scope.scopeModel.isLoading = true;
		loadAllControlls();
	}

	function loadAllControlls() {
		return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector, loadCountrySelector])
			.catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			})
			.finally(function () {
				$scope.scopeModel.isLoading = false;
			});

	}

	function loadCarrierAccountSelector() {
		var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
		carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {

			var payload = {
			};
			VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, loadCarrierAccountPromiseDeferred);
		});
		return loadCarrierAccountPromiseDeferred.promise;
	}
		
	function loadCountrySelector() {
		var countryLoadPromiseDeferred = UtilsService.createPromiseDeferred();
		countryReadyPromiseDeferred.promise
			.then(function () {
				var directivePayload = { connectionId: CP_WhS_TOneAccountService.getTOneConnectionId()};
				VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
			});
		return countryLoadPromiseDeferred.promise;
	}

	function getFilter() {
		var objectFromScope = {
			EffectiveOn: $scope.scopeModel.effectiveOn,
			SupplierId: carrierAccountSelectorAPI.getSelectedIds(),
			SupplierZoneName: $scope.scopeModel.supplierZoneName,
			CountriesIds: countryDirectiveApi.getSelectedIds(),
			IsHistory: false,
			IsChild: false
		};
		return objectFromScope;
	}

}

appControllers.controller("CP_WhS_SupplierRatesManagementController", SupplierRatesManagementController);