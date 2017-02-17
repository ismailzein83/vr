(function (appControllers) {
    "use strict";

    salePriceListPreviewController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function salePriceListPreviewController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService) {
        var priceListId;
        var filter = {};
        var codeChangeGridApi;
        var rateChangeGridApi;
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = utilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.onCodeChangeGridReady = function (api) {
                codeChangeGridApi = api;
                codeChangeGridApi.loadGrid(filter);
            };
            $scope.onRateChangeGridReady = function (api) {
                rateChangeGridApi = api;
                rateChangeGridApi.loadGrid(filter);
            };
            $scope.onCodeCountryReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };
            $scope.onRateCountryReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            };
            $scope.searchCodeClicked = function () {
                SetFilteredObject();
                return codeChangeGridApi.loadGrid(filter);
            };
            $scope.searchRateClicked = function () {
                SetFilteredObject();
                return rateChangeGridApi.loadGrid(filter);
            };
        }
        function SetFilteredObject() {
            if (priceListId != undefined) {
                filter =
                {
                    PriceListId: priceListId,
                    Countries: countryDirectiveApi.getSelectedIds()
                }
            }
        }
        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined) {
                priceListId = parameters.PriceListId;
                filter.PriceListId = priceListId;
            }
        }
        function loadCountrySelector() {
            var countryLoadPromiseDeferred = utilsService.createPromiseDeferred();

            countryReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};
                    vruiUtilsService.callDirectiveLoad(countryDirectiveApi, directivePayload, countryLoadPromiseDeferred);
                });
            return countryLoadPromiseDeferred.promise;
        }
        function setTitle() {
            if (priceListId != undefined)
                $scope.title = 'Sale Pricelist: ' + priceListId;
        }
        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([loadCountrySelector, setTitle])
              .catch(function (error) {
                  vrNotificationService.notifyExceptionWithClose(error, $scope);
              })
             .finally(function () {
                 $scope.isLoadingFilter = false;
             });
        }
    }
    appControllers.controller('WhS_BE_SalePriceListPreviewController', salePriceListPreviewController);
})(appControllers);