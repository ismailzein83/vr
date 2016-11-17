(function (appControllers) {

    "use strict";

    currencyExchangeRateManagementController.$inject = ['$scope', 'VRCommon_CurrencyExchangeRateService',  'UtilsService', 'VRUIUtilsService'];

    function currencyExchangeRateManagementController($scope, VRCommon_CurrencyExchangeRateService, UtilsService, VRUIUtilsService) {
        var gridAPI;

        var currencySelectorAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        var filter = {};

        function defineScope() {
            $scope.exchangeDate = new Date();
            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                getFilterObject();
                gridAPI.loadGrid(filter)
            };

            $scope.onCurrencySelectReady = function (api) {
                currencySelectorAPI = api;
                currencyReadyPromiseDeferred.resolve();
            };
            $scope.addNewCurrencyExchangeRate = addNewCurrencyExchangeRate;
        }

        function load() {
            $scope.isGettingData = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCurrencySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }
        function loadCurrencySelector() {
            var currencyLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            currencyReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, undefined, currencyLoadPromiseDeferred);
                });
            return currencyLoadPromiseDeferred.promise;
        }

        function getFilterObject() {
            filter = {
                ExchangeDate: $scope.exchangeDate,
                Currencies: currencySelectorAPI.getSelectedIds() ? currencySelectorAPI.getSelectedIds() : null 
            };
        }

        function addNewCurrencyExchangeRate() {
           
            var onExchangeRateAdded = function (exchangeRateObj) {
                gridAPI.onExchangeRateAdded(exchangeRateObj);
            };

            VRCommon_CurrencyExchangeRateService.addExchangeRate(onExchangeRateAdded);
        }
    }

    appControllers.controller('VRCommon_CurrencyExchangeRateManagementController', currencyExchangeRateManagementController);
})(appControllers);