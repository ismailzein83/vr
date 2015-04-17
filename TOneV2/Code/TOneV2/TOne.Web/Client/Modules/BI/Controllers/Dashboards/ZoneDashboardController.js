appControllers.controller('ZoneDashboardController',
    function ZoneDashboardController($scope, BIAPIService, BITimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum) {

        defineScopeObjects();
        defineScopeMethods();
        load();

        function defineScopeObjects() {

            //  $scope.zoneName = $routeParams.ZoneName;
            $scope.testModel = 'ZoneDashboardController ';

           
            $scope.optionsMeasureTypes = {
                datasource: []
            };

            for (prop in BIMeasureTypeEnum) {
                var obj = {
                    name: BIMeasureTypeEnum[prop].description,
                    value: BIMeasureTypeEnum[prop].value,
                    selected: true
                };
                $scope.optionsMeasureTypes.datasource.push(obj);
            }

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
        }

        function initializeValues()
        {
            $scope.fromDate = '2015-02-15';
            $scope.toDate = '2015-04-15';
            $scope.optionsMeasureTypes.lastselectedvalue = $scope.optionsMeasureTypes.datasource[0];
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

            $scope.otherMeasuresDescriptions.length = 0;
            var measures = [];
            angular.forEach($scope.optionsMeasureTypes.datasource, function (itm) {
                measures.push(itm.value);
                $scope.otherMeasuresDescriptions.push(itm.name);
            });

            $scope.topDestinationData.length = 0
            chartTopDestinationsAPI.showLoader();
            BIAPIService.GetTopEntities(BIEntityTypeEnum.SaleZone.value, measureType.value, $scope.fromDate, $scope.toDate, $scope.optionsTopCount.lastselectedvalue.value, measures)
            .then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.topDestinationData.push(itm);
                });
                var chartData = response;
                var chartDefinition = {
                    type: "pie",
                    title: "TOP DESTINATIONS",
                    yAxisTitle: "Value"
                };

                var seriesDefinitions = [{
                    title: measureType.Description,
                    titlePath: "EntityName",
                    valuePath: "Values[0]"
                }];

                chartTopDestinationsAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
            })
                .finally(function () {
                    chartTopDestinationsAPI.hideLoader();
                });
        }

        function getAndShowZone() {
            var measures = [];
            angular.forEach($scope.optionsMeasureTypes.datasource, function (measure) {
                if (measure.selected)
                    measures.push(measure);
            });
            if (measures.length == 0) {
                return;
            }

            var measureValues = [];
            angular.forEach(measures, function (m) {
                measureValues.push(m.value);
            });
            chartZoneReadyAPI.showLoader();
            BIAPIService.GetEntityMeasuresValues(BIEntityTypeEnum.SaleZone.value, $scope.selectedZoneId, BITimeDimensionTypeEnum.Daily.value, $scope.fromDate, $scope.toDate, measureValues)
            .then(function (response) {
                var chartData = response;                

                var chartDefinition = {
                    type: "spline",
                    title: $scope.selectedZoneName
                };
                var xAxisDefinition = {
                    titlePath: "Time",
                    isDate: true
                };
                var seriesDefinitions = [];
                for (var i = 0; i < measures.length; i++)
                {
                    var measure = measures[i];
                    seriesDefinitions.push({
                        title: measure.name,
                        valuePath: "Values[" + i + "]"
                    });
                }

                chartZoneReadyAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
            })
            .finally(function () {
                chartZoneReadyAPI.hideLoader();
            });;
        }
    });