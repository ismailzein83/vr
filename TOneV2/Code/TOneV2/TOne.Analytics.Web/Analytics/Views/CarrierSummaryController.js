(function (appControllers) {

    "use strict";

    CarrierSummaryController.$inject = ['$scope', 'GenericAnalyticDimensionEnum', 'AnalyticsService', 'BusinessEntityAPIService_temp', 'ZonesService', 'CurrencyAPIService'];
    function CarrierSummaryController($scope, GenericAnalyticDimensionEnum, analyticsService, BusinessEntityAPIService, ZonesService, CurrencyAPIService) {

        var measureFieldsValues = [];
        load();

        function defineScope() {

            var now = new Date();
            $scope.fromDate = new Date(2013, 1, 1);
            $scope.toDate = now;
            $scope.subViewConnector = {};

            $scope.groupKeys = [];
            $scope.filterKeys = [];
            $scope.selectedfilters = [];


            for(var item in GenericAnalyticDimensionEnum) {
                if (GenericAnalyticDimensionEnum[item].name == "Customer")
                    $scope.groupKeys.push(GenericAnalyticDimensionEnum[item]);
            }

            for (var item in GenericAnalyticDimensionEnum) {
                if ((GenericAnalyticDimensionEnum[item].name == "Customer") ||
                    (GenericAnalyticDimensionEnum[item].name == "Supplier"))
                    $scope.filterKeys.push(GenericAnalyticDimensionEnum[item]);
            }

            analyticsService.getGenericAnalyticMeasureValues().forEach(function (item) {
                if (item == 0 || item == 1 || item == 2 || item == 3 || item == 4 || item == 5 || item == 6 || item == 7)
                    measureFieldsValues.push(item);
            });

            //$scope.selectedGroupKeys = [];
            //$scope.currentSearchCriteria = {
            //    groupKeys: []
            //};

            $scope.measures = [];
            $scope.selectedMeasures = [];

            //$scope.customers = [];
            //$scope.selectedCustomers = [];
            //$scope.suppliers = [];
            //$scope.selectedSuppliers = [];

            //$scope.optionsCurrencies = {
            //    selectedvalues: '',
            //    datasource: []
            //};

            loadMeasures();
           // loadCurrencies();

            $scope.searchClicked = function () {
                return retrieveData();
            };

            $scope.checkExpandablerow = function (groupKeys) {
                return groupKeys.length !== $scope.groupKeys.length;
            };

            $scope.groupSelectionChanged = function () {
                $scope.currentSearchCriteria.groupKeys.length = 0;
            };
        }

        function retrieveData() {

            console.log($scope.selectedfilters);

            //var filters = [];

            //var filterCustomer = {
            //    Dimension: GenericAnalyticDimensionEnum.Customer.value,
            //    FilterValues: []
            //};

            //var filterSupplier = {
            //    Dimension: GenericAnalyticDimensionEnum.Supplier.value,
            //    FilterValues: []
            //};

            //$scope.selectedCustomers.forEach(function (item) {
            //    filterCustomer.FilterValues.push(item.CarrierAccountID);
            //});
            //$scope.selectedSuppliers.forEach(function (item) {
            //    filterSupplier.FilterValues.push(item.CarrierAccountID);
            //});

            //if (filterCustomer.FilterValues.length > 0)
            //    filters.push(filterCustomer);
            //if (filterSupplier.FilterValues.length > 0)
            //    filters.push(filterSupplier);

            //var query = {
            //    Filters: filters,
            //    DimensionFields: $scope.selectedGroupKeys,
            //    MeasureFields: measureFieldsValues,
            //    FromTime: $scope.fromDate,
            //    ToTime: $scope.toDate,
            //    Currency: $scope.optionsCurrencies.selectedvalues == null ? null : $scope.optionsCurrencies.selectedvalues.CurrencyID
            //};
            //$scope.subViewConnector.retrieveData(query);
        }

        function load() {
            defineScope();
        }

        function loadMeasures() {

            analyticsService.getGenericAnalyticMeasures().forEach(function (item) {
                if (item.name == "FirstCDRAttempt" || item.name == "ABR" || item.name == "ASR" || item.name == "NER" || item.name == "Attempts" || item.name == "SuccessfulAttempts" || item.name == "FailedAttempts" || item.name == "DeliveredAttempts")
                    $scope.selectedMeasures.push(item);
            });
            $scope.measures = $scope.selectedMeasures;
        }

        //function loadCurrencies() {
        //    return CurrencyAPIService.GetCurrencies().then(function (response) {
        //        $scope.optionsCurrencies.datasource = response;
        //    });
        //}
    }
    appControllers.controller('Carrier_CarrierSummaryController', CarrierSummaryController);

})(appControllers);