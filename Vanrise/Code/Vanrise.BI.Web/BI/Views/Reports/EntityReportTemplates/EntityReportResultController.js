EntityReportResultController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'TimeDimensionTypeEnum', 'BIEntityTypeEnum', 'BIMeasureTypeEnum', 'VR_BI_BIAPIService', 'BIUtilitiesService'];

function EntityReportResultController($scope, VRNavigationService, UtilsService, TimeDimensionTypeEnum, BIEntityTypeEnum, BIMeasureTypeEnum, VR_BI_BIAPIService, BIUtilitiesService) {
        var gridAPI;
        var entityType;
        var entityId;
        var entityName;
        var measures= [];
        defineScope();       
        load();   
        function defineScope() {
            $scope.data = [];
            for (prop in BIMeasureTypeEnum) {
                var obj = {
                    measure: BIMeasureTypeEnum[prop],
                    name: BIMeasureTypeEnum[prop].description,
                    value: BIMeasureTypeEnum[prop].value,
                    selected: true
                };
                measures.push(obj);
            }

            $scope.isGettingData = false;
            $scope.gridReady = function (api) {
                gridAPI = api;
                var entityReportResultReadyAPI = {
                    loadData: function (fromDate, toDate, timeDimensionValue) {
                        return loadData(fromDate, toDate, timeDimensionValue);
                    }
                };
                if ($scope.onEntityReportResultReady != undefined)
                    $scope.onEntityReportResultReady(entityReportResultReadyAPI);
            };
            $scope.chartEntityReady = function (api) {
                chartEntityReadyAPI = api;
            };
            $scope.choiceSelectionChanged = function () {
                if ($scope.selectedChoiceIndex == 1) {
                    loadEntityChart($scope.data);
                }
            };

            defineChoicesGroup();

            
        }

        function load() {
          
            defineMeasureTypes();
            if ($scope.viewScope != undefined) {
                entityType = $scope.viewScope.entityType;
                entityId = $scope.viewScope.entityId;
                entityName = $scope.viewScope.title;
            }
            else {
                entityType = $scope.entityType;
                entityId = $scope.entityId;
                entityName = $scope.title;
            }
            if ($scope.dataItem != undefined) {
                var fromDate = $scope.dataItem.Time;
                var parentTimeDimensionValue = $scope.dataItem.timeDimensionType;
                var toDate = BIUtilitiesService.getNextDate(parentTimeDimensionValue, $scope.dataItem);
                var timeDimensionValue = parentTimeDimensionValue + 1;
                loadData(fromDate, toDate, timeDimensionValue);
            }

        }
       
        function defineMeasureTypes() {
            $scope.measureTypes = [];
            for (var m in BIMeasureTypeEnum) {
                $scope.measureTypes.push(BIMeasureTypeEnum[m]);
            }

        }
        function defineChoicesGroup() {
            $scope.choicesGroup = [
                {title:"Report"},
                { title: "Graph" }
            ];

            $scope.choicesReady = function (api) {
                //api.selectChoice(1);
            };
        }
        function loadData(fromDate, toDate, timeDimensionValue) {
            $scope.timeDimensionValue = timeDimensionValue;
            $scope.isGettingData = true;
            var measureTypes = [];
            angular.forEach($scope.measureTypes, function (measureType) {
                measureTypes.push(measureType.value);
            });
          
            return VR_BI_BIAPIService.GetEntityMeasuresValues(entityType, entityId, timeDimensionValue, fromDate, toDate,null, measureTypes)
             .then(function (response) {
                $scope.data.length = 0;
                angular.forEach(response, function (itm) {
                     itm.timeDimensionType = timeDimensionValue;
                     $scope.data.push(itm);
                });

                BIUtilitiesService.fillDateTimeProperties(response, timeDimensionValue, fromDate, toDate, true);
                if ($scope.selectedChoiceIndex == 1)
                    loadEntityChart($scope.data);
                 $scope.isGettingData = false;

             }).catch(function (error) {
                 $scope.isGettingData = false;
             });

        }
        function loadEntityChart(chartData) {
            var chartDefinition = {
                type: "spline",
                title: entityName
            };
            var xAxisDefinition = {
                titlePath: "dateTimeValue"
            };
            var seriesDefinitions = [];
            for (var i = 0; i < measures.length; i++) {
                var measure = measures[i];
                seriesDefinitions.push({
                    title: measure.name,
                    valuePath: "Values[" + i + "]",
                    selected: measure.measure == BIMeasureTypeEnum.Sale || measure.measure == BIMeasureTypeEnum.Cost
                });
            }
            chartEntityReadyAPI.renderChart(chartData, chartDefinition, seriesDefinitions, xAxisDefinition);

        }
}

appControllers.controller('BI_EntityReportResultController', EntityReportResultController);