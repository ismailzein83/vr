(function (appControllers) {

    "use strict";

    CarrierSummaryStatsController.$inject = ['$scope', 'CarrierSummaryStatsAPIService', 'CarrierAccountAPIService', 'CurrencyAPIService', 'CarrierTypeEnum', 'VRModalService'];
    function CarrierSummaryStatsController($scope, CarrierSummaryStatsAPIService, CarrierAccountAPIService, CurrencyAPIService, CarrierTypeEnum, VRModalService) {

        var gridApi;

        function retrieveData() {
            console.log($scope.optionsCurrencies.selectedvalues);

            return gridApi.retrieveData({
                CarrierType: $scope.isCustomer,
                CustomerID: $scope.selectedvalues.CarrierAccountID,
                TopRecord: $scope.top,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                Currency: $scope.optionsCurrencies.selectedvalues.CurrencyID,
                GroupByProfile: $scope.byProfile,
                ShowInactive: $scope.showInactive
            });
        }

        function defineScope() {

            $scope.datasource = [];

            $scope.optionsCurrencies = {
                selectedvalues: '',
                datasource: []
            };

            $scope.gridReady = function (api) {
                gridApi = api;
                return retrieveData();
            };

            $scope.searchClicked = function () {
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CarrierSummaryStatsAPIService.GetFilteredCarrierSummaryStats(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                });
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
    appControllers.controller('Carrier_CarrierSummaryStatsController', CarrierSummaryStatsController);

})(appControllers);