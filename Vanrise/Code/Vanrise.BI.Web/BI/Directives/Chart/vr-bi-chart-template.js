'use strict';
app.directive('vrBiChartTemplate', ['UtilsService', 'TimeDimensionTypeEnum', 'VRNotificationService', 'VRUIUtilsService', 'VR_ChartDefinitionTypeEnum', 'VR_BI_BIConfigurationAPIService',
function (UtilsService, TimeDimensionTypeEnum, VRNotificationService, VRUIUtilsService, VR_ChartDefinitionTypeEnum, VR_BI_BIConfigurationAPIService) {

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
        var measureDirectiveAPI;
        var measureReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var timeEntityDirectiveAPI;
        var timeEntityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var entityDirectiveAPI;
        var entityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var filterDimensionAPI;
        var filterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        function initializeController() {

            ctrl.onMeasureDirectiveReady = function (api) {
                measureDirectiveAPI = api;
                var setLoader = function (value) { ctrl.isLoadingMeasures = value };
                var payload;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, measureDirectiveAPI, payload, setLoader, measureReadyPromiseDeferred);
            };

            ctrl.onTimeEntityDirectiveReady = function (api) {
                timeEntityDirectiveAPI = api;
                timeEntityReadyPromiseDeferred.resolve();
            };

            ctrl.onEntityDirectiveReady = function (api) {
                entityDirectiveAPI = api;
                entityReadyPromiseDeferred.resolve();
            };
            $scope.onFilterDimensionReady = function (api) {
                filterDimensionAPI = api;
                var setLoader = function (value) { $scope.isLoadingFilterDirective = value };
                var payload = { entityNames: UtilsService.getPropValuesFromArray(ctrl.selectedEntitiesType, "Name") };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDimensionAPI, payload, setLoader, filterReadyPromiseDeferred);
            };

            $scope.onEntitySelectionChanged = function () {
                if (filterDimensionAPI != undefined) {

                    var setLoader = function (value) { $scope.isLoadingFilterDirective = value };
                    var payload = { entityNames: UtilsService.getPropValuesFromArray(ctrl.selectedEntitiesType, "Name") };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDimensionAPI, payload, setLoader, filterReadyPromiseDeferred);
                }

            };
            ctrl.selectedEntitiesType = [];
            ctrl.definitionTypes = [];
            ctrl.selectedDefinitionType;
            ctrl.selectedMeasureTypes = [];
            ctrl.selectedMeasureType;
            ctrl.selectedTopMeasure;

            ctrl.isPieChart = true;

            ctrl.onSwitchValueChanged = function () {
                if (ctrl.isPieChart) {
                    ctrl.selectedTopMeasure = undefined;
                    ctrl.selectedTopMeasure = lastTopMeasureValue;
                } else {
                    ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes.length > 0 ? ctrl.selectedMeasureTypes[0] : undefined;
                }

            };

            ctrl.onSelectionChanged = function () {
                if (ctrl.selectedTopMeasure == undefined) {
                    ctrl.selectedMeasureType != undefined && ctrl.isPieChart ? ctrl.selectedTopMeasure = ctrl.selectedMeasureType : ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];
                } else {
                    if (ctrl.selectedMeasureTypes.length > 0 && !UtilsService.contains(ctrl.selectedMeasureTypes, ctrl.selectedTopMeasure)) {
                        ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];

                    } else if (!ctrl.isPieChart && ctrl.selectedMeasureTypes.length == 0)

                        ctrl.selectedTopMeasure = undefined;
                    else if (ctrl.selectedMeasureType != undefined && ctrl.isPieChart)
                        ctrl.selectedTopMeasure = ctrl.selectedMeasureType;
                }
                lastTopMeasureValue = ctrl.selectedTopMeasure;
            };

            defineChartDefinitionTypes();
            defineOperationTypes();
            defineTimeDimensionTypes();

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var measureTypes;
                if (ctrl.selectedOperationType != undefined) {
                    if (ctrl.selectedOperationType.value == "TopEntities" && ctrl.isPieChart) {
                        if (ctrl.selectedEntitiesType.length > 0 && ctrl.selectedMeasureType != undefined) {
                            measureTypes = [];
                            measureTypes.push(ctrl.selectedMeasureType.Name);
                        }

                    }
                    else if ((ctrl.selectedOperationType.value == "MeasuresGroupedByTime" || !ctrl.isPieChart) && ctrl.selectedMeasureTypes.length > 0 && ctrl.selectedTopMeasure != undefined) {
                        measureTypes = [];
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
                var topMeasure;
                if (ctrl.selectedTopMeasure != undefined)
                    topMeasure = ctrl.selectedTopMeasure.Name;

                var entityType;

                if (ctrl.selectedOperationType.value != "MeasuresGroupedByTime" && ctrl.selectedEntitiesType.length > 0) {
                    entityType = [];
                    for (var i = 0; i < ctrl.selectedEntitiesType.length; i++)
                        entityType.push(ctrl.selectedEntitiesType[i].Name);
                } else if (ctrl.selectedOperationType.value == "MeasuresGroupedByTime") {
                    entityType = null;
                }

                return {
                    $type: "Vanrise.BI.Entities.ChartDirectiveSetting, Vanrise.BI.Entities",
                    OperationType: ctrl.selectedOperationType != undefined ? ctrl.selectedOperationType.value : undefined,
                    EntityType: entityType,
                    MeasureTypes: measureTypes,
                    TopMeasure: topMeasure,
                    DefinitionType: ctrl.selectedDefinitionType != undefined ? ctrl.selectedDefinitionType.value : undefined,
                    IsPieChart: ctrl.isPieChart,
                    TopRecords: ctrl.topRecords,
                    TimeEntity: ctrl.selectedTimeEntity != undefined ? ctrl.selectedTimeEntity.Name : undefined,
                    Filter: filterDimensionAPI != undefined ? filterDimensionAPI.getData() : undefined
                };

            };

            api.load = function (payload) {
                if (payload != undefined && payload.DefinitionType != undefined) {
                    for (var i = 0; i < ctrl.definitionTypes.length; i++) {
                        if (ctrl.definitionTypes[i].value == payload.DefinitionType)
                            ctrl.selectedDefinitionType = ctrl.definitionTypes[i];
                    }
                }

                if (payload != undefined && payload.operationTypes != undefined) {
                    for (var i = 0; i < ctrl.operationTypes.length; i++) {

                        if (ctrl.operationTypes[i].value == payload.OperationType)
                            ctrl.selectedOperationType = ctrl.operationTypes[i];
                    }
                }

                ctrl.isPieChart = payload != undefined ? payload.IsPieChart : undefined;
                ctrl.topRecords = payload != undefined ? payload.TopRecords : 10;


                var promises = [];
                var loadMeasurePromiseDeferred = UtilsService.createPromiseDeferred();
                measureReadyPromiseDeferred.promise.then(function () {
                    var measurePayload = { selectedIds: payload != undefined ? payload.MeasureTypes : undefined };
                    measureReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(measureDirectiveAPI, measurePayload, loadMeasurePromiseDeferred);

                });

                promises.push(loadMeasurePromiseDeferred.promise);

                var loadTimeEntityPromiseDeferred = UtilsService.createPromiseDeferred();
                timeEntityReadyPromiseDeferred.promise.then(function () {
                    var timeEntityPayload = { selectedIds: payload != undefined ? payload.TimeEntity : undefined };

                    VRUIUtilsService.callDirectiveLoad(timeEntityDirectiveAPI, timeEntityPayload, loadTimeEntityPromiseDeferred);

                });
                promises.push(loadTimeEntityPromiseDeferred.promise);


                var loadEntityPromiseDeferred = UtilsService.createPromiseDeferred();
                entityReadyPromiseDeferred.promise.then(function () {
                    var entityPayload = { selectedIds: payload != undefined ? payload.EntityType : undefined };

                    VRUIUtilsService.callDirectiveLoad(entityDirectiveAPI, entityPayload, loadEntityPromiseDeferred);

                });
                promises.push(loadEntityPromiseDeferred.promise);

                var loadFilterDimentionPromiseDeferred = UtilsService.createPromiseDeferred();
                filterReadyPromiseDeferred.promise.then(function () {
                    var entityPayload = payload != undefined ? { entityNames: payload.EntityType, filter: payload.Filter } : undefined;
                    filterReadyPromiseDeferred = undefined;
                    VRUIUtilsService.callDirectiveLoad(filterDimensionAPI, entityPayload, loadFilterDimentionPromiseDeferred);

                });
                promises.push(loadFilterDimentionPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises).then(function () {
                    if (payload != undefined) {
                        for (var j = 0; j < ctrl.selectedMeasureTypes.length; j++) {
                            if (ctrl.selectedMeasureTypes[j].Name == payload.TopMeasure)
                                ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[j];
                        }
                        lastTopMeasureValue = ctrl.selectedTopMeasure;
                    }

                });

            };

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

        function defineChartDefinitionTypes() {
            ctrl.definitionTypes = [];
            for (var m in VR_ChartDefinitionTypeEnum) {
                ctrl.definitionTypes.push(VR_ChartDefinitionTypeEnum[m]);
            }
            ctrl.selectedDefinitionType = ctrl.definitionTypes[0];
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);