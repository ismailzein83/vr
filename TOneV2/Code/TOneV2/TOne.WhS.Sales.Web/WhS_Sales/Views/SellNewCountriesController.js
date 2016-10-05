(function (appControllers) {

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
            $scope.title = "Sell New Countries";

            $scope.countries = [];
            
            $scope.onCountryGridReady = function (api) {
                countryGridReadyDeferred.resolve();
            };

            $scope.selectAllCountries = function () {
                allCountriesSelected = !allCountriesSelected;
                for (var i = 0; i < $scope.countries.length; i++) {
                    $scope.countries[i].isSelected = allCountriesSelected;
                }
            };

            $scope.sellNewCountries = function () {
                var customerZonesObj = buildCutomerZonesObjFromScope();

                WhS_BE_CustomerZoneAPIService.AddCustomerZones(customerZonesObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Countries", response)) {
                        if ($scope.onCountriesSold != undefined) {
                            $scope.onCountriesSold(response.InsertedObject);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.validateSelection = function () {
                var country = UtilsService.getItemByVal($scope.countries, true, "isSelected");
                if (country != undefined)
                    return null;
                return 'No countries selected';
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCountryGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
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

        function buildCutomerZonesObjFromScope()
        {
            var startEffectiveTime = UtilsService.getDateFromDateTime(new Date());

            return {
                CustomerId: customerId,
                Countries: getSelectedCountries(startEffectiveTime),
                StartEffectiveTime: startEffectiveTime
            };
        }
        function getSelectedCountries(startEffectiveTime) {
            if ($scope.countries.length == 0)
                return null;

            var selectedCountries = [];

            for (var i = 0; i < $scope.countries.length; i++) {
                var country = $scope.countries[i];

                if (country.isSelected) {
                    selectedCountries.push({
                        CountryId: country.CountryId,
                        StartEffectiveTime: startEffectiveTime
                    });
                }
            }

            return selectedCountries;
        }
    }

    appControllers.controller("WhS_Sales_SellNewCountriesController", SellNewCountriesController);

})(appControllers);