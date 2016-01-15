'use strict';
app.directive('vrBiDatagridTemplate', ['UtilsService', '$compile', 'VRNotificationService', 'VRUIUtilsService','ChartSeriesTypeEnum','TimeDimensionTypeEnum','BIConfigurationAPIService',
function (UtilsService, $compile, VRNotificationService, VRUIUtilsService, ChartSeriesTypeEnum, TimeDimensionTypeEnum, BIConfigurationAPIService) {

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
        templateUrl: "/Client/Modules/BI/Directives/Grid/Templates/BIDataGridTemplate.html"

    };


    function biChart(ctrl, $scope, $attrs) {

        function initializeController() {
            ctrl.Measures = [];
            ctrl.Entities = [];
            ctrl.selectedEntitiesType = [];
            ctrl.topRecords = 10;
            ctrl.selectedMeasureTypes = [];
            ctrl.selectedTopMeasure;

            ctrl.onSelectionChanged = function () {
                if (ctrl.selectedTopMeasure == undefined)
                    ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];
                else {
                    if (!UtilsService.contains(ctrl.selectedMeasureTypes, ctrl.selectedTopMeasure))
                        ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];
                }
            }

            ctrl.onSelectionOperationChanged = function () {
                if (ctrl.selectedOperationType.value == "MeasuresGroupedByTime") {
                    ctrl.entityRequired = false;
                }
                else {
                    ctrl.entityRequired = true;
                }
            }

            defineNumberOfColumns();
            defineOperationTypes();
            defineChartSeriesTypes();
            defineTimeDimensionTypes();
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                switch (ctrl.selectedOperationType.value) {
                    case "TopEntities": if (ctrl.selectedEntitiesType.length == 0 || ctrl.selectedMeasureTypes == undefined || ctrl.selectedMeasureTypes.length == 0) return false;
                    case "MeasuresGroupedByTime": if (ctrl.selectedMeasureTypes == undefined || ctrl.selectedMeasureTypes.length == 0) return false;
                }

                var topMeasure = null;
                if (ctrl.selectedTopMeasure != undefined)
                    topMeasure = ctrl.selectedTopMeasure.Name;
                var measureTypes = [];
                for (var i = 0; i < ctrl.selectedMeasureTypes.length; i++) {
                    measureTypes.push(ctrl.selectedMeasureTypes[i].Name);
                    if (ctrl.selectedMeasureTypes[i].Name == ctrl.selectedTopMeasure.Name) {
                        var swap = measureTypes[0];
                        measureTypes[0] = ctrl.selectedMeasureTypes[i].Name;
                        measureTypes[i] = swap;
                    }
                }
                var entityType = [];
                if (ctrl.selectedEntitiesType.length > 0 && ctrl.selectedOperationType.value != "MeasuresGroupedByTime") {
                    for (var i = 0; i < ctrl.selectedEntitiesType.length; i++)
                        entityType.push(ctrl.selectedEntitiesType[i].Name);

                }

                return {
                    $type: "Vanrise.BI.Entities.DataGridDirectiveSetting, Vanrise.BI.Entities",
                    OperationType: ctrl.selectedOperationType.value,
                    EntityType: entityType,
                    MeasureTypes: measureTypes,
                    TopMeasure: topMeasure,
                    TopRecords: ctrl.topRecords
                };

            }

            api.load = function (payload) {

                ctrl.topRecords = payload != undefined ? payload.TopRecords : undefined;
                if (payload != undefined && payload.OperationType != undefined) {
                    for (var i = 0; i < ctrl.operationTypes.length; i++) {

                        if (ctrl.operationTypes[i].value == payload.OperationType)
                            ctrl.selectedOperationType = ctrl.operationTypes[i];
                    }
                }
                var promises = [];
                var loadMeasures = BIConfigurationAPIService.GetMeasures().then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.Measures.push(itm);
                        });
                        if (payload != undefined && payload.MeasureTypes != undefined) {
                            for (var i = 0; i < payload.MeasureTypes.length; i++) {
                                var measureType = payload.MeasureTypes[i];
                                for (var j = 0; j < ctrl.Measures.length; j++) {
                                    if (measureType == ctrl.Measures[j].Name)
                                        ctrl.selectedMeasureTypes.push(ctrl.Measures[j]);
                                    if (ctrl.Measures[j].Name == payload.TopMeasure)
                                        ctrl.selectedTopMeasure = ctrl.Measures[j];
                                }
                            }
                        }
                 });
                 promises.push(loadMeasures);
                 var loadEntities = BIConfigurationAPIService.GetEntities().then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.Entities.push(itm);
                        });
                        if (payload != undefined && payload.EntityType != undefined) {
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
        function defineNumberOfColumns() {
            ctrl.numberOfColumns = [
                {
                    value: "6",
                    description: "Half Row"
                },
                {
                    value: "12",
                    description: "Full Row"
                }
            ];

            ctrl.selectedNumberOfColumns = ctrl.numberOfColumns[0];
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

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);