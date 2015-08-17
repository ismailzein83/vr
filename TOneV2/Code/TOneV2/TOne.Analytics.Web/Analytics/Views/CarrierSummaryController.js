(function (appControllers) {

    "use strict";

    CarrierSummaryController.$inject = ['$scope', 'CarrierSummaryStatsAPIService', 'CarrierAccountAPIService', 'CurrencyAPIService', 'CarrierTypeEnum', 'VRModalService'];
    function CarrierSummaryController($scope, CarrierSummaryStatsAPIService, CarrierAccountAPIService, CurrencyAPIService, CarrierTypeEnum, VRModalService) {

        var mainGridApi;

        function retrieveData() {
            return mainGridApi.retrieveData({
                Name: $scope.name
            });
        }


        function defineGrid() {

            $scope.datasource = [];

            $scope.onGridReady = function (api) {
                mainGridApi = api;
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CarrierSummaryStatsAPIService.GetFilteredCarrierMasks(dataRetrievalInput)
                .then(function (response) {

                    onResponseReady(response);
                });
            };
        }

        function defineScope() {

            $scope.datasource = [];

            $scope.optionsCurrencies = {
                selectedvalues: '',
                datasource: []
            };
        }

        function load() {

            loadCarriers();
            loadCurrencies();

        }

        function loadCarriers() {
            return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.SaleZone.value).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.datasource.push(itm);
                });
            });
        }

        function loadCurrencies() {
            return CurrencyAPIService.GetCurrencies().then(function (response) {
                $scope.optionsCurrencies.datasource = response;
            });

        }


        defineScope();
        load();
    }
    appControllers.controller('Carrier_CarrierSummaryController', CarrierSummaryController);

})(appControllers);