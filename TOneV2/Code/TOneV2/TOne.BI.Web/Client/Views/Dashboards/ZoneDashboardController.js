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
                    measure: BIMeasureTypeEnum[prop],
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
                { name: "20", value: "20" }
                ]
            };

            $scope.data = [];

            var actions = [
                    {
                        name: "Zone Summary",
                        clicked: function (zoneItem) {
                            $scope.zoneId = zoneItem.EntityId;
                            $scope.zoneName = zoneItem.EntityName;
                            var selectedDateTimeFilter = $scope.dateTimeFilterOption.lastselectedvalue;
                            $scope.fromDate = selectedDateTimeFilter.fromDate;
                            $scope.toDate = selectedDateTimeFilter.toDate;
                            var addModal = $modal({ scope: $scope, template: '/Client/Modules/BI/Views/Reports/ZoneSummary.html', show: true, animation: "am-fade-and-scale" });
                        }
                    },
                    {
                        name: "Zone Details",
                        clicked: function (zoneItem) {
                            $location.path("/BI/ZoneDetails/" + zoneItem.EntityId + "/" + zoneItem.EntityName)
                        }
                    }
            ];
            var actions1 = [{ name: 'test', clicked: function (zoneItem) { } }];
            $scope.zoneMenuActions = 
            function (zoneItem) {
                if (zoneItem.Values[1] > 200000)
                    return null;
                if (zoneItem.Values[1] > 50000)
                    return actions1;
                return actions;
            }

            $scope.actions = actions;

            var values = ['rrr', 'rttt'];
            $scope.values = values;
            $scope.getValues = function () {
                return values;
            }
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
            
            $scope.updateData = function (asyncHandle) {
                getAndShowTopDestination(asyncHandle);
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

        function getAndShowTopDestination(asyncHandle) {
            if (!chartTopDestinationsAPI)
                return;
            var measureType = $scope.optionsMeasureTypes.lastselectedvalue;
            if (measureType == undefined || measureType == null || measureType.length == 0)
                return;

            if (chartZoneReadyAPI)
                chartZoneReadyAPI.hideChart();

            var measures = [];
            angular.forEach($scope.optionsMeasureTypes.datasource, function (itm) {
                measures.push(itm.value);
            });
            $scope.isGettingData = true;
            BIAPIService.GetTopEntities(BIEntityTypeEnum.SaleZone.value, measureType.value, $scope.fromDate, $scope.toDate, $scope.optionsTopCount.lastselectedvalue.value, measures)
            .then(function (response) {
                $scope.data.length = 0;
                angular.forEach(response, function (itm) {
                    $scope.data.push(itm);
                });
                var chartData = response;
                var chartDefinition = {
                    type: "pie",
                    //  title: "TOP DESTINATIONS",
                    yAxisTitle: "Value"
                };

                var seriesDefinitions = [{
                    title: measureType.name,
                    titlePath: "EntityName",
                    valuePath: "Values[" + $scope.optionsMeasureTypes.datasource.indexOf(measureType) + "]"
                }];

                chartTopDestinationsAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
            })
                .finally(function () {
                    $scope.isGettingData = false;
                    if (asyncHandle)
                        asyncHandle.operationDone();
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
            $scope.isGettingZoneData = true;
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
                        valuePath: "Values[" + i + "]",
                        selected: measure.measure == BIMeasureTypeEnum.Sale || measure.measure == BIMeasureTypeEnum.Cost
                    });
                }

                chartZoneReadyAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
            })
            .finally(function () {
                $scope.isGettingZoneData = false;
            });;
        }
    });