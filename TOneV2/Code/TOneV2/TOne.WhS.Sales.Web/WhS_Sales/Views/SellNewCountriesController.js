﻿(function (appControllers) {

    "use strict";

    SellNewCountriesController.$inject = ["$scope", "WhS_BE_CustomerZoneAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function SellNewCountriesController($scope, WhS_BE_CustomerZoneAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var customerId;
        var countryGridReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedSaleZones;

        var allCountriesSelected = false;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                customerId = parameters.CustomerId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.title = "Sell New Countries";
            $scope.showInfo = false;

            $scope.countries = [];
            $scope.disableSaveButton = true;

            $scope.onCountryGridReady = function (api) {
                countryGridReadyDeferred.resolve();
            };

            $scope.onCountryCheckChanged = function () {
                var country = UtilsService.getItemByVal($scope.countries, true, "isSelected");
                $scope.disableSaveButton = (country == undefined); // disable the add button if no country is selected
            };

            $scope.selectAllCountries = function () {
                allCountriesSelected = !allCountriesSelected;
                $scope.disableSaveButton = !allCountriesSelected;

                for (var i = 0; i < $scope.countries.length; i++) {
                    $scope.countries[i].isSelected = allCountriesSelected;
                }
            };

            $scope.sellNewCountries = function () {
                var customerZonesObj = buildCutomerZonesObjFromScope();

                WhS_BE_CustomerZoneAPIService.AddCustomerZones(customerZonesObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Countries", response)) {
                        if ($scope.onCountriesSold != undefined) {
                            $scope.onCountriesSold(response);
                        }

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountryGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadCountryGrid() {
            var countries;

            function getCountries() {
                return WhS_BE_CustomerZoneAPIService.GetCountriesToSell(customerId).then(function (response) {
                    countries = response;
                });
            }

            return UtilsService.waitMultiplePromises([countryGridReadyDeferred.promise, getCountries()]).then(function () {
                if (countries != undefined) {
                    for (var i = 0; i < countries.length; i++)
                        $scope.countries.push(countries[i]);
                }
            });
        }

        function buildCutomerZonesObjFromScope() {
            return {
                CustomerId: customerId,
                Countries: getSelectedCountries(),
                StartEffectiveTime: new Date()
            };
        }
        function getSelectedCountries() {
            if ($scope.countries.length == 0)
                return null;

            var selectedCountries = [];

            for (var i = 0; i < $scope.countries.length; i++) {
                var country = $scope.countries[i];

                if (country.isSelected) {
                    selectedCountries.push({
                        CountryId: country.CountryId
                    });
                }
            }

            return selectedCountries;
        }
    }

    appControllers.controller("WhS_Sales_SellNewCountriesController", SellNewCountriesController);

})(appControllers);