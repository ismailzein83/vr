(function (appControllers) {

    "use strict";

    SellNewCountriesController.$inject = ["$scope", 'WhS_Sales_RatePlanService', 'WhS_Sales_RatePlanUtilsService', "UtilsService", 'VRUIUtilsService', 'VRValidationService', "VRNavigationService", "VRNotificationService", "WhS_Sales_RatePlanAPIService", "WhS_BE_SalePriceListOwnerTypeEnum", "VRDateTimeService"];

    function SellNewCountriesController($scope, WhS_Sales_RatePlanService, WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService, VRValidationService, VRNavigationService, VRNotificationService, WhS_Sales_RatePlanAPIService, WhS_BE_SalePriceListOwnerTypeEnum, VRDateTimeService) {

        var customerId;
        var countryChanges;
        var saleAreaSettings;
        var customerPricingSettings;

        var retroactiveDayOffset = 0;
        var newRateDayOffset = 0;
        var endCountryDayOffset = 0;

        var retroactiveDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
        var newRateDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
        var endCountryDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

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
                customerPricingSettings = parameters.customerPricingSettings;
            }
        }
        function defineScope() {

            initGlobalVars();

            $scope.scopeModel = {};
            $scope.scopeModel.newCountries = [];

            $scope.scopeModel.onNewCountryGridReady = function (api) {
                newCountryGridAPI = api;
                newCountryGridReadyDeferred.resolve();
            };

            $scope.scopeModel.addCountries = function () {
                var onCountriesAdded = function (addedCountries) {
                    if (addedCountries == undefined)
                        return;
                    for (var i = 0; i < addedCountries.length; i++) {
                        var country = addedCountries[i];
                        $scope.scopeModel.newCountries.push({
                            country: country
                        });
                    }
                };

                var excludedCountryIds = getNewCountryIds();

                WhS_Sales_RatePlanService.addCountries({
                    customerId: customerId,
                    retroactiveDate: retroactiveDate,
                    newRateDate: newRateDate,
                    excludedCountryIds: excludedCountryIds,
                    onCountriesAdded: onCountriesAdded
                });

                function getNewCountryIds() {
                    var newCountryIds = [];
                    for (var i = 0; i < $scope.scopeModel.newCountries.length; i++)
                        newCountryIds.push($scope.scopeModel.newCountries[i].country.countryId);
                    return newCountryIds;
                }
            };

            $scope.scopeModel.removeNewCountry = function (newCountry) {
                var newCountries = getNewCountries();
                var newCountryGridIndex = UtilsService.getItemIndexByVal(newCountries, newCountry.country.countryId, 'countryId');
                $scope.scopeModel.newCountries.splice(newCountryGridIndex, 1);
            };

            $scope.scopeModel.isNewCountryBEDValid = function (dataItem) {
                if (dataItem.country.beginEffectiveDate == null)
                    return 'BED is a required field';
                if (dataItem.country.beginEffectiveDate < retroactiveDate)
                    return 'Retroactive Date: ' + UtilsService.getShortDate(retroactiveDate);
                return null;
            };

            $scope.scopeModel.onSoldCountryGridReady = function (api) {
                soldCountryGridAPI = api;
                soldCountryGridReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadNewCountryGrid, loadSoldCountryGrid]).then(function () {
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
        function loadNewCountryGrid() {
            newCountryGridReadyDeferred.promise.then(function () {
                if (countryChanges != undefined && countryChanges.NewCountries != null) {
                    for (var i = 0; i < countryChanges.NewCountries.length; i++) {
                        var newCountry = countryChanges.NewCountries[i];
                        $scope.scopeModel.newCountries.push({
                            country: {
                                countryId: newCountry.CountryId,
                                countryName: newCountry.Name,
                                beginEffectiveDate: newCountry.BED
                            }
                        });
                    }
                }
            });
            return newCountryGridReadyDeferred.promise;
        }
        function loadSoldCountryGrid() {
            var soldCountryGridLoadDeferred = UtilsService.createPromiseDeferred();
            soldCountryGridReadyDeferred.promise.then(function () {
                var soldCountryGridPayload = {
                    query: {
                        CustomerId: customerId,
                        EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())
                    },
                    settings: {
                        endCountryDate: endCountryDate
                    }
                };
                if (countryChanges != undefined)
                    soldCountryGridPayload.changedCountries = countryChanges.ChangedCountries;
                VRUIUtilsService.callDirectiveLoad(soldCountryGridAPI, soldCountryGridPayload, soldCountryGridLoadDeferred);
            });
            return soldCountryGridLoadDeferred.promise;
        }

        function buildCountryChanges() {
            var countryChanges = {
                ChangedCountries: soldCountryGridAPI.getData()
            };

            var newCountries = getNewCountries();

            if (newCountries != undefined) {
                countryChanges.NewCountries = [];

                for (var i = 0; i < newCountries.length; i++) {
                    var newCountry = newCountries[i];
                    var draftNewCountry = draftNewCountryMapper(newCountry);
                    countryChanges.NewCountries.push(draftNewCountry);
                }
            }

            return countryChanges;
        }
        function getNewCountries() {
            return UtilsService.getPropValuesFromArray($scope.scopeModel.newCountries, 'country');
        }
        function draftNewCountryMapper(country) {
            return {
                CountryId: country.countryId,
                Name: country.countryName,
                BED: country.beginEffectiveDate,
                EED: null
            };
        }

        function initGlobalVars() {
            if (customerPricingSettings != undefined) {
                if (customerPricingSettings.NewRateDayOffset != null)
                    newRateDayOffset = customerPricingSettings.NewRateDayOffset;
                if (customerPricingSettings.RetroactiveDayOffset != null)
                    retroactiveDayOffset = customerPricingSettings.RetroactiveDayOffset;
                if (customerPricingSettings.EndCountryDayOffset != null)
                    endCountryDayOffset = customerPricingSettings.EndCountryDayOffset;
            }
            if (newRateDayOffset > 0) {
                var newRateDateValue = WhS_Sales_RatePlanUtilsService.getNowPlusDays(newRateDayOffset);
                newRateDate = UtilsService.getDateFromDateTime(newRateDateValue);
            }
            if (retroactiveDayOffset > 0) {
                var retroactiveDateValue = WhS_Sales_RatePlanUtilsService.getNowMinusDays(retroactiveDayOffset);
                retroactiveDate = UtilsService.getDateFromDateTime(retroactiveDateValue);
            }
            if (endCountryDayOffset > 0) {
                var endCountryDateValue = WhS_Sales_RatePlanUtilsService.getNowPlusDays(endCountryDayOffset);
                endCountryDate = UtilsService.getDateFromDateTime(endCountryDateValue);
            }
        }
        function getNumberIfValid(value) {
            if (value != undefined) {
                var valueAsNumber = Number(value);
                if (!isNaN(valueAsNumber))
                    return valueAsNumber;
            }
            return 0;
        }
    }

    appControllers.controller("WhS_Sales_SellNewCountriesController", SellNewCountriesController);

})(appControllers);