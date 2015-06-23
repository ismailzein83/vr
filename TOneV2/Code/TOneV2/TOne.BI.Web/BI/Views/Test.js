BIConfigurationController.$inject = ['$scope', 'BIVisualElementService1', 'BIUtilitiesService', 'BIConfigurationAPIService', 'UtilsService', 'VRNavigationService', '$q', 'VRNotificationService', 'BITimeDimensionTypeEnum'];

function BIConfigurationController($scope, BIVisualElementService1,BIUtilitiesService, BIConfigurationAPIService, UtilsService, VRNavigationService, $q, VRNotificationService, BITimeDimensionTypeEnum) {

    $scope.isInitializing = false;
    var mainGridAPI;
    var CDROption = [];
    var getDataAfterLoading;


    defineScope();
    load();
    defineScopeMethods();
    function defineScope() {

        $scope.fromDate = "2015-04-01";
        $scope.toDate = "2015-04-30";
        defineTimeDimensionTypes();
        $scope.profit = [];
        $scope.visualElements = [];
        $scope.Measures = [];
        $scope.Entities = [];
        $scope.selectedMeasures = [];
        //$scope.selectedEntities =
        $scope.selectedEntity;
        defineDirectives();
        defineOperationTypes();
        $scope.data = [];
        $scope.selectedDirective;
        var element = {};
        var elementSettings = {};
        $scope.removeVisualElement = function (visualElement) {
            $scope.visualElements.splice($scope.visualElements.indexOf(visualElement), 1);
        };
        
       
        defineNumberOfColumns();
        $scope.chartReady = function (api) {
            $scope.chartAPI = api;
           
        };

        $scope.chartTopReady = function (api) {
            chartTopAPI = api;
           // updateChart();
        };


        $scope.getTitle = function () {
            var title = "";
            for (var i = 0; i < $scope.selectedMeasures.length; i++) {
                title += "/" + $scope.selectedMeasures[i].Name
            }
            return title;
        }
        $scope.add = function () {
            var measures = [];
            if ($scope.selectedMeasures != undefined)
                for (var i = 0; i < $scope.selectedMeasures.length; i++) {
                    measures.push($scope.selectedMeasures[i].Name);
                }
            elementSettings = {
                operationType: $scope.selectedOperationType,
                entityType: $scope.selectedEntity,
                measureTypes: $scope.selectedMeasures
            };
            element = {
                //   timedimensiontype: $scope.selectedTimeDimensionType.value,
                //  fromdate:$scope.fromdate,
                //    todate:$scope.todate,
                directive: $scope.selectedDirective.value,
                settings: elementSettings,
                numberOfColumns: $scope.selectedNumberOfColumns.value
            };

            addVisualElement(element.directive, element.settings, element.numberOfColumns);
        };
        $scope.getData = function () {
            
            return getData();
        };

    }
    function defineTimeDimensionTypes() {
        $scope.timeDimensionTypes = [];
        for (var td in BITimeDimensionTypeEnum)
            $scope.timeDimensionTypes.push(BITimeDimensionTypeEnum[td]);

        $scope.selectedTimeDimensionType = $.grep($scope.timeDimensionTypes, function (t) {
            return t == BITimeDimensionTypeEnum.Daily;
        })[0];
    }
    function defineScopeMethods() {
    }
    function load() {
        getDataAfterLoading = true;
        UtilsService.waitMultipleAsyncOperations([loadMeasures, loadEntities])
            .then(function () {
                if (getDataAfterLoading)
                    return;
            })
            .finally(function () {
                $scope.isInitializing = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }
    function loadMeasures() {
        return BIConfigurationAPIService.GetMeasures().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Measures.push(itm);
                console.log(itm);
            });
        });
    }
    function loadEntities() {
        return BIConfigurationAPIService.GetEntities().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.Entities.push(itm);
               // console.log($scope.Entities[0].Id);
            });
        });
    }

    //function getData() {
    //    updateChart();
    //    if ($scope.selectedMeasures.length == 0)
    //        return;
    //    if (chartTopAPI == undefined)
    //        return;


     
        
    //    var refreshDataOperations = [];
    //    angular.forEach($scope.visualElements, function (visualElement) {
    //        refreshDataOperations.push(visualElement.API.retrieveData);
    //    });
    //    //BIVisualElementService1.retrieveData($scope.visualElements[0], $scope.visualElements[0].settings);
    //    $scope.isGettingData = true;
    //    UtilsService.waitMultipleAsyncOperations(refreshDataOperations)
    //        .finally(function () {
    //            $scope.isGettingData = false;
    //        });

    //    $scope.isGettingData = true;
    //    UtilsService.waitMultipleAsyncOperations([updateTopChartValues]).finally(function () {
    //        $scope.isGettingData = false;
    //    }).catch(function (error) {
    //        VRNotificationService.notifyExceptionWithClose(error, $scope);
    //    });

    //    var isSucceeded;
    //    $scope.showResult = true;
    //    $scope.isGettingData = true;
    //    var measures = [];
    //    for (var i = 0; i < $scope.selectedMeasures.length; i++)
    //    {
    //        measures.push($scope.selectedMeasures[i].Id);
    //    }
    //    //console.log(measures);
    //    return BIConfigurationAPIService.GetMeasureValues($scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, measures).then(function (response) {
    //        $scope.data = [];
    //        BIUtilitiesService.fillDateTimeProperties(response, $scope.selectedTimeDimensionType.value, $scope.fromDate, $scope.toDate, true);
    //        angular.forEach(response, function (itm) {
    //         //   console.log(itm);
    //            $scope.data.push(itm);
    //        });
    //      //  console.log(currentData[0].Values);
    //        //mainGridAPI.addItemsToSource(response.Data);
    //        //isSucceeded = true;
    //    })
    //        .finally(function () {
    //            $scope.isGettingData = false;
    //        });
    //}

    //function updateChart() {
    //    if ($scope.chartAPI == undefined)
    //        return;
    //    $scope.profit.length = 0;
    //    $scope.isGettingData = true;
    //    var selectedTimeDimension = $scope.selectedTimeDimensionType.value;
    //    var measuresID = [];
    //    console.log($scope.selectedMeasures.length);
    //    for (var j = 0; j < $scope.selectedMeasures.length; j++) {
    //        measuresID.push($scope.selectedMeasures[j].Id);
    //    }

    //    return BIConfigurationAPIService.GetMeasureValues(selectedTimeDimension, $scope.fromDate, $scope.toDate, measuresID)
    //        .then(function (response) {
    //            BIUtilitiesService.fillDateTimeProperties(response, selectedTimeDimension, $scope.fromDate, $scope.toDate);
    //            angular.forEach(response, function (item) {
    //                $scope.profit.push(item);
    //            });

    //            var chartData = $scope.profit;
    //            var chartDefinition = {
    //                type: "column",
    //              //  title: getTitle(),
    //                yAxisTitle: "Value"
    //            };

    //            var xAxisDefinition = { titlePath: "dateTimeValue", groupNamePath: "dateTimeGroupValue" };
    //            var seriesDefinitions = [];
    //            addSeriesDefinitions(seriesDefinitions);
    //            $scope.chartAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);
    //        })
    //        .finally(function () {
    //            $scope.isGettingData = false;
    //        });

       
    //}
    //function updateTopChartValues() {
    //    if ($scope.selectedEntity == undefined) {
    //        return;
    //    }
    //    return updateTopChart(chartTopAPI, $scope.selectedEntity.Id, {
    //        chartTitle: $scope.selectedEntity.Name,
    //        seriesTitle: $scope.selectedEntity.Name
    //    });
    //}
    function defineOperationTypes() {
        $scope.operationTypes = [{
            value: "TopEntities",
            description: "Top X"
        }, {
            value: "MeasuresGroupedByTime",
            description: "Time Variation Data"
        }
        ];
        $scope.selectedOperationType = $scope.operationTypes[0];
    }

    //function updateTopChart(chartAPI, entityType, chartSettings) {
    //    if (!chartAPI)
    //        return;
    //    var measureType = $scope.selectedMeasures[0];
    //    if (measureType == undefined || measureType == null || measureType.length == 0)
    //        return;
    //    var measuresID = [];
    //    console.log($scope.selectedMeasures.length);
    //    for (var j = 0; j < $scope.selectedMeasures.length; j++) {
    //        measuresID.push($scope.selectedMeasures[j].Id);
    //        console.log($scope.selectedMeasures[j].Id);
    //    }
    //    return BIConfigurationAPIService.GetTopEntities(entityType, measureType.Id, $scope.fromDate, $scope.toDate, 10, measuresID)
    //        .then(function (response) {
    //            var chartData = [];
    //            angular.forEach(response, function (item) {
    //                chartData.push(item);
    //            });

    //            var chartDefinition = {
    //                type: "pie",
    //                //   title: chartSettings.chartTitle,
    //                yAxisTitle: "Value"
    //            };

    //            var seriesDefinitions = [{
    //                title: measureType.Name,//chartSettings.seriesTitle,
    //                titlePath: "EntityName",
    //                valuePath: "Values[0]"
    //            }];

    //            chartAPI.renderSingleDimensionChart(chartData, chartDefinition, seriesDefinitions);
    //        });
    //}


    function addSeriesDefinitions(seriesDefinitions) {
        for (var i = 0; i < $scope.selectedMeasures.length; i++) {
            seriesDefinitions.push({
                title: $scope.selectedMeasures[i].Name,
                valuePath: "Values[" + i + "]",
                type: "spline"
            })
        }
    }
    function defineNumberOfColumns() {
        $scope.numberOfColumns = [
            {
                value: "6",
                description: "Half Row"
            },
            {
                value: "12",
                description: "Full Row"
            }
        ];

        $scope.selectedNumberOfColumns = $scope.numberOfColumns[0];
    }

    function addVisualElement(directive, settings, numberOfColumns) {
        var visualElement = {
            directive: directive,
            settings: settings,
            numberOfColumns: numberOfColumns
        };
        visualElement.onElementReady = function (api) {
            visualElement.API = api;
        };
       // console.log(visualElement);
        $scope.visualElements.push(visualElement);
    }

    function defineDirectives() {
        $scope.directives = [
            {
                value: "vr-chart-bi",
                description: "Chart"
            },
            {
                value: "vr-datagrid-bi",
                description: "Report"
            }
        ];
        $scope.selectedDirective = $scope.directives[0];
    }
}
appControllers.controller('BI_BIConfigurationController', BIConfigurationController);