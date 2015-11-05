(function (appControllers) {

    "use strict";

    currencyExchangeRateManagementController.$inject = ['$scope', 'VRCommon_CurrencyExchangeRateService'];

    function currencyExchangeRateManagementController($scope, VRCommon_CurrencyExchangeRateService) {
        var gridAPI;
        var currencySelectorAPI;
        defineScope();
        load();
        var filter = {};

        function defineScope() {
            $scope.searchClicked = function () {
                setFilterObject();
                gridAPI.loadGrid(filter)
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter)
            }

            $scope.onCurrencySelectReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorAPI.load();
            }
            $scope.addNewCurrencyExchangeRate = addNewCurrencyExchangeRate;
        }

        function load() {

           

        }

        function setFilterObject() {
            console.log(currencySelectorAPI.getSelectedIds());
            filter = {
                ExchangeDate: $scope.exchangeDate,
                Currencies: currencySelectorAPI.getSelectedIds() ? currencySelectorAPI.getSelectedIds() : null 
            };
        }

        function addNewCurrencyExchangeRate() {
           
            var onExchangeRateAdded = function (exchangeRateObj) {
                if (gridAPI != undefined) {
                    gridAPI.onExchangeRateAdded(exchangeRateObj);
                }

            };
            VRCommon_CurrencyExchangeRateService.addExchangeRate(onExchangeRateAdded);
        }

    }

    appControllers.controller('VRCommon_CurrencyExchangeRateManagementController', currencyExchangeRateManagementController);
})(appControllers);