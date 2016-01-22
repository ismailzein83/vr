(function (appControllers) {

    "use strict";

    currencyEditorController.$inject = ['$scope', 'VRCommon_CurrencyAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function currencyEditorController($scope, VRCommon_CurrencyAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        
        var currencyId;
        var currencyEntity;
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
                    getCurrency().then(function () {
                        loadAllControls().finally(function () {
                            currencyEntity = undefined;
                        });
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                }
                else {
                    loadAllControls();
                }
          
        }
        function getCurrency() {
            return VRCommon_CurrencyAPIService.GetCurrency(currencyId).then(function (currency) {
                currencyEntity = currency;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isGettingData = false;
              });
        }
        function setTitle() {
            if (editMode && currencyEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(currencyEntity.Name, "Currency");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Currency");
        }

        function loadStaticData() {

            if (currencyEntity == undefined)
                return;

            $scope.name = currencyEntity.Name;
            $scope.symbol = currencyEntity.Symbol;
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
            $scope.isGettingData = false;
            return VRCommon_CurrencyAPIService.AddCurrency(currencyObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Currency", response,"Symbol")) {
                    if ($scope.onCurrencyAdded != undefined)
                        $scope.onCurrencyAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });

        }
        function updateCurrency() {
            var currencyObject = buildCurrencyObjFromScope();
            $scope.isGettingData = false;
            VRCommon_CurrencyAPIService.UpdateCurrency(currencyObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Currency", response, "Symbol")) {
                    if ($scope.onCurrencyUpdated != undefined)
                        $scope.onCurrencyUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
        }
    }

    appControllers.controller('VRCommon_CurrencyEditorController', currencyEditorController);
})(appControllers);
