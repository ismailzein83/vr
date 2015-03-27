appControllers.controller('ZoneDashboardController',
    function ZoneDashboardController($scope, BIAPIService) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {

            //  $scope.zoneName = $routeParams.ZoneName;
            $scope.testModel = 'ZoneDashboardController ';

           
            $scope.optionsMeasureTypes = {
                selectedvalues: [],
                datasource: [],
                lastselectedvalue: ''
            };

            $scope.optionsTopCount = {
                datasource: [
                { name: "5", value: "5" },
                { name: "10", value: "10" },
                { name: "15", value: "15" },
                { name: "20", value: "25" }
                ]
            };

            $scope.otherMeasuresDescriptions = [];
            $scope.topDestinationData = [];
        }

        var chartTopDestinationsAPI;
        var chartZoneReadyAPI;
        function defineScopeMethods() {

            $scope.chartTopDestinationsReady = function (api) {
                chartTopDestinationsAPI = api;
                chartTopDestinationsAPI.onDataItemClicked = function (zoneItem) {
                   
                    $scope.selectedZoneId = zoneItem.EntityId;
                    $scope.selectedZoneName = zoneItem.EntityName;
                    getAndShowZone();
                };
                getAndShowTopDestination();
            };

            $scope.chartZoneReady = function (api) {
                chartZoneReadyAPI = api;
            };
            
            $scope.filterChanged = function () {
                getAndShowTopDestination();
            };

            $scope.updateZone = function () {
                getAndShowZone();
            };
        }

        function load() {
            initializeValues();
            getMeasureTypes();
        }

        function initializeValues()
        {
            $scope.fromDate = '2012-1-2';
            $scope.toDate = '2012-3-1';
            $scope.optionsTopCount.lastselectedvalue = $scope.optionsTopCount.datasource[1];
        }

        function getAndShowTopDestination() {
            if (!chartTopDestinationsAPI)
                return;
            var measureType = $scope.optionsMeasureTypes.lastselectedvalue;
            if (measureType == undefined || measureType == null || measureType.length == 0)
                return;

            if (chartZoneReadyAPI)
                chartZoneReadyAPI.hideChart();
            $scope.isZoneVisible = false;

            $scope.otherMeasuresDescriptions.length = 0;
            var moreMeasures = [];
            angular.forEach($scope.optionsMeasureTypes.datasource, function (itm) {
                if (itm != $scope.optionsMeasureTypes.lastselectedvalue) {
                    moreMeasures.push(itm.Value);
                    $scope.otherMeasuresDescriptions.push(itm.Description);
                }
            });

            $scope.topDestinationData.length = 0
            chartTopDestinationsAPI.showLoader();
            BIAPIService.GetTopEntities(0, measureType.Value, $scope.fromDate, $scope.toDate, $scope.optionsTopCount.lastselectedvalue.value, moreMeasures)
            .then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.topDestinationData.push(itm);
                });
                var chartData = response;
                var chartDefinition = {
                    type: "pie",
                    title: "TOP DESTINATIONS",
                    yAxisTitle: "Value",
                    showLegendsWithValues: $scope.showValuesOnLegends
                };

                var seriesDefinitions = [{
                    title: measureType.Description,
                    titleFieldName: "EntityName",
                    valueFieldName: "Value"
                }];

                chartTopDestinationsAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
            })
                .finally(function () {
                    chartTopDestinationsAPI.hideLoader();
                });
        }

        function getAndShowZone2(zoneId, zoneName)
        {
            chartZoneReadyAPI.showLoader();
            BIAPIService.GetEntityMeasureValues(0, zoneId, $scope.optionsMeasureTypes.lastselectedvalue.Value, 0, $scope.fromDate, $scope.toDate)
            .then(function (response) {
                var chartData = response;

                var chartDefinition = {
                    type: "spline",
                    title: zoneName,
                    yAxisTitle: $scope.optionsMeasureTypes.lastselectedvalue.Description
                };
                var xAxisDefinition = {
                    titleFieldName: "Time",
                    isDate: true
                };
                var seriesDefinitions = [{
                    title: $scope.optionsMeasureTypes.lastselectedvalue.Description,
                    valueFieldName: "Value"
                }
                ];
                chartZoneReadyAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);

            })
            .finally(function () {
                chartZoneReadyAPI.hideLoader();
            });;
        }

        function getAndShowZone() {
            var measures = [];
            angular.forEach($scope.optionsMeasureTypes.datasource, function (measure) {
                if (measure.selected)
                    measures.push(measure);
            });
            

            var measureValues = [];
            angular.forEach(measures, function (m) {
                measureValues.push(m.Value);
            });
            chartZoneReadyAPI.showLoader();
            BIAPIService.GetEntityMeasuresValues(0, $scope.selectedZoneId, 0, $scope.fromDate, $scope.toDate, measureValues)
            .then(function (response) {
                var chartData = response;
                angular.forEach(chartData, function (dataItem) {
                    for(i=0;i<measures.length;i++)
                    {
                        dataItem[measures[i].Description.replace(" ", "_")] = dataItem.Values[i];
                    }
                });

                var chartDefinition = {
                    type: "spline",
                    title: $scope.selectedZoneName,
                    yAxisTitle: $scope.optionsMeasureTypes.lastselectedvalue.Description
                };
                var xAxisDefinition = {
                    titleFieldName: "Time",
                    isDate: true
                };
                var seriesDefinitions = [];
                angular.forEach(measures, function (measure) {
                    seriesDefinitions.push({
                        title: measure.Description,
                        valueFieldName: measure.Description.replace(" ", "_")
                    });
                });

                chartZoneReadyAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
                $scope.isZoneVisible = true;
            })
            .finally(function () {
                chartZoneReadyAPI.hideLoader();
            });;
        }

        function getMeasureTypes()
        {
            BIAPIService.GetMeasureTypeList().then(function (response) {
                angular.forEach(response, function (itm) {
                    itm.selected = true;
                    $scope.optionsMeasureTypes.datasource.push(itm);
                });
                
                $scope.optionsMeasureTypes.lastselectedvalue = $scope.optionsMeasureTypes.datasource[0];

                getAndShowTopDestination();
            });
        }

    });