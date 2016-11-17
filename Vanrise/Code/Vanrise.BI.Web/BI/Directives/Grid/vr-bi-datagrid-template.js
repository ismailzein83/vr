'use strict';
app.directive('vrBiDatagridTemplate', ['UtilsService', '$compile', 'VRNotificationService', 'VRUIUtilsService', 'TimeDimensionTypeEnum', 'VR_BI_BIConfigurationAPIService',
function (UtilsService, $compile, VRNotificationService, VRUIUtilsService, TimeDimensionTypeEnum, VR_BI_BIConfigurationAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new biGrid(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BI/Directives/Grid/Templates/BIDataGridTemplate.html"

    };

    function biGrid(ctrl, $scope, $attrs) {
        var measureDirectiveAPI;
        var measureReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var timeEntityDirectiveAPI;
        var timeEntityReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var entityDirectiveAPI;
        var entityReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var filterDimensionAPI;
        var filterReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        function initializeController() {

            $scope.onFilterDimensionReady = function (api) {
                filterDimensionAPI = api;
                var setLoader = function (value) { $scope.isLoadingFilterDirective = value };
                var payload = { entityNames: UtilsService.getPropValuesFromArray(ctrl.selectedEntitiesType, "Name") };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDimensionAPI, payload, setLoader, filterReadyPromiseDeferred);
            };

            ctrl.onEntitySelectionChanged = function () {
                if (filterDimensionAPI != undefined) {

                    var setLoader = function (value) { $scope.isLoadingFilterDirective = value };
                    var payload = { entityNames: UtilsService.getPropValuesFromArray(ctrl.selectedEntitiesType, "Name") };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, filterDimensionAPI, payload, setLoader, filterReadyPromiseDeferred);
                }

            };
            ctrl.selectedEntitiesType = [];
           
            ctrl.selectedMeasureTypes = [];
            ctrl.selectedTopMeasure;

            ctrl.onSelectionChanged = function () {
                if (ctrl.selectedTopMeasure == undefined)
                    ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];
                else {
                    if (!UtilsService.contains(ctrl.selectedMeasureTypes, ctrl.selectedTopMeasure))
                        ctrl.selectedTopMeasure = ctrl.selectedMeasureTypes[0];
                }
            };



            ctrl.onMeasureDirectiveReady = function (api) {
                measureDirectiveAPI = api;
                measureReadyPromiseDeferred.resolve();
            };

            ctrl.onTimeEntityDirectiveReady = function (api) {
                timeEntityDirectiveAPI = api;
                timeEntityReadyPromiseDeferred.resolve();
            };
            ctrl.onEntityDirectiveReady = function (api) {
                entityDirectiveAPI = api;
                entityReadyPromiseDeferred.resolve();
            };
            defineNumberOfColumns();
            defineOperationTypes();
            defineTimeDimensionTypes();
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var topMeasure;
                if (ctrl.selectedTopMeasure != undefined)
                    topMeasure = ctrl.selectedTopMeasure.Name;
                var measureTypes;
                if (ctrl.selectedMeasureTypes.length > 0) {
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

                var entityType;
                if (ctrl.selectedEntitiesType.length > 0 && ctrl.selectedOperationType.value != "MeasuresGroupedByTime") {
                    entityType = [];
                    for (var i = 0; i < ctrl.selectedEntitiesType.length; i++)
                        entityType.push(ctrl.selectedEntitiesType[i].Name);

                }
                else if (ctrl.selectedOperationType.value == "MeasuresGroupedByTime") {
                    entityType = null;
                }


                return {
                    $type: "Vanrise.BI.Entities.DataGridDirectiveSetting, Vanrise.BI.Entities",
                    OperationType: ctrl.selectedOperationType != undefined ? ctrl.selectedOperationType.value : undefined,
                    EntityType: entityType,
                    MeasureTypes: measureTypes,
                    TopMeasure: topMeasure,
                    TopRecords: ctrl.topRecords,
                    TimeEntity: ctrl.selectedTimeEntity != undefined ? ctrl.selectedTimeEntity.Name : undefined,
                    Filter: filterDimensionAPI != undefined ? filterDimensionAPI.getData() : undefined
                };

            };

            api.load = function (payload) {

                ctrl.topRecords = payload != undefined ? payload.TopRecords : 10;
                if (payload != undefined && payload.OperationType != undefined) {
                    for (var i = 0; i < ctrl.operationTypes.length; i++) {

                        if (ctrl.operationTypes[i].value == payload.OperationType)
                            ctrl.selectedOperationType = ctrl.operationTypes[i];
                    }
                }

                var promises = [];
                var loadMeasurePromiseDeferred = UtilsService.createPromiseDeferred();
                measureReadyPromiseDeferred.promise.then(function () {
                    var measurePayload = { selectedIds: payload != undefined ? payload.MeasureTypes : undefined };

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

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);