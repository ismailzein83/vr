(function (appControllers) {

    "use strict";

    currencyExchangeRateEditorController.$inject = ['$scope', 'VRCommon_CurrencyExchangeRateAPIService', 'VRNotificationService', 'VRNavigationService'];

    function currencyExchangeRateEditorController($scope, VRCommon_CurrencyExchangeRateAPIService, VRNotificationService, VRNavigationService) {

        
        var currencySelectorAPI;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            
        }
        function defineScope() {
            $scope.saveExchangeRate = function () {
                    return insertCurrency();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.onCurrencySelectReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorAPI.load();
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

  
        function insertCurrency() {
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
