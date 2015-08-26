(function (appControllers) {

    "use strict";

    CarrierSummaryStatsController.$inject = ['$scope', 'CarrierSummaryStatsAPIService', 'CarrierAccountAPIService', 'ZoneAPIService', 'CurrencyAPIService', 'CarrierTypeEnum', 'VRModalService'];
    function CarrierSummaryStatsController($scope, CarrierSummaryStatsAPIService, CarrierAccountAPIService, ZoneAPIService, CurrencyAPIService, CarrierTypeEnum, VRModalService) {

        var gridApi;

        function retrieveData() {
            
            $scope.getHeader = $scope.optionsGroups.selectedvalues.value == 3 ? "Zone" : $scope.byProfile ? "Profile" : "Carrier";

            $scope.optionsGroups.selectedvalues

            if ($scope.selectedvalues == undefined)
                $scope.selectedvalues = null;

            if ($scope.optionsCurrencies.selectedvalues == undefined)
                $scope.optionsCurrencies.selectedvalues = null;

            return gridApi.retrieveData({
                //CarrierType: $scope.isCustomer,
                CarrierType: $scope.optionsGroups.selectedvalues.value,
                CustomerID: $scope.selectedvalues == null ? null : $scope.selectedvalues.CarrierAccountID,
                ZoneID: $scope.selectedvaluesZones == null ? null : $scope.selectedvaluesZones.ZoneId,
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
            $scope.datasource = [];
            $scope.selectedvaluesZones = [];

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

            loadCarriers();
            loadCurrencies();
        }

        function loadCarriers() {
            return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.SaleZone.value,false).then(function (response) {
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

        //$scope.datasourceZones = function (text) {
        //    return ZoneAPIService.GetOwnZones(text);
        //}


        //function loadZones(text) {
        //    return BusinessEntityAPIService.GetOwnZones(text).then(function (response) {
        //        $scope.datasourceZones = response;
        //    });

        //}

        defineScope();
        load();
    }
    appControllers.controller('Carrier_CarrierSummaryStatsController', CarrierSummaryStatsController);

})(appControllers);