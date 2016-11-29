(function (appControllers) {

	"use strict";

	SellNewCountriesController.$inject = ["$scope", 'WhS_Sales_RatePlanUtilsService', "UtilsService", 'VRUIUtilsService', 'VRValidationService', "VRNavigationService", "VRNotificationService"];

	function SellNewCountriesController($scope, WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService, VRValidationService, VRNavigationService, VRNotificationService) {

		var customerId;
		var countryChanges;
		var saleAreaSettings;

		var effectiveDateDayOffset = 0;
		var retroactiveDayOffset = 0;
		var retroactiveDate = UtilsService.getDateFromDateTime(new Date());

		var countrySelectorAPI;
		var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var newCountryGridAPI;
		var newCountryGridReadyDeferred = UtilsService.createPromiseDeferred();

		var soldCountryGridAPI;
		var soldCountryGridReadyDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters != undefined) {
				customerId = parameters.customerId;
				countryChanges = parameters.countryChanges;
				saleAreaSettings = parameters.saleAreaSettings;
			}
			if (saleAreaSettings != undefined) {
				// Make sure that the offsets are valid numbers
				var effectiveDateDayOffsetValue = Number(saleAreaSettings.EffectiveDateDayOffset);
				if (!isNaN(effectiveDateDayOffsetValue))
					effectiveDateDayOffset = effectiveDateDayOffsetValue;
				var retroactiveDayOffsetValue = Number(saleAreaSettings.RetroactiveDayOffset);
				if (!isNaN(retroactiveDayOffsetValue))
					retroactiveDayOffset = retroactiveDayOffsetValue;
			}
			if (retroactiveDayOffset > 0) {
				var retroactiveDateValue = WhS_Sales_RatePlanUtilsService.getNowMinusDays(retroactiveDayOffset);
				retroactiveDate = UtilsService.getDateFromDateTime(retroactiveDateValue);
			}
		}
		function defineScope() {

			$scope.scopeModel = {};
			$scope.scopeModel.newCountries = [];

			$scope.scopeModel.onCountrySelectorReady = function (api) {
				countrySelectorAPI = api;
				countrySelectorReadyDeferred.resolve();
			};
			$scope.scopeModel.onCountrySelected = function (country) {
				addCountry(country);
			};
			$scope.scopeModel.onCountryDeselected = function (country) {
				removeCountry(country.CountryId);
			};

			$scope.scopeModel.onNewCountryGridReady = function (api) {
				newCountryGridAPI = api;
				newCountryGridReadyDeferred.resolve();
			};
			$scope.scopeModel.onGridRowDeleted = function (dataItem) {
				removeCountry(dataItem.Entity.CountryId);
			};

			$scope.scopeModel.onSoldCountryGridReady = function (api) {
				soldCountryGridAPI = api;
				soldCountryGridReadyDeferred.resolve();
			};

			$scope.scopeModel.isNewCountryBEDValid = function (dataItem) {
				if (dataItem.Entity.BED == null)
					return 'BED is a required field';
				if (dataItem.Entity.BED < retroactiveDate)
					return 'Retroactive Date: ' + UtilsService.getShortDate(retroactiveDate);
				return null;
			};

			$scope.scopeModel.save = function () {
				if ($scope.onCountryChangesUpdated != undefined) {
					var updatedCountryChanges = buildCountryChanges();
					$scope.onCountryChangesUpdated(updatedCountryChanges);
				}
				$scope.modalContext.closeModal();
			};
			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}
		function load() {
			$scope.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadCountrySelector, loadNewCountryGrid, loadSoldCountryGrid]).then(function () {
				countryChanges = undefined;
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.isLoading = false;
			});
		}
		function setTitle() {
			$scope.title = 'Manage Selling Countries';
		}
		function loadCountrySelector() {
			var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

			countrySelectorReadyDeferred.promise.then(function () {
				var countrySelectorPayload = {
					filter: getCountrySelectorFilter()
				};
				if (countryChanges != undefined && countryChanges.NewCountries != null) {
					countrySelectorPayload.selectedIds = UtilsService.getPropValuesFromArray(countryChanges.NewCountries, 'CountryId');
				}
				VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);
			});

			function getCountrySelectorFilter() {
				var filter = {};

				filter.Filters = [];
				var notSoldToCustomerFilter = {
					$type: 'TOne.WhS.BusinessEntity.Business.CountryNotSoldToCustomerFilter, TOne.WhS.BusinessEntity.Business',
					CustomerId: customerId,
					EffectiveOn: UtilsService.getDateFromDateTime(new Date())
				};
				filter.Filters.push(notSoldToCustomerFilter);

				return filter;
			}

			return countrySelectorLoadDeferred.promise;
		}
		function loadNewCountryGrid() {
			var newCountryGridLoadDeferred = UtilsService.createPromiseDeferred();

			if (countryChanges != undefined && countryChanges.NewCountries != null) {
				for (var i = 0; i < countryChanges.NewCountries.length; i++) {
					var newCountry = countryChanges.NewCountries[i];
					var dataItem = {
						Entity: {
							CountryId: newCountry.CountryId,
							Name: newCountry.Name,
							BED: newCountry.BED,
							EED: newCountry.EED
						}
					};
					$scope.scopeModel.newCountries.push(dataItem);
				}
			}

			newCountryGridLoadDeferred.resolve();
			return newCountryGridLoadDeferred.promise;
		}
		function loadSoldCountryGrid() {
			var soldCountryGridLoadDeferred = UtilsService.createPromiseDeferred();
			soldCountryGridReadyDeferred.promise.then(function () {
				var soldCountryGridPayload = {
					query: {
						CustomerId: customerId,
						EffectiveOn: UtilsService.getDateFromDateTime(new Date())
					},
					settings: {
						effectiveDateDayOffset: effectiveDateDayOffset
					}
				};
				if (countryChanges != undefined)
					soldCountryGridPayload.changedCountries = countryChanges.ChangedCountries;
				VRUIUtilsService.callDirectiveLoad(soldCountryGridAPI, soldCountryGridPayload, soldCountryGridLoadDeferred);
			});
			return soldCountryGridLoadDeferred.promise;
		}

		function addCountry(country) {
			var dataItem = {
				Entity: {
					CountryId: country.CountryId,
					Name: country.Name,
					BED: WhS_Sales_RatePlanUtilsService.getNowPlusDays(effectiveDateDayOffset),
					EED: null
				}
			};
			$scope.scopeModel.newCountries.push(dataItem);
		}
		function removeCountry(countryId) {

			var countrySelectorIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedCountries, countryId, 'CountryId');
			$scope.scopeModel.selectedCountries.splice(countrySelectorIndex, 1);

			var newCountryEntities = getNewCountryEntities();
			var countryGridIndex = UtilsService.getItemIndexByVal(newCountryEntities, countryId, 'CountryId');
			$scope.scopeModel.newCountries.splice(countryGridIndex, 1);
		}

		function buildCountryChanges() {

			var countryChanges = {};
			countryChanges.ChangedCountries = soldCountryGridAPI.getData();

			if ($scope.scopeModel.newCountries.length > 0)
				countryChanges.NewCountries = getNewCountryEntities();

			return countryChanges;
		}
		function getNewCountryEntities() {
			return UtilsService.getPropValuesFromArray($scope.scopeModel.newCountries, 'Entity');
		}
	}

	appControllers.controller("WhS_Sales_SellNewCountriesController", SellNewCountriesController);

})(appControllers);