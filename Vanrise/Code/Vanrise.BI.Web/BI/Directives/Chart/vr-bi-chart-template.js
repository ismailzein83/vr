'use strict';
app.directive('vrBiChartTemplate', ['UtilsService', 'TimeDimensionTypeEnum', 'VRNotificationService', 'VRUIUtilsService','ChartDefinitionTypeEnum','BIConfigurationAPIService','ChartSeriesTypeEnum',
function (UtilsService, TimeDimensionTypeEnum, VRNotificationService, VRUIUtilsService, ChartDefinitionTypeEnum, BIConfigurationAPIService, ChartSeriesTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new biChart(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BI/Directives/Chart/Templates/BIChartTemplate.html"

    };


    function biChart(ctrl, $scope, $attrs) {
        var lastTopMeasureValue;
        function initializeController() {
           
            ctrl.Measures = [];
            ctrl.Entities = [];
            ctrl.selectedEntitiesType = [];
            ctrl.definitionTypes = [];
            ctrl.selectedDefinitionType;
            ctrl.selectedMeasureTypes = [];
            ctrl.selectedMeasureType;
            ctrl.selectedTopMeasure;
            
            ctrl.singleMeasureRequired = false;
            ctrl.topMeasureRequired = false;
            ctrl.multipleMeasureRequired = false;
            ctrl.entityRequired = false;
            ctrl.isPieChart = true;

            ctrl.onSwitchValueChanged = function () {
                if (ctrl.isPieChart) {
                    ctrl.selectedTopMeasure = undefined;
                    ctrl.singleMeasureRequired = true;
                    ctrl.multipleMeasureRequired = false;
                    ctrl.topMeasureRequired = false;
                    ctrl.selectedTopMeasure = lastTopMeasureValue;
                }
                else {
                    ctrl.singleMeasureRequired = false;
                    ctrl.multipleMeasureRequired = true;
                    ctrl.topMeasureRequired = true;
                    ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes.length > 0 ? ctrl.selectedMeasureTypes[0] : undefined;
                }

            }
            ctrl.onSelectionOperationChanged = function () {
                if (ctrl.selectedOperationType.value == "MeasuresGroupedByTime") {
                    ctrl.singleMeasureRequired = false;
                    ctrl.topMeasureRequired = false;
                    ctrl.multipleMeasureRequired = true;
                    ctrl.entityRequired = false;
                }
                else {
                    ctrl.singleMeasureRequired = true;
                    ctrl.topMeasureRequired = false;
                    ctrl.multipleMeasureRequired = false;
                    ctrl.entityRequired = true;
                }
            }

            ctrl.onSelectionChanged = function () {
                if (ctrl.selectedTopMeasure == undefined) {
                    ctrl.selectedMeasureType != undefined && ctrl.isPieChart ? ctrl.selectedTopMeasure = ctrl.selectedMeasureType : ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];
                }
                else {
                    if (ctrl.selectedMeasureTypes.length > 0 && !UtilsService.contains(ctrl.selectedMeasureTypes, ctrl.selectedTopMeasure)) {
                        ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];

                    }
                    else if (!ctrl.isPieChart && ctrl.selectedMeasureTypes.length == 0)

                        ctrl.selectedTopMeasure = undefined;
                    else if (ctrl.selectedMeasureType != undefined && ctrl.isPieChart)
                        ctrl.selectedTopMeasure = ctrl.selectedMeasureType;
                }
                lastTopMeasureValue = ctrl.selectedTopMeasure;
            }
            ctrl.topRecords = 10;

            defineChartDefinitionTypes();
            defineOperationTypes();
            defineChartSeriesTypes();
            defineTimeDimensionTypes();


            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var measureTypes = [];
                if (ctrl.selectedOperationType.value == "TopEntities" && ctrl.isPieChart) {
                    if (ctrl.selectedEntitiesType.length == 0 || ctrl.selectedMeasureType == undefined)
                        return false;
                    else {
                        measureTypes.push(ctrl.selectedMeasureType.Name);
                    }
                }
                else if (ctrl.selectedOperationType.value == "MeasuresGroupedByTime" || !ctrl.isPieChart) {
                    if (ctrl.selectedMeasureTypes == undefined || ctrl.selectedMeasureTypes.length == 0)
                        return false;
                    else {
                        for (var i = 0; i < ctrl.selectedMeasureTypes.length; i++) {
                            measureTypes.push(ctrl.selectedMeasureTypes[i].Name);
                            if (ctrl.selectedMeasureTypes[i].Name == ctrl.selectedTopMeasure.Name) {
                                var swap = measureTypes[0];
                                measureTypes[0] = ctrl.selectedMeasureTypes[i].Name;
                                measureTypes[i] = swap;
                            }
                        }

                    }
                }
                var topMeasure = null;
                if (ctrl.selectedTopMeasure != undefined)
                    topMeasure = ctrl.selectedTopMeasure.Name;
                var entityType = [];

                if (ctrl.selectedEntitiesType.length > 0 && ctrl.selectedOperationType.value != "MeasuresGroupedByTime") {
                    for (var i = 0; i < ctrl.selectedEntitiesType.length; i++)
                        entityType.push(ctrl.selectedEntitiesType[i].Name);
                }

                return {
                    $type: "Vanrise.BI.Entities.ChartDirectiveSetting, Vanrise.BI.Entities",
                    OperationType: ctrl.selectedOperationType.value,
                    EntityType: entityType,
                    MeasureTypes: measureTypes,
                    TopMeasure: topMeasure,
                    DefinitionType: ctrl.selectedDefinitionType.value,
                    IsPieChart: ctrl.isPieChart,
                    TopRecords: ctrl.topRecords
                };
                
            }

            api.load = function (payload) {
              if (payload != undefined &&  payload.DefinitionType != undefined) {
                    for (var i = 0; i < ctrl.definitionTypes.length; i++) {
                        if (ctrl.definitionTypes[i].value == payload.DefinitionType)
                            ctrl.selectedDefinitionType = ctrl.definitionTypes[i];
                    }
              }

              if (payload != undefined &&  payload.operationTypes != undefined) {
                    for (var i = 0; i < ctrl.operationTypes.length; i++) {

                        if (ctrl.operationTypes[i].value == payload.OperationType)
                            ctrl.selectedOperationType = ctrl.operationTypes[i];
                    }
               }

              ctrl.isPieChart = payload != undefined ? payload.IsPieChart : undefined;
              ctrl.topRecords = payload != undefined ? payload.TopRecords : undefined;
               var promises = [];
               var loadEntities= BIConfigurationAPIService.GetEntities().then(function (response) {
                    angular.forEach(response, function (itm) {
                        ctrl.Entities.push(itm);
                    });
                    if (payload != undefined &&  payload.EntityType != undefined) {
                        ctrl.selectedEntitiesType.length = 0;
                        for (var j = 0; j < payload.EntityType.length; j++) {
                            for (var i = 0; i < ctrl.Entities.length; i++) {
                                if (ctrl.Entities[i].Name == payload.EntityType[j] && !UtilsService.contains(ctrl.selectedEntitiesType, ctrl.Entities[i])) {
                                    ctrl.selectedEntitiesType.push(ctrl.Entities[i]);
                                }
                            }
                        }
                    }
                });  
               promises.push(loadEntities);       
               var loadMeasures= BIConfigurationAPIService.GetMeasures().then(function (response) {
                   angular.forEach(response, function (itm) {
                       ctrl.Measures.push(itm);

                   });
                   if (payload != undefined && payload.MeasureTypes != undefined)
                   {
                       for (var i = 0; i < payload.MeasureTypes.length; i++) {
                           var measureType = payload.MeasureTypes[i];
                           for (var j = 0; j < ctrl.Measures.length; j++) {

                               if (measureType == ctrl.Measures[j].Name) {
                                   if (ctrl.selectedOperationType.value == "TopEntities" && payload.IsPieChart) {
                                       ctrl.singleMeasureRequired = true;
                                       ctrl.multipleMeasureRequired = false;
                                       ctrl.topMeasureRequired = false;
                                       ctrl.selectedMeasureType = ctrl.Measures[j];
                                   }

                                   else {
                                       ctrl.selectedMeasureTypes.push(ctrl.Measures[j]);
                                       ctrl.multipleMeasureRequired = true;
                                       ctrl.singleMeasureRequired = false;
                                       if (ctrl.selectedOperationType.value == "MeasuresGroupedByTime") {
                                           ctrl.topMeasureRequired = false;
                                           ctrl.entityRequired = false;
                                       }
                                       else {
                                           ctrl.topMeasureRequired = true;
                                           ctrl.entityRequired = true;
                                       }
                                   }

                               }



                           }
                       }
                   }
               });
               promises.push(loadMeasures);
               lastTopMeasureValue = ctrl.selectedTopMeasure;
               return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);

          
        }
        function defineTimeDimensionTypes() {
            ctrl.timeDimensionTypes = [];
            for (var td in TimeDimensionTypeEnum)
                ctrl.timeDimensionTypes.push(TimeDimensionTypeEnum[td]);

            ctrl.selectedTimeDimensionType = $.grep(ctrl.timeDimensionTypes, function (t) {
                return t == TimeDimensionTypeEnum.Daily;
            })[0];
        }
        function defineOperationTypes() {
            ctrl.operationTypes = [{
                value: "TopEntities",
                description: "Top X"
            }, {
                value: "MeasuresGroupedByTime",
                description: "Time Variation Data"
            }
            ];
            ctrl.selectedOperationType = ctrl.operationTypes[0];
        }
        function defineChartSeriesTypes() {
            ctrl.chartSeriesTypes = [];
            for (var m in ChartSeriesTypeEnum) {
                ctrl.chartSeriesTypes.push(ChartSeriesTypeEnum[m]);
            }
        }
        function defineChartDefinitionTypes() {
            ctrl.definitionTypes = [];
            for (var m in ChartDefinitionTypeEnum) {
                ctrl.definitionTypes.push(ChartDefinitionTypeEnum[m]);
            }
            ctrl.selectedDefinitionType = ctrl.definitionTypes[0];
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);