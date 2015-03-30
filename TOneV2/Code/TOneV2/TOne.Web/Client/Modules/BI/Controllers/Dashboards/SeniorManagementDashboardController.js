appControllers.controller('SeniorManagementDashboardController',
    function SeniorManagementDashboardController($scope, $location, $modal, BIAPIService, BIEntityTypeEnum, BIMeasureTypeEnum) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {


            $scope.testModel = 'SeniorManagementDashboardController';
            $scope.dateTimeFilterOption = {
                datasource: [{ name: "Today", value: 0, fromDate: '2012-1-2', toDate: '2012-1-3' },
            { name: "WTD", value: 1, fromDate: '2012-1-2', toDate: '2012-02-9' },
            { name: "MTD", value: 2, fromDate: '2012-1-2', toDate: '2013-2-2' },
            { name: "YTD", value: 3, fromDate: '2011-1-2', toDate: '2014-1-1' }]
            };

            $scope.optionsMeasureTypes = {
                datasource: []
            };

            for (prop in BIMeasureTypeEnum) {
                var obj = {
                    name: BIMeasureTypeEnum[prop].description,
                    value: BIMeasureTypeEnum[prop].value
                };
                $scope.optionsMeasureTypes.datasource.push(obj);
            }
           

            $scope.optionTopCounts = {
                datasource: [{ value: 5 },
               {value: 10 },
               { value: 15 },
               { value: 20 }
                ]
            };

        }
        var chartTopDestinationsAPI;
        var chartTopCustomersAPI;
        var chartTopSuppliersAPI;
        function defineScopeMethods() {

            $scope.chartTopDestinationsReady = function (api) {
                chartTopDestinationsAPI = api;
                chartTopDestinationsAPI.onDataItemClicked = function (zoneItem) {
                   
                    //$scope.$root.$apply(function () {
                        $scope.zoneId = zoneItem.EntityId;
                        $scope.zoneName = zoneItem.EntityName;
                        var selectedDateTimeFilter = $scope.dateTimeFilterOption.lastselectedvalue;
                        $scope.fromDate = selectedDateTimeFilter.fromDate;
                        $scope.toDate = selectedDateTimeFilter.toDate;
                       var addModal = $modal({ scope: $scope, template: '/Client/Modules/BI/Views/Reports/ZoneSummary.html', show: true });
                      //  addModal.$promise.then(addModal.show);
                        //$location.path("/BI/ZoneDetails/" + zoneItem.EntityId + "/" + zoneItem.EntityName).replace();
                    //});
                   
                   
                };
                updateTopDestinationsChart();
            };

            $scope.chartTopCustomersReady = function (api) {
                chartTopCustomersAPI = api;
                updateTopCustomersChart();
            };

            $scope.chartTopSuppliersReady = function (api) {
                chartTopSuppliersAPI = api
                updateTopSuppliersChart();
            };

            $scope.updateCharts = function () {
                updateCharts();
            };
        }

        function load() {
            $scope.optionsMeasureTypes.lastselectedvalue = $scope.optionsMeasureTypes.datasource[0];
            $scope.dateTimeFilterOption.lastselectedvalue = $scope.dateTimeFilterOption.datasource[0];
            $scope.optionTopCounts.lastselectedvalue = $scope.optionTopCounts.datasource[1];
        }

        function updateCharts() {
            updateTopDestinationsChart();
            updateTopCustomersChart();
            updateTopSuppliersChart();
        }

        function updateTopDestinationsChart() {
            updateTopChart(chartTopDestinationsAPI, BIEntityTypeEnum.SaleZone.value, {
                chartTitle: "TOP DESTINATIONS",
                seriesTitle: "Top Destinations"
            });
        }

        function updateTopCustomersChart() {
            updateTopChart(chartTopCustomersAPI, BIEntityTypeEnum.Customer.value, {
                chartTitle: "TOP CUSTOMERS",
                seriesTitle: "Top Customers"
            });
        }

        function updateTopSuppliersChart() {
            updateTopChart(chartTopSuppliersAPI, BIEntityTypeEnum.Supplier.value, {
                chartTitle: "TOP SUPPLIERS",
                seriesTitle: "Top Suppliers"
            });
        }

        function updateTopChart(chartAPI, entityType, chartSettings) {
            if (!chartAPI)
                return;
            var measureType = $scope.optionsMeasureTypes.lastselectedvalue;
            if (measureType == undefined || measureType == null || measureType.length == 0)
                return;

            chartAPI.showLoader();
            var selectedDateTimeFilter = $scope.dateTimeFilterOption.lastselectedvalue;
            BIAPIService.GetTopEntities(entityType, measureType.value, selectedDateTimeFilter.fromDate, selectedDateTimeFilter.toDate, $scope.optionTopCounts.lastselectedvalue.value, [measureType.value])
                .then(function (response) {
                    var chartData = [];
                    angular.forEach(response, function (item) {
                        chartData.push(item);
                    });
                    
                    var chartDefinition = {
                        type: "pie",
                        title: chartSettings.chartTitle,
                        yAxisTitle: "Value",
                        showLegendsWithValues: $scope.showLegendsWithValues
                    };

                    var seriesDefinitions = [{
                        title: chartSettings.seriesTitle,
                        titlePath: "EntityName",
                        valuePath: "Values[0]"
                    }];

                    chartAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
                })
                .finally(function () {
                    chartAPI.hideLoader();
                });
        }
    });