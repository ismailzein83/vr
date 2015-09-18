(function (appControllers) {

    "use strict";

    GHourlyReportController.$inject = ['$scope', 'GenericAnalyticDimensionEnum', 'AnalyticsService', 'BusinessEntityAPIService_temp', 'ZonesService', 'CarrierAccountConnectionAPIService', 'CarrierTypeEnum'];
    function GHourlyReportController($scope, GenericAnalyticDimensionEnum, analyticsService, BusinessEntityAPIService, ZonesService, CarrierAccountConnectionAPIService, CarrierTypeEnum) {

        var measureFieldsValues = [];
        load();

        function defineScope() {

            var now = new Date();
            $scope.fromDate = new Date(2013, 1, 1);
            $scope.toDate = now;
            $scope.subViewConnector = {};



            $scope.groupKeys = [];

            analyticsService.getGenericAnalyticGroupKeys().forEach(function (item) {
                if (item.name == "Customer" || item.name == "Supplier" || item.name == "Zone" || item.name == "CodeGroup" || item.name == "Switch" || item.name == "GateWayIn" ||
                    item.name == "GateWayOut" || item.name == "PortIn" || item.name == "PortOut" || item.name == "CodeSales" || item.name == "CodeBuy" || item.name == "Hour" || item.name == "Date")
                    $scope.groupKeys.push(item);
            });

            analyticsService.getGenericAnalyticMeasureValues().forEach(function (item) {
                if (item == 2 || item == 3 || item == 5 || item == 6 || item == 7 || item == 8 || item == 14 || item == 15 || item == 19 || item == 20)
                    measureFieldsValues.push(item);
            });

            $scope.selectedGroupKeys = [];
            $scope.currentSearchCriteria = {
                groupKeys: []
            };

            $scope.zones = [];
            $scope.selectedZones = [];

            $scope.searchZones = function (text) {
                return ZonesService.getSalesZones(text);
            }

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

            $scope.connections = [];
            $scope.selectedConnections = [];
            $scope.onSelectionChanged = function () {

                var value;
                switch ($scope.selectedConnectionIndex) {
                    case 0: $scope.selectedConnections.length = 0; $scope.connections.length = 0; return;
                    case 1: value = CarrierTypeEnum.Customer.value;
                        break;
                    case 2: value = CarrierTypeEnum.Supplier.value;
                        break;
                }
                return CarrierAccountConnectionAPIService.GetConnectionByCarrierType(value).then(function (response) {
                    $scope.selectedConnections.length = 0;
                    $scope.connections.length = 0;
                    angular.forEach(response, function (itm) {
                        $scope.connections.push(itm);
                    });

                });
            }
            $scope.selectedConnectionIndex;


            loadSwitches();
            loadCodeGroups();
            loadMeasures();

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
            var filterSupplier = {
                Dimension: GenericAnalyticDimensionEnum.Supplier.value,
                FilterValues: []
            };
            var filterPortIn = {
                Dimension: GenericAnalyticDimensionEnum.PortIn.value,
                FilterValues: []
            };
            var filterPortOut = {
                Dimension: GenericAnalyticDimensionEnum.PortOut.value,
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
            $scope.selectedSuppliers.forEach(function (item) {
                filterSupplier.FilterValues.push(item.CarrierAccountID);
            });

            var connectionList = [];
            switch ($scope.selectedConnectionIndex) {
                case 1:
                    $scope.selectedConnections.forEach(function (item) {
                        filterPortIn.FilterValues.push(item.Value);
                    });
                    break;
                case 2:
                    $scope.selectedConnections.forEach(function (item) {
                        filterPortOut.FilterValues.push(item.Value);
                    });
                    break;
            }

            if (filterCodeGroup.FilterValues.length > 0)
                filters.push(filterCodeGroup);
            if (filterSwitch.FilterValues.length > 0)
                filters.push(filterSwitch);
            if (filterZone.FilterValues.length > 0)
                filters.push(filterZone);
            if (filterCustomer.FilterValues.length > 0)
                filters.push(filterCustomer);
            if (filterSupplier.FilterValues.length > 0)
                filters.push(filterSupplier);
            if (filterPortIn.FilterValues.length > 0)
                filters.push(filterPortIn);
            if (filterPortOut.FilterValues.length > 0)
                filters.push(filterPortOut);

            var query = {
                Filters: filters,
                DimensionFields: $scope.selectedGroupKeys,
                MeasureFields: measureFieldsValues,
                FromTime: $scope.fromDate,
                ToTime: $scope.toDate,
                Currency: null
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

            analyticsService.getGenericAnalyticMeasures().forEach(function (item) {
                if (item.name == "ASR" || item.name == "NER" || item.name == "SuccessfulAttempts" || item.name == "FailedAttempts" || item.name == "DeliveredAttempts" || 
                    item.name == "DurationsInSeconds" || item.name == "ACD" || item.name == "LastCDRAttempt"|| item.name == "GreenArea" || item.name == "GrayArea" )
                    $scope.selectedMeasures.push(item);
            });
            $scope.measures = $scope.selectedMeasures;
        }
    }
    appControllers.controller('Analytics_GHourlyReportController', GHourlyReportController);

})(appControllers);