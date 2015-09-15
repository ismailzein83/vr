(function (appControllers) {

    "use strict";

    GenericAnalyticController.$inject = ['$scope','GenericAnalyticDimensionEnum', 'AnalyticsService', 'BusinessEntityAPIService_temp', 'ZonesService', 'CurrencyAPIService'];
    function GenericAnalyticController($scope, GenericAnalyticDimensionEnum, analyticsService, BusinessEntityAPIService, ZonesService, CurrencyAPIService) {

        var measureFields;
        load();

        function defineScope() {

            var now = new Date();
            $scope.fromDate = new Date(2013, 1, 1);
            $scope.toDate = now;
            $scope.subViewConnector = {};
            
            measureFields = analyticsService.getGenericAnalyticMeasureValues();
            $scope.groupKeys = analyticsService.getGenericAnalyticGroupKeys();

            $scope.selectedGroupKeys = [];
            $scope.currentSearchCriteria = {
                groupKeys: []
            };

            $scope.zones = [];
            $scope.selectedZones = [];

            $scope.searchZones = function (text) {
                return ZonesService.getSalesZones(text);
            }

            $scope.optionsCurrencies = {
                selectedvalues: '',
                datasource: []
            };

            $scope.switches = [];
            $scope.selectedSwitches = [];

            $scope.codeGroups = [];
            $scope.selectedCodeGroups = [];

            $scope.measures = [];
            $scope.selectedMeasures = [];

            $scope.customers = [];
            $scope.selectedCustomers = [];
            $scope.suppliers = [];
            $scope.selectedSuppliers = [];


            loadSwitches();
            loadCodeGroups();
            loadMeasures();
            loadCurrencies();

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

            var filters = [];

            var filterCodeGroup = {
                Dimension: GenericAnalyticDimensionEnum.CodeGroup.value,
                FilterValues: []
            };
            var filterSwitch = {
                Dimension: GenericAnalyticDimensionEnum.Switch.value,
                FilterValues: []
            };
            var filterZone = {
                Dimension: GenericAnalyticDimensionEnum.Zone.value,
                FilterValues: []
            };
            var filterCustomer = {
                Dimension: GenericAnalyticDimensionEnum.Customer.value,
                FilterValues: []
            };


            $scope.selectedCodeGroups.forEach(function (item) {
                filterCodeGroup.FilterValues.push(item.Code);
            });
            $scope.selectedSwitches.forEach(function (item) {
                filterSwitch.FilterValues.push(item.SwitchId);
            });
            $scope.selectedZones.forEach(function (item) {
                filterZone.FilterValues.push(item.ZoneId);
            });
            $scope.selectedCustomers.forEach(function (item) {
                filterCustomer.FilterValues.push(item.CarrierAccountID);
            });

            if (filterCodeGroup.FilterValues.length > 0)
                filters.push(filterCodeGroup);
            if (filterSwitch.FilterValues.length > 0)
                filters.push(filterSwitch);
            if (filterZone.FilterValues.length > 0)
                filters.push(filterZone);
            if (filterCustomer.FilterValues.length > 0)
                filters.push(filterCustomer);

            var groupKeys = [];
            $scope.selectedGroupKeys.forEach(function (group) {
                groupKeys.push(group.value);
            });

            var query = {
                Filters: filters,
                DimensionFields: $scope.selectedGroupKeys,
                MeasureFields: measureFields,
                FromTime: $scope.fromDate,
                ToTime: $scope.toDate,
                Currency: $scope.optionsCurrencies.selectedvalues == null ? null : $scope.optionsCurrencies.selectedvalues.CurrencyID
            };
            $scope.subViewConnector.retrieveData(query);
        }

        function load() {
            defineScope();
        }

        function loadSwitches() {
            return BusinessEntityAPIService.GetSwitches().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.switches.push(itm);
                });
            });
        }

        function loadCodeGroups() {
            return BusinessEntityAPIService.GetCodeGroups().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.codeGroups.push(itm);
                });
            });
        }

        function loadMeasures() {
            return $scope.measures = $scope.selectedMeasures = analyticsService.getGenericAnalyticMeasures();
        }

        function loadCurrencies() {
            return CurrencyAPIService.GetCurrencies().then(function (response) {
                $scope.optionsCurrencies.datasource = response;
            });
        }
    }
    appControllers.controller('Generic_GenericAnalyticController', GenericAnalyticController);

})(appControllers);