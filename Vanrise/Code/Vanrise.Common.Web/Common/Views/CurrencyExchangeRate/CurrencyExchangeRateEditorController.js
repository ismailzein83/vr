(function (appControllers) {

    "use strict";

    currencyExchangeRateEditorController.$inject = ['$scope', 'VRCommon_CurrencyExchangeRateAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function currencyExchangeRateEditorController($scope, VRCommon_CurrencyExchangeRateAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        
        var currencySelectorAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var currencyId;
        var disableCurrency;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                currencyId = parameters.CurrencyId;
            }
            $scope.disableCurrency = (currencyId != undefined);
        }

        function defineScope() {
            $scope.saveExchangeRate = function () {
                return insertCurrencyExchangeRate();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.onCurrencySelectReady = function (api) {
                currencySelectorAPI = api;
                currencyReadyPromiseDeferred.resolve();
            };


        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
            $scope.title = UtilsService.buildTitleForAddEditor("Currency Exchange Rate");
           
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadCurrencySelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadCurrencySelector() {
            var currencyLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            currencyReadyPromiseDeferred.promise
                .then(function () {
                var directivePayload = {
                    selectedIds:  currencyId 
                };

                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, directivePayload, currencyLoadPromiseDeferred);
            });
            return currencyLoadPromiseDeferred.promise;
        }


        function buildCurrencyExchangeRateObjFromScope() {
            var obj = {
                CurrencyExchangeRateId : 0,
                CurrencyId: currencySelectorAPI.getSelectedIds() ,
                Rate: $scope.rate,
                ExchangeDate: $scope.exchangeDate
            };
            return obj;
        }

  
        function insertCurrencyExchangeRate() {
            $scope.isLoading = true;

            var object = buildCurrencyExchangeRateObjFromScope();

            return VRCommon_CurrencyExchangeRateAPIService.AddCurrencyExchangeRate(object)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Currency Exchange Rate", response)) {
                    if ($scope.onCurrencyExchangeRateAdded != undefined)
                        $scope.onCurrencyExchangeRateAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VRCommon_CurrencyExchangeRateEditorController', currencyExchangeRateEditorController);
})(appControllers);
