(function (appControllers) {

    "use strict";

    SellNewZonesController.$inject = ["$scope", "WhS_BE_CustomerZoneAPIService", "UtilsService", "VRNavigationService", "VRNotificationService"];

    function SellNewZonesController($scope, WhS_BE_CustomerZoneAPIService, UtilsService, VRNavigationService, VRNotificationService) {

        var customerId;
        var countryGridAPI;
        var selectedSaleZones;

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
            $scope.title = "Sell New Zones";

            $scope.countries = [];
            $scope.disableSaveButton = true;

            $scope.onCountryGridReady = function (api) {
                countryGridAPI = api;

                WhS_BE_CustomerZoneAPIService.GetCountriesToSell(customerId).then(function (response) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.countries.push(response[i]);
                    }
                });
            };

            $scope.onCountryCheckChanged = function () {
                var country = UtilsService.getItemByVal($scope.countries, true, "isSelected");
                $scope.disableSaveButton = (country == undefined); // disable the add button if no country is selected
            };

            $scope.sellNewZones = function () {
                var customerZonesObj = buildCutomerZonesObjFromScope();

                WhS_BE_CustomerZoneAPIService.AddCustomerZones(customerZonesObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Customer Zones", response)) {
                        if ($scope.onCustomerZonesSold != undefined) {
                            $scope.onCustomerZonesSold(response);
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

    appControllers.controller("WhS_Sales_SellNewZonesController", SellNewZonesController);

})(appControllers);
