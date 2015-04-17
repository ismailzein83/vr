appControllers.controller('SeniorManagementDashboardController',
    function SeniorManagementDashboardController($scope, $location, $modal, BIAPIService, BIEntityTypeEnum, BIMeasureTypeEnum, $animate) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {


            $scope.testModel = 'SeniorManagementDashboardController';
            $scope.dateTimeFilterOption = {
                datasource: [{ name: "Today", value: 0, fromDate: '2015-3-30', toDate: '2015-3-30' },
            { name: "WTD", value: 1, fromDate: '2015-3-23', toDate: '2015-3-30' },
            { name: "MTD", value: 2, fromDate: '2015-3-01', toDate: '2015-3-31' },
            { name: "YTD", value: 3, fromDate: '2014-3-31', toDate: '2015-3-31' }]
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
                        var addModal = $modal({ scope: $scope, template: '/Client/Modules/BI/Views/Reports/ZoneSummary.html', show: true, animation: "am-fade-and-scale" });
                       // $animate.enabled(true);
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

            $scope.updateCharts = function (asyncHandle) {
                updateCharts(asyncHandle);
            };
        }

        function load() {
           
            $scope.optionsMeasureTypes.lastselectedvalue = $scope.optionsMeasureTypes.datasource[0];
            $scope.dateTimeFilterOption.lastselectedvalue = $scope.dateTimeFilterOption.datasource[0];
            $scope.optionTopCounts.lastselectedvalue = $scope.optionTopCounts.datasource[1];
        }

        function updateCharts(asyncHandle) {
            var finishedTasks = 0;
            var taskHandle = {
                operationDone: function () {
                    finishedTasks++;
                    if (finishedTasks == 3)
                        if (asyncHandle)
                            asyncHandle.operationDone();
                }
            };
            updateTopDestinationsChart(taskHandle);
            updateTopCustomersChart(taskHandle);
            updateTopSuppliersChart(taskHandle);
        }

        function updateTopDestinationsChart(asyncHandle) {
            updateTopChart(chartTopDestinationsAPI, BIEntityTypeEnum.SaleZone.value, {
                chartTitle: "TOP DESTINATIONS",
                seriesTitle: "Top Destinations"
            }, asyncHandle);
        }

        function updateTopCustomersChart(asyncHandle) {
            updateTopChart(chartTopCustomersAPI, BIEntityTypeEnum.Customer.value, {
                chartTitle: "TOP CUSTOMERS",
                seriesTitle: "Top Customers"
            }, asyncHandle);
        }

        function updateTopSuppliersChart(asyncHandle) {
            updateTopChart(chartTopSuppliersAPI, BIEntityTypeEnum.Supplier.value, {
                chartTitle: "TOP SUPPLIERS",
                seriesTitle: "Top Suppliers"
            }, asyncHandle);
        }

        function updateTopChart(chartAPI, entityType, chartSettings, asyncHandle) {
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
                        yAxisTitle: "Value"
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
                    if (asyncHandle)
                        asyncHandle.operationDone();
                });
        }
    });