(function (appControllers) {

	"use strict";

	CustomerRateManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_WhS_CustomerPriceListOwnerTypeEnum', 'VRDateTimeService', 'CP_WhS_SalePriceListOwnerTypeEnum', 'CP_WhS_PrimarySaleEntityEnum','CP_WhS_TOneAccountService'];

	function CustomerRateManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, CP_WhS_CustomerPriceListOwnerTypeEnum, VRDateTimeService, CP_WhS_SalePriceListOwnerTypeEnum, CP_WhS_PrimarySaleEntityEnum, CP_WhS_TOneAccountService) {

		var gridAPI;

		var carrierAccountSelectorAPI;
		var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var countryDirectiveApi;
		var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var gridPayload = {};
		var customerCurrencyId;
		defineScope();
		load();


		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.effectiveOn = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

			$scope.scopeModel.searchClicked = function () {
				setGridPayload();
				return gridAPI.load(gridPayload);
			};

			$scope.ownerTypes = UtilsService.getArrayEnum(CP_WhS_CustomerPriceListOwnerTypeEnum);

			$scope.selectedOwnerType = $scope.ownerTypes[0];

			$scope.scopeModel.onCarrierAccountsSelectorReady = function (api) {
				carrierAccountSelectorAPI = api;
				carrierAccountSelectorReadyPromiseDeferred.resolve();
			};

			$scope.scopeModel.onCustomerRateGridReady = function (api) {
				gridAPI = api;
			};

			$scope.scopeModel.onCustomerSelectionChanged = function (option) {
				if (option != undefined)
					customerCurrencyId = option.CurrencyId;
			};

			$scope.scopeModel.onCountryReady = function (api) {
				countryDirectiveApi = api;
				countryReadyPromiseDeferred.resolve()
			};

		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {
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
					var directivePayload = { connectionId: CP_WhS_TOneAccountService.getTOneConnectionId() };
					VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
				});
			return countryLoadPromiseDeferred.promise;
		}


		function setGridPayload() {
			gridPayload = {
				query: {
					EffectiveOn: $scope.scopeModel.effectiveOn,
					OwnerType: CP_WhS_SalePriceListOwnerTypeEnum.Customer.value,
					OwnerId: carrierAccountSelectorAPI.getSelectedIds(),
					CurrencyId: customerCurrencyId,
					SaleZoneName: $scope.scopeModel.saleZoneName,
					CountriesIds: countryDirectiveApi.getSelectedIds()
				},
				primarySaleEntity: CP_WhS_PrimarySaleEntityEnum.Customer.value
			};
		}
	}

	appControllers.controller('CP_WhS_CustomerRateManagementController', CustomerRateManagementController);
})(appControllers);