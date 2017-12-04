(function (appControllers) {

    "use strict";

    ManageCountriesController.$inject = ["$scope", 'WhS_Sales_RatePlanService', 'WhS_Sales_RatePlanUtilsService', "UtilsService", 'VRUIUtilsService', 'VRValidationService', "VRNavigationService", "VRNotificationService", "WhS_Sales_RatePlanAPIService", "WhS_BE_SalePriceListOwnerTypeEnum", "VRDateTimeService", 'WhS_Sales_CountrySelectionTypeEnum'];

    function ManageCountriesController($scope, WhS_Sales_RatePlanService, WhS_Sales_RatePlanUtilsService, UtilsService, VRUIUtilsService, VRValidationService, VRNavigationService, VRNotificationService, WhS_Sales_RatePlanAPIService, WhS_BE_SalePriceListOwnerTypeEnum, VRDateTimeService, WhS_Sales_CountrySelectionTypeEnum) {

        var customerId;
        var countryChanges;
        var customerPricingSettings;

        var retroactiveDayOffset = 0;
        var sellCountryDayOffset = 0;
        var endCountryDayOffset = 0;

        var retroactiveDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
        var sellCountryDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());
        var endCountryDate = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

        var countryToSellGridAPI;
        var countryToSellGridReadyDeferred = UtilsService.createPromiseDeferred();

        var countrySelectionTypeSelectorAPI;
        var countrySelectionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var countrySelectorAPI;
        var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var countryToCloseSelectorAPI;
        var countryToCloseSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var today = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                customerId = parameters.customerId;
                countryChanges = parameters.countryChanges;
                customerPricingSettings = parameters.customerPricingSettings;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            initGlobalVars();

            $scope.scopeModel.countriesToSell = [];
            $scope.scopeModel.isCountryToSellSelectorShown = false;
            $scope.scopeModel.beginEffectiveDate = sellCountryDate;

            $scope.scopeModel.countriesToClose = [];
            $scope.scopeModel.endEffectiveDate = (countryChanges != undefined && countryChanges.ChangedCountries != null) ? countryChanges.ChangedCountries.EED : endCountryDate;

            $scope.scopeModel.onCountryToSellGridReady = function (api) {
                countryToSellGridAPI = api;
                countryToSellGridReadyDeferred.resolve();
            };
            $scope.scopeModel.addCountriesToSell = function () {
                $scope.scopeModel.isLoading = true;
                addCountriesToSell().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
            $scope.scopeModel.removeCountryToSell = function (countryToSell) {
                $scope.scopeModel.isLoading = true;

                var countriesToSell = getGridEntities($scope.scopeModel.countriesToSell);
                var countryToSellGridIndex = UtilsService.getItemIndexByVal(countriesToSell, countryToSell.entity.countryId, 'countryId');
                $scope.scopeModel.countriesToSell.splice(countryToSellGridIndex, 1);

                loadCountryToSellSelector().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
            $scope.scopeModel.isCountryToSellBEDValid = function (dataItem) {
                if (dataItem.entity.beginEffectiveDate == null)
                    return 'BED is a required field';
                if (UtilsService.createDateFromString(dataItem.entity.beginEffectiveDate) < retroactiveDate)
                    return 'Retroactive Date: ' + UtilsService.getShortDate(retroactiveDate);
                return null;
            };

            $scope.scopeModel.onCountrySelectionTypeSelectorReady = function (api) {
                countrySelectionTypeSelectorAPI = api;
                countrySelectionTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCountrySelectionTypeSelected = function (selectedCountrySelectionType) {
                $scope.scopeModel.isCountryToSellSelectorShown = (selectedCountrySelectionType.value == WhS_Sales_CountrySelectionTypeEnum.Specific.value);
            };

            $scope.scopeModel.onCountryToSellSelectorReady = function (api) {
                countrySelectorAPI = api;
                countrySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.isBEDValid = function () {
                if ($scope.scopeModel.beginEffectiveDate == undefined)
                    return 'BED is required';
                if (retroactiveDate == undefined)
                    return 'Retroactive date was not found';
                return ($scope.scopeModel.beginEffectiveDate < retroactiveDate) ? ('Retroactive date: ' + UtilsService.getShortDate(retroactiveDate)) : null;
            };
            $scope.scopeModel.disableAddCountriesToSellButton = function () {
                if ($scope.scopeModel.selectedCountrySelectionType == undefined || $scope.scopeModel.beginEffectiveDate == undefined)
                    return true;
                if ($scope.scopeModel.selectedCountrySelectionType.value == WhS_Sales_CountrySelectionTypeEnum.All.value)
                    return false;
                return ($scope.scopeModel.selectedCountriesToSell == undefined || $scope.scopeModel.selectedCountriesToSell.length == 0);
            };

            $scope.scopeModel.onCountryToCloseSelectorReady = function (api) {
                countryToCloseSelectorAPI = api;
                countryToCloseSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.addCountriesToClose = function () {
                $scope.scopeModel.isLoading = true;
                addCountriesToClose().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
            $scope.scopeModel.removeCountryToClose = function (countryToClose) {
                $scope.scopeModel.isLoading = true;

                var countriesToClose = getGridEntities($scope.scopeModel.countriesToClose);
                var countryToCloseGridIndex = UtilsService.getItemIndexByVal(countriesToClose, countryToClose.entity.countryId, 'countryId');
                $scope.scopeModel.countriesToClose.splice(countryToCloseGridIndex, 1);

                loadCountryToCloseSelector().catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.isEEDValid = function () {
                if ($scope.scopeModel.countriesToClose.length > 0) {
                    if ($scope.scopeModel.endEffectiveDate != undefined && $scope.scopeModel.endEffectiveDate < today)
                        return 'EED must be greater than or equal to today';
                }
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
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadCountryToSellGrid, loadCountryToCloseGrid, loadCountrySelectionTypeSelector, loadCountryToSellSelector, loadCountryToCloseSelector]).then(function () {
                countryChanges = undefined;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = 'Manage Countries';
        }

        function loadCountryToSellGrid() {
            countryToSellGridReadyDeferred.promise.then(function () {
                if (countryChanges != undefined && countryChanges.NewCountries != null) {
                    for (var i = 0; i < countryChanges.NewCountries.length; i++) {
                        var newCountry = countryChanges.NewCountries[i];
                        $scope.scopeModel.countriesToSell.push({
                            entity: {
                                countryId: newCountry.CountryId,
                                countryName: newCountry.Name,
                                beginEffectiveDate: newCountry.BED
                            }
                        });
                    }
                }
            });
            return countryToSellGridReadyDeferred.promise;
        }
        function loadCountrySelectionTypeSelector() {
            countrySelectionTypeSelectorReadyDeferred.promise.then(function () {
                $scope.scopeModel.countrySelectionTypes = UtilsService.getArrayEnum(WhS_Sales_CountrySelectionTypeEnum);
                $scope.scopeModel.selectedCountrySelectionType = UtilsService.getItemByVal($scope.scopeModel.countrySelectionTypes, WhS_Sales_CountrySelectionTypeEnum.All.value, 'value');
            });
            return countrySelectionTypeSelectorReadyDeferred.promise;
        }
        function loadCountryToSellSelector() {
            var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            countrySelectorReadyDeferred.promise.then(function () {
                var countrySelectorPayload = {
                    filter: getCountrySelectorFilter()
                };
                VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);
            });

            function getCountrySelectorFilter() {
                var filter = {
                    ExcludedCountryIds: getCountryIdsToSell()
                };
                var effectiveOn = UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime());

                filter.Filters = [];

                var notSoldToCustomerFilter = {
                    $type: 'TOne.WhS.BusinessEntity.Business.CountryNotSoldToCustomerFilter, TOne.WhS.BusinessEntity.Business',
                    CustomerId: customerId,
                    EffectiveOn: effectiveOn
                };
                filter.Filters.push(notSoldToCustomerFilter);

                var countryNotEmptyFilter = {
                    $type: 'TOne.WhS.BusinessEntity.Business.CountryNotEmptyFilter, TOne.WhS.BusinessEntity.Business',
                    CustomerId: customerId,
                    EffectiveOn: effectiveOn
                };
                filter.Filters.push(countryNotEmptyFilter);

                function getCountryIdsToSell() {
                    var countryIdsToSell = [];
                    if (countryChanges != undefined) {
                        if (countryChanges.NewCountries != null) {
                            for (var i = 0; i < countryChanges.NewCountries.length; i++)
                                countryIdsToSell.push(countryChanges.NewCountries[i].CountryId);
                        }
                    }
                    else {
                        countryIdsToSell = getCountryIds($scope.scopeModel.countriesToSell);
                    }
                    return countryIdsToSell;
                }

                return filter;
            }

            return countrySelectorLoadDeferred.promise;
        }

        function loadCountryToCloseGrid() {
            countryToCloseSelectorReadyDeferred.promise.then(function () {
                if (countryChanges != undefined && countryChanges.ChangedCountries != null && countryChanges.ChangedCountries.Countries != null) {
                    for (var i = 0; i < countryChanges.ChangedCountries.Countries.length; i++) {
                        var changedCountry = countryChanges.ChangedCountries.Countries[i];
                        $scope.scopeModel.countriesToClose.push({
                            entity: {
                                countryId: changedCountry.CountryId,
                                countryName: changedCountry.Name
                            }
                        });
                    }
                }
            });
            return countryToCloseSelectorReadyDeferred.promise;
        }
        function loadCountryToCloseSelector() {
            var countryToCloseSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            countryToCloseSelectorReadyDeferred.promise.then(function () {
                var countryToCloseSelectorPayload = {
                    selectedIds: (countryChanges != undefined && countryChanges.ChangedCountries != null) ? countryChanges.ChangedCountries.CountryIds : undefined,
                    filter: getCountryToCloseSelectorFilter()
                };
                VRUIUtilsService.callDirectiveLoad(countryToCloseSelectorAPI, countryToCloseSelectorPayload, countryToCloseSelectorLoadDeferred);
            });

            function getCountryToCloseSelectorFilter() {
                var filter = {
                    ExcludedCountryIds: getCountryIdsToClose()
                };
                filter.Filters = [];
                var countryToCloseFilter = {
                    $type: 'TOne.WhS.Sales.Business.CountryToCloseFilter, TOne.WhS.Sales.Business',
                    CustomerId: customerId,
                    EffectiveOn: UtilsService.getDateFromDateTime(VRDateTimeService.getNowDateTime())
                };
                filter.Filters.push(countryToCloseFilter);

                function getCountryIdsToClose() {
                    var countryIdsToClose = [];
                    if (countryChanges != undefined) {
                        if (countryChanges.ChangedCountries != null && countryChanges.ChangedCountries.Countries != null) {
                            for (var i = 0; i < countryChanges.ChangedCountries.Countries.length; i++)
                                countryIdsToClose.push(countryChanges.ChangedCountries.Countries[i].CountryId);
                        }
                    }
                    else {
                        countryIdsToClose = getCountryIds($scope.scopeModel.countriesToClose);
                    }
                    return countryIdsToClose;
                }

                return filter;
            }

            return countryToCloseSelectorLoadDeferred.promise;
        }

        function initGlobalVars() {
            if (customerPricingSettings != undefined) {
                if (customerPricingSettings.NewRateDayOffset != null)
                    sellCountryDayOffset = customerPricingSettings.NewRateDayOffset;
                if (customerPricingSettings.RetroactiveDayOffset != null)
                    retroactiveDayOffset = customerPricingSettings.RetroactiveDayOffset;
                if (customerPricingSettings.EndCountryDayOffset != null)
                    endCountryDayOffset = customerPricingSettings.EndCountryDayOffset;
            }
            if (sellCountryDayOffset > 0) {
                var sellCountryDateValue = WhS_Sales_RatePlanUtilsService.getNowPlusDays(sellCountryDayOffset);
                sellCountryDate = UtilsService.getDateFromDateTime(sellCountryDateValue);
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
        function buildCountryChanges() {
            var countryChanges = {};
            countryChanges.NewCountries = getNewCountries();
            countryChanges.ChangedCountries = getChangedCountries();

            function getNewCountries() {
                var newCountries;
                if ($scope.scopeModel.countriesToSell.length > 0) {
                    newCountries = [];
                    var countriesToSell = getGridEntities($scope.scopeModel.countriesToSell);
                    for (var i = 0; i < countriesToSell.length; i++) {
                        var countryToSell = countriesToSell[i];
                        newCountries.push({
                            CountryId: countryToSell.countryId,
                            Name: countryToSell.countryName,
                            BED: countryToSell.beginEffectiveDate,
                            EED: null
                        });
                    }
                }
                return newCountries;
            }
            function getChangedCountries() {
                var changedCountries;
                if ($scope.scopeModel.countriesToClose.length > 0) {
                    changedCountries = {
                        Countries: [],
                        EED: $scope.scopeModel.endEffectiveDate
                    };
                    for (var i = 0; i < $scope.scopeModel.countriesToClose.length; i++) {
                        var countryToClose = $scope.scopeModel.countriesToClose[i];
                        changedCountries.Countries.push({
                            CountryId: countryToClose.entity.countryId,
                            Name: countryToClose.entity.countryName
                        });
                    }
                }
                return changedCountries;
            }

            return countryChanges;
        }

        function addCountriesToSell() {
            var promises = [];

            var selectedCountries = getSelectedCountriesToSell();
            if (selectedCountries != undefined && selectedCountries.length > 0) {

                for (var i = 0; i < selectedCountries.length; i++) {
                    var selectedCountry = selectedCountries[i];
                    $scope.scopeModel.countriesToSell.push({
                        entity: {
                            countryId: selectedCountry.CountryId,
                            countryName: selectedCountry.Name,
                            beginEffectiveDate: $scope.scopeModel.beginEffectiveDate
                        }
                    });
                }
            }

            // Reload the country selector to exclude the already added countries from being reselected
            var loadCountrySelectorPromise = loadCountryToSellSelector();
            promises.push(loadCountrySelectorPromise);

            function getSelectedCountriesToSell() {
                var selectedCountries;
                if ($scope.scopeModel.selectedCountrySelectionType.value == WhS_Sales_CountrySelectionTypeEnum.All.value)
                    selectedCountries = countrySelectorAPI.getAllCountries();
                else if ($scope.scopeModel.selectedCountriesToSell != null)
                    selectedCountries = $scope.scopeModel.selectedCountriesToSell;
                return selectedCountries;
            }

            return UtilsService.waitMultiplePromises(promises);
        }
        function addCountriesToClose() {
            var promises = [];

            if ($scope.scopeModel.selectedCountriesToClose != undefined && $scope.scopeModel.selectedCountriesToClose.length != 0) {
                for (var i = 0; i < $scope.scopeModel.selectedCountriesToClose.length; i++) {
                    var selectedCountryToClose = $scope.scopeModel.selectedCountriesToClose[i];
                    $scope.scopeModel.countriesToClose.push({
                        entity: {
                            countryId: selectedCountryToClose.CountryId,
                            countryName: selectedCountryToClose.Name
                        }
                    });
                }
            }

            var loadCountryToCloseSelectorPromise = loadCountryToCloseSelector();
            promises.push(loadCountryToCloseSelectorPromise);

            return UtilsService.waitMultiplePromises(promises);
        }

        function getGridEntities(gridDataSource) {
            return UtilsService.getPropValuesFromArray(gridDataSource, 'entity');
        }
        function getCountryIds(gridDataSource) {
            var countryIds = [];
            for (var i = 0; i < gridDataSource.length; i++)
                countryIds.push(gridDataSource[i].entity.countryId);
            return countryIds;
        }
    }

    appControllers.controller("WhS_Sales_ManageCountriesController", ManageCountriesController);

})(appControllers);