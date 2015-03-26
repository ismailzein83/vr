﻿appControllers.controller('SeniorManagementDashboardController',
    function SeniorManagementDashboardController($scope, $location, $modal, BIAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {


            $scope.testModel = 'SeniorManagementDashboardController';
            $scope.dateTimeFilters = [{ name: "Today", value: 0, fromDate: '2012-1-2', toDate: '2012-1-3' },
            { name: "WTD", value: 1, fromDate: '2012-1-2', toDate: '2012-02-9' },
            { name: "MTD", value: 2, fromDate: '2012-1-2', toDate: '2013-2-2' },
            { name: "YTD", value: 3, fromDate: '2011-1-2', toDate: '2014-1-1' }, ];

            $scope.measureTypes = [{ name: "Duration In Minutes", value: 0 },
            { name: "Sale", value: 1 },
            { name: "Cost", value: 2 }
            ];

            $scope.topCounts = [{ name: "5", value: 5 },
           { name: "10", value: 10 },
           { name: "15", value: 15 }
            ];

        }
        var chartTopDestinationsAPI;
        var chartTopCustomersAPI;
        var chartTopSuppliersAPI;
        function defineScopeMethods() {

            $scope.chartTopDestinationsReady = function (api) {
                chartTopDestinationsAPI = api;
                chartTopDestinationsAPI.onDataItemClicked = function (zoneItem) {
                    console.log("/BI/ZoneDetails/" + zoneItem.EntityId);
                    $scope.$root.$apply(function () {
                        $scope.zoneId = zoneItem.EntityId;
                        $scope.zoneName = zoneItem.EntityName;
                        addModal = $modal({ scope: $scope, template: '/Client/Modules/BI/Views/Reports/ZoneDetails.html', show: false });
                        addModal.$promise.then(addModal.show);
                        //$location.path("/BI/ZoneDetails/" + zoneItem.EntityId + "/" + zoneItem.EntityName).replace();
                    });
                   
                    console.log('not gone');
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
                updateTopDestinationsChart();
                updateTopCustomersChart();
                updateTopSuppliersChart();
            };
        }

        function load() {
            $scope.selectedDateTimeFilter = $scope.dateTimeFilters[0];
            $scope.selectedMeasureType = $scope.measureTypes[0];
            $scope.selectedTopCount = $scope.topCounts[1];
        }

        function updateTopDestinationsChart() {
            updateTopChart(chartTopDestinationsAPI, 0, {
                chartTitle: "TOP DESTINATIONS",
                seriesTitle: "Top Destinations"
            });
        }

        function updateTopCustomersChart() {
            updateTopChart(chartTopCustomersAPI, 1, {
                chartTitle: "TOP CUSTOMERS",
                seriesTitle: "Top Customers"
            });
        }

        function updateTopSuppliersChart() {
            updateTopChart(chartTopSuppliersAPI, 2, {
                chartTitle: "TOP SUPPLIERS",
                seriesTitle: "Top Suppliers"
            });
        }

        function updateTopChart(chartAPI, entityType, chartSettings) {
           
            chartAPI.showLoader();
            BIAPIService.GetTopEntities(entityType, $scope.selectedMeasureType.value, $scope.selectedDateTimeFilter.fromDate, $scope.selectedDateTimeFilter.toDate, $scope.selectedTopCount.value)
                .then(function (response) {

                    var chartData = response;
                    var chartDefinition = {
                        type: "pie",
                        title: chartSettings.chartTitle,
                        yAxisTitle: "Value",
                        showLegendsWithValues: $scope.showLegendsWithValues
                    };

                    var seriesDefinitions = [{
                        title: chartSettings.seriesTitle,
                        titleFieldName: "EntityName",
                        valueFieldName: "Value"
                    }];

                    chartAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
                })
                .finally(function () {
                    chartAPI.hideLoader();
                });
        }

    });