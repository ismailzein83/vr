appControllers.controller('SeniorManagementDashboardController',
    function SeniorManagementDashboardController($scope, VRNavigationService, VRModalService, BIAPIService, BIEntityTypeEnum, BIMeasureTypeEnum, $animate) {

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

            $scope.chartTopDestinationMenuActions = [
                {
                    name: "Zone Summary",
                    clicked: function (zoneItem) {
                        var selectedDateTimeFilter = $scope.dateTimeFilterOption.lastselectedvalue;

                        var parameters = {
                            zoneId: zoneItem.EntityId,
                            fromDate: selectedDateTimeFilter.fromDate,
                            toDate: selectedDateTimeFilter.toDate
                        }
                        var modalSettings = {
                            useModalTemplate: true
                        };
                        modalSettings.onScopeReady = function (modalScope) {
                            modalScope.title = zoneItem.EntityName;
                        };
                        var modalScope = VRModalService.showModal('/Client/Modules/BI/Views/Reports/ZoneSummary.html', parameters, modalSettings);
                    }
                },
                {
                    name: "Zone Details",
                    clicked: function (zoneItem) {
                        var parameters = {
                            zoneId: zoneItem.EntityId,
                            zoneName: zoneItem.EntityName
                        };
                        VRNavigationService.goto("/BI/ZoneDetails", parameters);
                    }
                }
            ];
        }
        var chartTopDestinationsAPI;
        var chartTopCustomersAPI;
        var chartTopSuppliersAPI;
        function defineScopeMethods() {

            $scope.chartTopDestinationsReady = function (api) {
                chartTopDestinationsAPI = api;
                updateCharts();
            };

            $scope.chartTopCustomersReady = function (api) {
                chartTopCustomersAPI = api;
                updateCharts();
            };

            $scope.chartTopSuppliersReady = function (api) {
                chartTopSuppliersAPI = api
                updateCharts();
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
            if (chartTopDestinationsAPI == undefined || chartTopCustomersAPI == undefined || chartTopSuppliersAPI == undefined)
                return;
            var finishedTasks = 0;
            var taskHandle = {
                operationDone: function () {
                    finishedTasks++;
                    if (finishedTasks == 3) {
                        if (asyncHandle)
                            asyncHandle.operationDone();
                        $scope.isGettingData = false;
                    }
                }
            };
            $scope.isGettingData = true;
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

            var selectedDateTimeFilter = $scope.dateTimeFilterOption.lastselectedvalue;
            BIAPIService.GetTopEntities(entityType, measureType.value, selectedDateTimeFilter.fromDate, selectedDateTimeFilter.toDate, $scope.optionTopCounts.lastselectedvalue.value, [measureType.value])
                .then(function (response) {
                    var chartData = [];
                    angular.forEach(response, function (item) {
                        chartData.push(item);
                    });
                    
                    var chartDefinition = {
                        type: "pie",
                     //   title: chartSettings.chartTitle,
                        yAxisTitle: "Value"
                    };

                    var seriesDefinitions = [{
                        title: measureType.name,//chartSettings.seriesTitle,
                        titlePath: "EntityName",
                        valuePath: "Values[0]"
                    }];

                    chartAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
                })
                .finally(function () {
                    if (asyncHandle)
                        asyncHandle.operationDone();
                });
        }
    });