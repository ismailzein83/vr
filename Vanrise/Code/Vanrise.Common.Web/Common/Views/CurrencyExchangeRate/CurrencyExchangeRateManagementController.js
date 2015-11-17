(function (appControllers) {

    "use strict";

    currencyExchangeRateManagementController.$inject = ['$scope', 'VRCommon_CurrencyExchangeRateService',  'UtilsService', 'VRUIUtilsService'];

    function currencyExchangeRateManagementController($scope, VRCommon_CurrencyExchangeRateService, UtilsService, VRUIUtilsService) {
        var gridAPI;
        var currencyDirectiveApi;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
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
                currencyDirectiveApi = api;
                currencyReadyPromiseDeferred.resolve();
            }
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
                    var directivePayload = {};

                    VRUIUtilsService.callDirectiveLoad(currencyDirectiveApi, directivePayload, currencyLoadPromiseDeferred);
                });
            return currencyLoadPromiseDeferred.promise;
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