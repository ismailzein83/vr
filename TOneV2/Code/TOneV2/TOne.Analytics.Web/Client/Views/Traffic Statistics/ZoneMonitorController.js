/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
appControllers.controller('ZoneMonitorController',
    function ZoneMonitorController($scope, AnalyticsAPIService, uiGridConstants, $q, BusinessEntityAPIService, TrafficStatisticsMeasureEnum, CarrierTypeEnum) {


        var chartSelectedMeasureAPI;
        var chartSelectedEntityAPI;
        var mainGridAPI;
        var overallSelectedMeasure;
        var resultKey;
        var sortColumn;
        var sortDescending = true;
        var currentSortedColDef;
        var trafficStatisticSummary;
        var measures = [];
        var currentData;

        defineScopeObjects();
        defineScopeMethods();
        load();
        function defineScopeObjects() {
            $scope.customvalidateTestFrom = function (fromDate) {
                return validateDates(fromDate, $scope.toDate);
            };
            $scope.customvalidateTestTo = function (toDate) {
                return validateDates($scope.fromDate, toDate);
            };
            function validateDates(fromDate, toDate) {
                if (fromDate == undefined || toDate == undefined)
                    return null;
                var from = new Date(fromDate);
                var to = new Date(toDate);
                if (from.getTime() > to.getTime())
                    return "Start should be before end";
                else
                    return null;
            }
            $scope.testModel = 'ZoneMonitorController';

            $scope.topCounts = [5, 10, 15];
            $scope.selectedTopCount = $scope.topCounts[1];

            $scope.switches = [];
            $scope.selectedSwitches = [];

            $scope.codeGroups = [];
            $scope.selectedCodeGroups = [];

            $scope.customers = [];
            $scope.selectedCustomers = [];
           
            $scope.suppliers = [];
            $scope.selectedSuppliers = [];

            $scope.totalDataCount = 0;
            $scope.currentPage = 1;

            $scope.gridAllMeasuresScope = {};
            $scope.showResult = false;
            $scope.measures = measures;
            $scope.data = [];
            $scope.overallData = [];
        }
        
        function defineScopeMethods() {

            $scope.chartSelectedMeasureReady = function (api) {
                chartSelectedMeasureAPI = api;
                chartSelectedMeasureAPI.onDataItemClicked = function (selectedEntity) {
                    if (selectedEntity.zoneId == undefined)
                        return;
                    $scope.selectedEntityId = selectedEntity.zoneId;
                    $scope.selectedEntityName = selectedEntity.zoneName;
                    //console.log($scope.selectedEntityName);
                    getAndShowEntityStatistics();
                };
            };

            $scope.chartSelectedEntityReady = function (api) {
                chartSelectedEntityAPI = api;
                
            };

            $scope.getData = function (asyncHandle) {
                $scope.currentPage = 1;
                resultKey = null;
                mainGridAPI.reset();
                resetSorting();
                getData(asyncHandle, true);
            };
            
            $scope.pageChanged = function () {
                getData();
            };
            
            $scope.onMainGridReady = function (api) {
                mainGridAPI = api;
            }

            $scope.onZoneClicked = function (dataItem) {
                selectZone(dataItem.GroupKeyValues[0].Id, dataItem.GroupKeyValues[0].Name);
            };

            $scope.onMainGridSortChanged = function (colDef, sortDirection, handle) {
                sortColumn = colDef.tag;
                sortDescending = (sortDirection == "DESC");
                getData(handle);
            }

            $scope.isOverallItemClickable = function (dataItem) {
                return (dataItem.measure.isSum == true);
            };

            $scope.onOverallItemClicked = function (dataItem) {
                overallSelectedMeasure = dataItem.measure;
                renderOverallChart();
            };
        }

        function load() {
            loadMeasures();
            overallSelectedMeasure = TrafficStatisticsMeasureEnum.Attempts;
            $scope.fromDate = '2014-04-27';
            $scope.toDate = '2014-04-29';
            loadCodeGroups();
            loadSwitches();
            loadCustomers();
            loadSuppliers();
        }

        function selectZone(zoneId, zoneName) {
            $scope.selectedEntityId = zoneId;
            $scope.selectedEntityName = zoneName;
            getAndShowEntityStatistics();
        }

        function resetSorting() {
            sortColumn = TrafficStatisticsMeasureEnum.Attempts;
            sortDescending = true;
        }

        function loadMeasures() {           
            for (var prop in TrafficStatisticsMeasureEnum) {
                measures.push(TrafficStatisticsMeasureEnum[prop]);
            }
        }
        
        function getData(asyncHandle, withSummary) {
            if (!chartSelectedMeasureAPI)
                return;
            if (withSummary == undefined)
                withSummary = false;
            
            $scope.data.length = 0;
            chartSelectedMeasureAPI.showLoader();
            if (chartSelectedEntityAPI)
                chartSelectedEntityAPI.hideChart();

            var count = $scope.selectedTopCount;
           
            var fromRow = ($scope.currentPage - 1) * count + 1;
            var toRow = fromRow + count - 1;
            var filter = buildFilter();
            var getTrafficStatisticSummaryInput = {
                TempTableKey: resultKey,
                Filter: filter,
                WithSummary: withSummary,
                GroupKeys: [4],
                From: $scope.fromDate,
                To: $scope.toDate,
                FromRow: fromRow,
                ToRow: toRow,
                OrderBy: sortColumn.value,
                IsDescending: sortDescending
            };
            var isSucceeded;
            $scope.showResult = true;
            AnalyticsAPIService.GetTrafficStatisticSummary(getTrafficStatisticSummaryInput).then(function (response) {

                currentData = response.Data;
                if (currentSortedColDef != undefined)
                    currentSortedColDef.currentSorting = sortDescending ? 'DESC' : 'ASC';

                resultKey = response.ResultKey;
                $scope.totalDataCount = response.TotalCount;
                if (withSummary) {
                    trafficStatisticSummary = response.Summary;
                    $scope.overallData.length = 0;
                    angular.forEach(measures, function (measure) {
                        $scope.overallData.push({
                            measure: measure,
                            measureDescription: measure.description,
                            value: trafficStatisticSummary[measure.propertyName]
                        });
                    });
                }
                angular.forEach(response.Data, function (itm) {
                    itm.zoneName = itm.GroupKeyValues[0].Name;
                    $scope.data.push(itm);
                });
                renderOverallChart();
                isSucceeded = true;
            })
                .finally(function () {
                    chartSelectedMeasureAPI.hideLoader();
                    if (asyncHandle)
                        asyncHandle.operationDone(isSucceeded);
                });
        }

        function renderOverallChart(){
            var chartData = [];
            var measure = overallSelectedMeasure;
            var othersValue = trafficStatisticSummary[measure.propertyName];
            angular.forEach(currentData, function (itm) {
                chartData.push({
                    zoneId: itm.GroupKeyValues[0].Id,
                    zoneName: itm.GroupKeyValues[0].Name,
                    value: itm[measure.propertyName]
                });
                othersValue -= itm[measure.propertyName];
            });
            chartData.sort(function (a, b) {
                if (a.value > b.value) {
                    return 1;
                }
                if (a.value < b.value) {
                    return -1;
                }
                // a must be equal to b
                return 0;
            });
            chartData.push({
                zoneName: "Other Destinations",
                value: othersValue
            });
            var chartDefinition = {
                type: "pie",
                title: measure.description,
                yAxisTitle: "Value"
            };

            var seriesDefinitions = [{
                title: measure.description,
                titlePath: "zoneName",
                valuePath: "value"
            }];
            //console.log(chartData.length);
            chartSelectedMeasureAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
        }

        function buildFilter() {
            var filter = {};
            filter.SwitchIds = getFilterIds($scope.selectedSwitches, "SwitchId");
            filter.CustomerIds = getFilterIds($scope.selectedCustomers, "CarrierAccountID");
            filter.SupplierIds = getFilterIds($scope.selectedSuppliers, "CarrierAccountID");
            filter.CodeGroups = getFilterIds($scope.selectedCodeGroups, "Code");
            return filter;
        }

        function getFilterIds(values, idProp) {
            var filterIds;
            if (values.length > 0) {
                filterIds = [];
                angular.forEach(values, function (val) {
                    filterIds.push(val[idProp]);
                });
            }
            return filterIds;
        }

        function getAndShowEntityStatistics() {
           
            chartSelectedEntityAPI.showLoader();
            AnalyticsAPIService.GetTrafficStatistics(4, $scope.selectedEntityId, $scope.fromDate, $scope.toDate)
            .then(function (response) {
                var chartData = response;

                var chartDefinition = {
                    type: "spline",
                    title: $scope.selectedEntityName
                };
                var xAxisDefinition = {
                    titlePath: "FirstCDRAttempt",
                    isDateTime: true
                };
                var seriesDefinitions = [];
                angular.forEach(measures, function (measure) {
                    seriesDefinitions.push({
                        title: measure.description,
                        valuePath: measure.propertyName,
                        selected: (measure == TrafficStatisticsMeasureEnum.Attempts || measure == TrafficStatisticsMeasureEnum.DurationsInMinutes)
                    });
                });

                chartSelectedEntityAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
            })
            .finally(function () {
                chartSelectedEntityAPI.hideLoader();
            });;
        }

        function loadSwitches() {
            BusinessEntityAPIService.GetSwitches().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.switches.push(itm);
                });
            });
        }

        function loadCodeGroups()
        {            
            BusinessEntityAPIService.GetCodeGroups().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.codeGroups.push(itm);
                });
            });

        }

        function loadCustomers() {
            BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.customers.push(itm);
                });
            });
        }

        function loadSuppliers() {
            BusinessEntityAPIService.GetCarriers(CarrierTypeEnum.Supplier.value).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.suppliers.push(itm);
                });
            });
        }
        
    });