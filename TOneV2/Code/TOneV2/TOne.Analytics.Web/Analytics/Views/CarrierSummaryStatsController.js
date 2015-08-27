(function (appControllers) {

    "use strict";

    CarrierSummaryStatsController.$inject = ['$scope', 'CarrierSummaryStatsAPIService', 'CarrierAccountAPIService', 'CarrierSummaryMeasureEnum', 'ZoneAPIService', 'CurrencyAPIService', 'CarrierTypeEnum', 'VRModalService'];
    function CarrierSummaryStatsController($scope, CarrierSummaryStatsAPIService, CarrierAccountAPIService, CarrierSummaryMeasureEnum, ZoneAPIService, CurrencyAPIService, CarrierTypeEnum, VRModalService) {

        var gridApi;

        function retrieveData() {

            $scope.datasource = [];

            $scope.getHeader = $scope.optionsGroups.selectedvalues.value == 3 ? "Zone" : $scope.byProfile ? "Profile" : "Carrier";

            

            if ($scope.selectedvalues == undefined)
                $scope.selectedvalues = null;

            if ($scope.optionsCurrencies.selectedvalues == undefined)
                $scope.optionsCurrencies.selectedvalues = null;

            return gridApi.retrieveData({
                CarrierType: $scope.optionsGroups.selectedvalues.value,
                CustomerID: $scope.optionsGroups.selectedvalues.value != 1 ? null : $scope.optionsCarriers.selectedvalues == null ? null : $scope.optionsCarriers.selectedvalues.CarrierAccountID,
                SupplierID: $scope.optionsGroups.selectedvalues.value != 2 ? null : $scope.optionsCarriers.selectedvalues == null ? null : $scope.optionsCarriers.selectedvalues.CarrierAccountID,
                ZoneID: $scope.optionsGroups.selectedvalues.value != 3 ? null : $scope.selectedvaluesZones == null ? null : $scope.selectedvaluesZones.ZoneId,
                TopRecord: $scope.top,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                Currency: $scope.optionsCurrencies.selectedvalues == null ? null : $scope.optionsCurrencies.selectedvalues.CurrencyID,
                GroupByProfile: $scope.byProfile,
                ShowInactive: $scope.showInactive
            });
        }

        function defineScope() {
            $scope.top = 2000;
            $scope.selectedvaluesZones = [];
            $scope.measures = [];
            var groupKeys = [];
            groupKeys.push({ name: "Customer", value: 1 });
            groupKeys.push({ name: "Supplier", value: 2 });
            groupKeys.push({ name: "Zone", value: 3 });
            $scope.optionsGroups = {
                selectedvalues: '',
                datasource: groupKeys
            };

          
            $scope.optionsCurrencies = {
                selectedvalues: '',
                datasource: []
            };

            $scope.optionsCarriers = {
                selectedvalues: '',
                datasource: []
            };
           
            $scope.getHeader = "";
            $scope.gridReady = function (api) {
                gridApi = api;
                //return retrieveData();
            };

            $scope.searchClicked = function () {
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CarrierSummaryStatsAPIService.GetFilteredCarrierSummaryStats(dataRetrievalInput)
                .then(function (response) {
                    gridApi.setSummary(response.Summary);
                    onResponseReady(response);
                });
            };
        }

        function load() {
            loadMeasures();
            loadCarriers();
            loadCurrencies();
        }

        function loadCarriers() {
            return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.SaleZone.value,false).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.optionsCarriers.datasource.push(itm);
                });
            });
        }

        function loadCurrencies() {
            return CurrencyAPIService.GetCurrencies().then(function (response) {
                $scope.optionsCurrencies.datasource = response;
            });

        }

        function loadMeasures() {
            for (var prop in CarrierSummaryMeasureEnum) {
                $scope.measures.push(CarrierSummaryMeasureEnum[prop]);
            }
        }

        defineScope();
        load();

    }
    appControllers.controller('Carrier_CarrierSummaryStatsController', CarrierSummaryStatsController);

})(appControllers);