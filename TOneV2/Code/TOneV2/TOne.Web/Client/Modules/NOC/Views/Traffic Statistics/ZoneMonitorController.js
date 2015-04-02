appControllers.controller('ZoneMonitorController',
    function ZoneMonitorController($scope, AnalyticsAPIService, uiGridConstants) {


        var chartSelectedMeasureAPI;
        var chartSelectedEntityAPI;
        var resultKey;
        var sortColumn;
        var sortDescending = true;
        var measures;

        defineScopeObjects();
        defineScopeMethods();
        load();
        function defineScopeObjects() {
            $scope.testModel = 'ZoneMonitorController';

            $scope.optionsTopCount = {
                datasource: [
                { description: "5", value: 5 },
                { description: "10", value: 10 },
                { description: "15", value: 15 }
                ]
            };
            $scope.optionsTopCount.lastselectedvalue = $scope.optionsTopCount.datasource[1];
            
            $scope.gridOptionsAllMeasures = {};
            $scope.gridOptionsAllMeasures.useExternalSorting = true;
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
                    }
                    getData();
                });
            };

            $scope.totalDataCount = 0;
            $scope.currentPage = 1;

        }
        
        function defineScopeMethods() {

            $scope.chartSelectedMeasureReady = function (api) {
                chartSelectedMeasureAPI = api;
            };

            $scope.chartSelectedEntityReady = function (api) {
                chartSelectedEntityAPI = api;
                chartSelectedEntityAPI.onDataItemClicked = function (selectedEntity) {

                    $scope.selectedEntityId = selectedEntity.GroupKeyValues[0].Id;
                    $scope.selectedEntityName = selectedEntity.GroupKeyValues[0].Name;
                    getAndShowEntityStatistics();
                };
            };

            $scope.getData = function () {
                $scope.currentPage = 1;
                resultKey = '';
                getData();
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
        }

        function load() {
            $scope.fromDate = '2014-04-27';
            $scope.toDate = '2014-04-29';
            loadMeasureTypes();
        }

        function defineGrid()
        {
            gridOption = $scope.gridOptionsAllMeasures;
            gridOption.enableHorizontalScrollbar = 0;
            gridOption.enableVerticalScrollbar = 0;
            //gridOption.minRowsToShow = 30;
            //gridOption.enableFiltering = false;
            //gridOption.saveFocus = false;
            //gridOption.saveScroll = true;
            gridOption.enableColumnResizing = true;
           

            gridOption.columnDefs = [];
            var zoneColumn = {
                name: 'Zone',
                headerCellTemplate: '/Client/Templates/Grid/HeaderTemplate.html',//template,
                enableColumnMenu: false,
                field: 'GroupKeyValues[0].Name'
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
                    }
                   // cellFilter: "number:2"
                };
                gridOption.columnDefs.push(colDef);
            });
        }
        
        function getData() {
            if (!chartSelectedMeasureAPI)
                return;
            $scope.showResult = true;
            $scope.gridOptionsAllMeasures.data.length = 0;
            chartSelectedMeasureAPI.showLoader();
            var count = $scope.optionsTopCount.lastselectedvalue.value;
            
           
            var fromRow = ($scope.currentPage - 1) * count + 1;
            var toRow = fromRow + count - 1;
            AnalyticsAPIService.GetTrafficStatisticSummary(resultKey, [4], $scope.fromDate, $scope.toDate, fromRow, toRow, sortColumn.value, sortDescending).then(function (response) {

                resultKey = response.ResultKey;
                $scope.totalDataCount = response.TotalCount;

                angular.forEach(response.Data, function (itm) {
                    $scope.gridOptionsAllMeasures.data.push(itm);
                });
                

                var chartData = response.Data;
                var chartDefinition = {
                    type: "pie",
                    title: sortColumn.description,
                    yAxisTitle: "Value",
                    showLegendsWithValues: $scope.showValuesOnLegends
                };

                var seriesDefinitions = [{
                    title: sortColumn.description,
                    titlePath: "GroupKeyValues[0].Name",
                    valuePath: sortColumn.description
                }];
                console.log(chartData.length);
                chartSelectedMeasureAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
            })
                .finally(function () {
                    chartSelectedMeasureAPI.hideLoader();
                });
        }

        function getAndShowEntityStatistics() {

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
        
    });