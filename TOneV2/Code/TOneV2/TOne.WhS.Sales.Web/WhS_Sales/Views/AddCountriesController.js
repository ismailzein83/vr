(function (appControllers) {

    "use strict";

    AddCountriesController.$inject = ['$scope', 'WhS_Sales_RatePlanUtilsService', 'WhS_Sales_CountrySelectionTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRDateTimeService'];

    function AddCountriesController($scope, WhS_Sales_RatePlanUtilsService, WhS_Sales_CountrySelectionTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRDateTimeService) {

        var customerId;

        var retroactiveDate;
        var newRateDate;
        var excludedCountryIds;

        var countrySelectionTypeSelectorAPI;
        var countrySelectionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var countrySelectorAPI;
        var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                customerId = parameters.customerId;
                retroactiveDate = parameters.retroactiveDate;
                newRateDate = parameters.newRateDate;
                excludedCountryIds = parameters.excludedCountryIds;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isCountrySelectorShown = false;
            $scope.scopeModel.beginEffectiveDate = newRateDate;

            $scope.scopeModel.onCountrySelectionTypeSelectorReady = function (api) {
                countrySelectionTypeSelectorAPI = api;
                countrySelectionTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCountrySelectionTypeSelected = function (selectedCountrySelectionType) {
                $scope.scopeModel.isCountrySelectorShown = (selectedCountrySelectionType.value == WhS_Sales_CountrySelectionTypeEnum.Specific.value);
            };

            $scope.scopeModel.onCountrySelectorReady = function (api) {
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

            $scope.scopeModel.save = function () {
                var addedCountries;
                if ($scope.scopeModel.selectedCountrySelectionType.value == WhS_Sales_CountrySelectionTypeEnum.All.value) {
                    var allCountries = countrySelectorAPI.getAllCountries();
                    if (allCountries != undefined)
                        addedCountries = getMappedCountries(allCountries);
                }
                else if ($scope.scopeModel.selectedCountries != null) {
                    addedCountries = getMappedCountries($scope.scopeModel.selectedCountries);
                }
                if ($scope.onCountriesAdded != undefined)
                    $scope.onCountriesAdded(addedCountries);
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
            UtilsService.waitMultipleAsyncOperations([setTitle, loadCountrySelectionTypeSelector, loadCountrySelector]).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = 'Add Countries';
        }
        function loadCountrySelectionTypeSelector() {
            countrySelectionTypeSelectorReadyDeferred.promise.then(function () {
                $scope.scopeModel.countrySelectionTypes = UtilsService.getArrayEnum(WhS_Sales_CountrySelectionTypeEnum);
                $scope.scopeModel.selectedCountrySelectionType = UtilsService.getItemByVal($scope.scopeModel.countrySelectionTypes, WhS_Sales_CountrySelectionTypeEnum.All.value, 'value');
            });
            return countrySelectionTypeSelectorReadyDeferred.promise;
        }
        function loadCountrySelector() {
            var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            countrySelectorReadyDeferred.promise.then(function () {
                var countrySelectorPayload = {
                    filter: getCountrySelectorFilter()
                };
                VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);
            });

            function getCountrySelectorFilter() {
                var filter = {
                    ExcludedCountryIds: excludedCountryIds
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

                return filter;
            }

            return countrySelectorLoadDeferred.promise;
        }

        function getMappedCountries(countries) {
            var mappedCountries = [];
            for (var i = 0; i < countries.length; i++) {
                var country = countries[i];
                mappedCountries.push({
                    countryId: country.CountryId,
                    countryName: country.Name,
                    beginEffectiveDate: $scope.scopeModel.beginEffectiveDate
                });
            }
            return mappedCountries;
        }
    }

    appControllers.controller("WhS_Sales_AddCountriesController", AddCountriesController);

})(appControllers);