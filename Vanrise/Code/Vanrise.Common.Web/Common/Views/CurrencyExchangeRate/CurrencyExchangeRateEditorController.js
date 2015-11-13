(function (appControllers) {

    "use strict";

    currencyExchangeRateEditorController.$inject = ['$scope', 'VRCommon_CurrencyExchangeRateAPIService', 'VRNotificationService', 'VRNavigationService'];

    function currencyExchangeRateEditorController($scope, VRCommon_CurrencyExchangeRateAPIService, VRNotificationService, VRNavigationService) {

        
        var currencySelectorAPI;
        var currencyId;
        var disableCurrency;
        defineScope();
        loadParameters();
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
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.onCurrencySelectReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorAPI.load({ selectedIds: currencyId });
            }
          
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
            });

        }
       
    }

    appControllers.controller('VRCommon_CurrencyExchangeRateEditorController', currencyExchangeRateEditorController);
})(appControllers);
