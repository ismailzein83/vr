/// <reference path="ZoneMonitorSettings.html" />
/// <reference path="ZoneMonitor.html" />
appControllers.controller('ZoneMonitorController',
    function ZoneMonitorController($scope, AnalyticsAPIService, uiGridConstants, $q, BusinessEntityAPIService, CarrierTypeEnum) {


        var chartSelectedMeasureAPI;
        var chartSelectedEntityAPI;
        var overallGridAPI;
        var resultKey;
        var sortColumn;
        var sortDescending = true;
        var measures;
        var currentSortedColDef;
        var trafficStatisticSummary;

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

            $scope.optionsTopCount = {
                datasource: [
                { description: "5", value: 5 },
                { description: "10", value: 10 },
                { description: "15", value: 15 }
                ]
            };
            $scope.optionsTopCount.lastselectedvalue = $scope.optionsTopCount.datasource[1];

            $scope.switches = [];
            $scope.selectedSwitches = [];

            $scope.codeGroups = [];
            $scope.selectedCodeGroups = [];

            $scope.customers = [];
            $scope.selectedCustomers = [];
           
            $scope.suppliers = [];
            $scope.selectedSuppliers = [];

            $scope.gridOptionsAllMeasures = {};
            $scope.gridOptionsAllMeasures.useExternalSorting = true;
            $scope.gridOptionsAllMeasures.enableGridMenu = true;
            $scope.gridOptionsAllMeasures.data = [];
            $scope.gridOptionsAllMeasures.onRegisterApi = function (gridApi) {
              
                gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                   
                    if (sortColumns.length > 0) {
                        var measure = $.grep(measures, function (m, index) {
                            // num = the current value for the item in the array
                            // index = the index of the item in the array
                            return m.description == sortColumns[0].field; // returns a boolean
                        })[0]; 
                        sortColumn = measure;
                       
                        console.log(sortColumns[0].sort.direction);
                        switch (sortColumns[0].sort.direction) {
                            case uiGridConstants.ASC:
                                sortDescending = false;
                                break;
                            case uiGridConstants.DESC:
                                sortDescending = true;
                                break;
                            case undefined:
                                sortDescending = false;
                                break;
                        }
                        if (currentSortedColDef != undefined)
                            currentSortedColDef.currentSorting = undefined;
                        currentSortedColDef = sortColumns[0].colDef;
                        
                    }
                    getData();
                });

            };

            $scope.totalDataCount = 0;
            $scope.currentPage = 1;

            $scope.gridAllMeasuresScope = {};

            

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
            $scope.toggleHeader = function (e) {
               alert("in")
                // console.log(e);
            };
            $scope.getData = function (asyncHandle) {
                $scope.currentPage = 1;
                resultKey = null;
                getData(asyncHandle, true);
            };
            
            $scope.pageChanged = function () {
                getData();
            };

            $scope.getGridHeight = function (gridOptions) {
                var height;
                if (gridOptions.data.length == 0) {
                    height = gridOptions.lastHeight;
                }
                else {
                    var rowHeight = 30; // your row height
                    var headerHeight = 30; // your header height
                    var height = (gridOption.data.length * rowHeight + headerHeight);
                }
                gridOptions.lastHeight = height;
                
                return {
                    height: height + "px"
                };
            };

            $scope.gridCellClicked = function (entity) {
                $scope.selectedEntityId = entity.GroupKeyValues[0].Id;
                $scope.selectedEntityName = entity.GroupKeyValues[0].Name;
                console.log($scope.selectedEntityName);
                getAndShowEntityStatistics();
            };

            $scope.onOverallGridReady = function (api) {
                overallGridAPI = api;
                defineOverallGrid();
            }
        }

        function load() {
            $scope.fromDate = '2014-04-27';
            $scope.toDate = '2014-04-29';
            loadMeasureTypes();
            loadCodeGroups();
            loadSwitches();
            loadCustomers();
            loadSuppliers();
        }



        function defineGrid()
        {
            if (measures == undefined || measures.length == 0)
                return;
            gridOption = $scope.gridOptionsAllMeasures;
            gridOption.enableHorizontalScrollbar = 0;
            gridOption.enableVerticalScrollbar = 0;
            //gridOption.minRowsToShow = 30;
            //gridOption.enableFiltering = false;
            //gridOption.saveFocus = false;
            //gridOption.saveScroll = true;
            gridOption.enableColumnResizing = true;
            gridOption.gridCellClicked = function (entity) {
                console.log("gridOption gridCellClicked");
                $scope.selectedEntityId = entity.GroupKeyValues[0].Id;
                $scope.selectedEntityName = entity.GroupKeyValues[0].Name;
                console.log($scope.selectedEntityName);
                getAndShowEntityStatistics();
            };

            gridOption.columnDefs = [];
            var zoneColumn = {
                name: 'Zone',
                headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',//template,
                enableColumnMenu: false,
                enableHiding: false,
                field: 'zoneName',
                cellTemplate: '/Client/Templates/Grid/ClickableCellTemplate.html'
            };
            gridOption.columnDefs.push(zoneColumn);
            var valColumnIndex = 0;
            angular.forEach(measures, function (measureType) {
                var colDef = {
                    name: measureType.description,
                    headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',//template,
                    enableColumnMenu: false,
                    field: measureType.description,
                    sort: {
                        direction: uiGridConstants.DESC,
                        priority: 1
                    },
                    cellFilter: "number:2"
                };
                gridOption.columnDefs.push(colDef);
            });
        }

        function defineOverallGrid() {
            columns = [];
            var firstColumn = {
                headerText: 'Measure',
                field: 'measure',
                isClickable: function (itm) {
                    return itm.measure == "Attempts" || itm.measure == "DeliveredAttempts"
                        || itm.measure == "SuccessfulAttempts" || itm.measure == "DurationsInSeconds" || itm.measure == "UtilizationInSeconds" || itm.measure == "NumberOfCalls" || itm.measure == "DeliveredNumberOfCalls";
                }
            };
            columns.push(firstColumn);

            var secondColumn = {
                headerText: 'Value',
                field: 'value',
                type: "Number"
            };
            columns.push(secondColumn);
            var gridOptions = {
                hideHeader: true,                
                columns: columns
            };
            overallGridAPI.defineGrid(gridOptions);
        }
        
        function getData(asyncHandle, withSummary) {
            if (!chartSelectedMeasureAPI)
                return;
            if (withSummary == undefined)
                withSummary = false;
            $scope.showResult = true;
            $scope.gridOptionsAllMeasures.data.length = 0;
            chartSelectedMeasureAPI.showLoader();
            if (chartSelectedEntityAPI)
                chartSelectedEntityAPI.hideChart();

            var count = $scope.optionsTopCount.lastselectedvalue.value;
           
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

            AnalyticsAPIService.GetTrafficStatisticSummary(getTrafficStatisticSummaryInput).then(function (response) {

                if (currentSortedColDef != undefined)
                    currentSortedColDef.currentSorting = sortDescending ? 'DESC' : 'ASC';

                resultKey = response.ResultKey;
                $scope.totalDataCount = response.TotalCount;
                if (withSummary) {
                    trafficStatisticSummary = response.Summary;
                    overallGridAPI.data.length = 0;
                    angular.forEach(measures, function (measureType) {
                        overallGridAPI.data.push({
                            measure: measureType.description,
                            value: trafficStatisticSummary[measureType.description],
                            clicked: function () {
                                renderOverallChart(response.Data, measureType.description);
                            }
                        });
                    });
                }
                angular.forEach(response.Data, function (itm) {
                    itm.clicked = function () {
                        console.log('itm.clicked');
                        $scope.selectedEntityId = itm.GroupKeyValues[0].Id;
                        $scope.selectedEntityName = itm.GroupKeyValues[0].Name;
                        console.log($scope.selectedEntityName);
                        getAndShowEntityStatistics();
                    };
                    itm.zoneName = itm.GroupKeyValues[0].Name;
                    $scope.gridOptionsAllMeasures.data.push(itm);
                });
                renderOverallChart(response.Data, 'Attempts');
                $scope.filterSectionCollapsed = true;
            })
                .finally(function () {
                    chartSelectedMeasureAPI.hideLoader();
                    if (asyncHandle)
                        asyncHandle.operationDone();
                });
        }

        function renderOverallChart(data, measure){
            var chartData = [];
            var othersValue = trafficStatisticSummary[measure];
            angular.forEach(data, function (itm) {    
                chartData.push({
                    zoneId: itm.GroupKeyValues[0].Id,
                    zoneName: itm.GroupKeyValues[0].Name,
                    value: itm[measure]
                });
                othersValue -= itm[measure];
            });
            chartData.push({
                zoneName: "Other Destinations",
                value: othersValue
            });
            var chartDefinition = {
                type: "pie",
                title: "Overall " + measure,
                yAxisTitle: "Value"
            };

            var seriesDefinitions = [{
                title: measure,
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
                for (var i = 0; i < measures.length; i++) {
                    var measure = measures[i];
                    seriesDefinitions.push({
                        title: measure.description,
                        valuePath: measure.description
                    });
                }

                chartSelectedEntityAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
            })
            .finally(function () {
                chartSelectedEntityAPI.hideLoader();
            });;
        }

        function loadMeasureTypes() {
            AnalyticsAPIService.GetTrafficStatisticMeasureList().then(function (response) {
                measures = [];
                angular.forEach(response, function (itm) {                   
                    measures.push({
                        value: itm.Value,
                        description: itm.Description
                    });
                });

                sortColumn = measures[2];
                defineGrid();
            });
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