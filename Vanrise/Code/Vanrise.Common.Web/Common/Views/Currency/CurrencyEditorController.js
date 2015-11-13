(function (appControllers) {

    "use strict";

    currencyEditorController.$inject = ['$scope', 'VRCommon_CurrencyAPIService', 'VRNotificationService', 'VRNavigationService'];

    function currencyEditorController($scope, VRCommon_CurrencyAPIService, VRNotificationService, VRNavigationService) {

        
        var currencyId;
        var editMode;
        defineScope();
        loadParameters();
        load();
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                currencyId = parameters.CurrencyId;
            }
            editMode = (currencyId != undefined);
        }
        function defineScope() {
            $scope.saveCurrency= function () {
                    if (editMode) {
                        return updateCurrency();
                    }
                    else {
                        return insertCurrency();
                    }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
                $scope.isGettingData = true;
                if (editMode) {
                  getCurrency();
                }
                else {
                    $scope.isGettingData = false;
                }
          
        }
        function getCurrency() {
            return VRCommon_CurrencyAPIService.GetCurrency(currencyId).then(function (currency) {
                fillScopeFromCurrencyObj(currency);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function buildCurrencyObjFromScope() {
            var obj = {
                CurrencyId: (currencyId != null) ? currencyId : 0,
                Name: $scope.name,
                Symbol: $scope.symbol
            };
            return obj;
        }

        function fillScopeFromCurrencyObj(currency) {
            $scope.name = currency.Name;
            $scope.symbol = currency.Symbol;
        }
        function insertCurrency() {
            var currencyObject = buildCurrencyObjFromScope();
            return VRCommon_CurrencyAPIService.AddCurrency(currencyObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Currency", response,"Symbol")) {
                    if ($scope.onCurrencyAdded != undefined)
                        $scope.onCurrencyAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }
        function updateCurrency() {
            var currencyObject = buildCurrencyObjFromScope();
            VRCommon_CurrencyAPIService.UpdateCurrency(currencyObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Currency", response, "Symbol")) {
                    if ($scope.onCurrencyUpdated != undefined)
                        $scope.onCurrencyUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }
    }

    appControllers.controller('VRCommon_CurrencyEditorController', currencyEditorController);
})(appControllers);
