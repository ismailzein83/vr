"use strict";

app.directive("vrAnalyticDatagridAnalyticrecords", ['UtilsService', 'VRNotificationService', 'Analytic_AnalyticService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService', 'VR_Analytic_AnalyticItemConfigAPIService',
    function (UtilsService, VRNotificationService, Analytic_AnalyticService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService, VR_Analytic_AnalyticItemConfigAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericGrid = new GenericGrid($scope, ctrl, $attrs);
                genericGrid.initializeController();
            },
            controllerAs: "analyticrecordsctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/Grid/Templates/AnalyticRecordsDataGrid.html"

        };

        function GenericGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            ctrl.datasource = [];
            ctrl.dimensions = [];
            ctrl.groupingDimensions = [];
            ctrl.measures = [];
            ctrl.drillDownDimensions = [];
            ctrl.dimensionsConfig = [];
            ctrl.measuresConfig = [];
            ctrl.sortField = "";
            var gridApi;
            var tableId;
            var drillDown;
            var fromTime;
            var toTime;

            function initializeController() {

                ctrl.mainGrid = (ctrl.parameters == undefined);

                ctrl.gridReady = function (api) {
                    gridApi = api;
                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.load = function (payLoad) {
                            var promiseReadyDeferred = UtilsService.createPromiseDeferred();
                            tableId = payLoad.TableId;
                            var promises = [];
                            if (payLoad.DimensionsConfig == undefined) {
                                promises.push(loadDimensionsConfig());
                            }
                            if(payLoad.MeasuresConfig ==undefined)
                            {
                                promises.push(loadMeasuresConfig());
                            }
                            UtilsService.waitMultiplePromises(promises).then(function()
                            {
                                loadGrid(payLoad).finally(function () {
                                    promiseReadyDeferred.resolve();
                                }).catch(function (error) {
                                    promiseReadyDeferred.reject(error);
                                });
                            }).catch(function (error) {
                                promiseReadyDeferred.reject(error);
                            });;
                            return promiseReadyDeferred.promise;

                        }

                        return directiveAPI;
                    }
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    ctrl.showGrid = true;

                    return VR_Analytic_AnalyticAPIService.GetFilteredRecords(dataRetrievalInput)
                        .then(function (response) {

                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    drillDown.setDrillDownExtensionObject(response.Data[i]);
                                }
                                if (dataRetrievalInput.Query.WithSummary)
                                    gridApi.setSummary(response.Summary);
                            }
                            onResponseReady(response);
                        });
                };

                function loadGrid(payLoad) {
                    var filters = payLoad.DimensionFilters;
                    var queryFinalized = loadGridQuery(payLoad);


                    for (var i = 0; i < ctrl.groupingDimensions.length; i++)
                    {
                        var groupingDimenion = ctrl.groupingDimensions[i];
                        var dimensionConfig = UtilsService.getItemByVal(ctrl.dimensionsConfig, groupingDimenion.DimensionName, "Name");
                        if(dimensionConfig !=undefined)
                        {
                            groupingDimenion.Type = dimensionConfig.Attribute.Type;
                            groupingDimenion.NumberPrecision = dimensionConfig.Attribute.NumberPrecision;
                        }
                    }
                    for (var i = 0; i < ctrl.measures.length; i++) {
                        var measure = ctrl.measures[i];
                        var measureConfig = UtilsService.getItemByVal(ctrl.measuresConfig, measure.MeasureName, "Name");
                        if (measureConfig != undefined) {
                            measure.Type = measureConfig.Attribute.Type;
                            measure.NumberPrecision = measureConfig.Attribute.NumberPrecision;

                        }
                    }

                    applyDimensionRules();

                    var drillDownDefinitions = [];

                    for (var i = 0; i < ctrl.drillDownDimensions.length; i++) {
                        var selectedDimensions = [];
                        var dimension = ctrl.drillDownDimensions[i];
                        for (var j = 0; j < ctrl.groupingDimensions.length; j++)
                            if (ctrl.groupingDimensions[j].DimensionName != dimension.DimensionName)
                                selectedDimensions.push(ctrl.groupingDimensions[j].DimensionName);
                        setDrillDownData(ctrl.drillDownDimensions[i], selectedDimensions)
                    }

                    function setDrillDownData(dimension, selectedDimensions) {
                        var objData = {};
                        objData.title = dimension.Title;
                        var drillDownDimensions = [];
                        for (var i = 0; i < ctrl.drillDownDimensions.length; i++) {
                            drillDownDimensions.push(ctrl.drillDownDimensions[i]);
                        }
                        objData.directive = "vr-analytic-datagrid-analyticrecords";

                        objData.loadDirective = function (directiveAPI, dataItem) {

                            dataItem.gridAPI = directiveAPI;

                            //UpdateFilters
                            var newFilters = [];
                            for (var j = 0; j < ctrl.groupingDimensions.length; j++) {
                                newFilters.push({
                                    Dimension: ctrl.groupingDimensions[j].DimensionName,
                                    FilterValues: [dataItem.DimensionValues[j].Value]
                                });
                            }
                            for (var i = 0; i < filters.length; i++)
                                newFilters.push(filters[i]);

                            //Remove Current Dimension from DrillDownDimensions
                            if (UtilsService.contains(drillDownDimensions, dimension)) {
                                var dimensionIndex = UtilsService.getItemIndexByVal(drillDownDimensions, dimension.DimensionName, 'DimensionName');
                                drillDownDimensions.splice(dimensionIndex, 1);
                            }

                            var drillDownPayLoad = {
                                DimensionsConfig: ctrl.dimensionsConfig,
                                MeasuresConfig:ctrl.measuresConfig,
                                Dimensions: ctrl.dimensions,
                                GroupingDimensions: [dimension],
                                DimensionFilters: newFilters,
                                Measures: ctrl.measures,
                                FromTime: fromTime,
                                ToTime: toTime,
                                DrillDownDimensions: drillDownDimensions,
                                TableId: payLoad.TableId
                            }
                            return dataItem.gridAPI.load(drillDownPayLoad);
                        };

                        drillDownDefinitions.push(objData);
                    }

                    drillDown = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, undefined);
                    return gridApi.retrieveData(queryFinalized);
                }

                function loadGridQuery(payLoad) {

                    fromTime = payLoad.FromTime;
                    toTime = payLoad.ToTime;
                    ctrl.groupingDimensions.length = 0;
                    ctrl.measures.length = 0;
                    ctrl.drillDownDimensions.length = 0;

                    if (payLoad.Dimensions != undefined) {
                        ctrl.dimensions = payLoad.Dimensions;
                    }
                    if (payLoad.DimensionsConfig != undefined) {
                        ctrl.dimensionsConfig = payLoad.DimensionsConfig;
                    }
                    if (payLoad.DrillDownDimensions != undefined) {
                        for (var i = 0; i < payLoad.DrillDownDimensions.length; i++) {
                            ctrl.drillDownDimensions.push(payLoad.DrillDownDimensions[i]);
                        }

                    }

                    if (payLoad.Settings != undefined) {
                        if (payLoad.Settings.Dimensions != undefined) {
                            ctrl.dimensions = payLoad.Settings.Dimensions;
                            for (var i = 0; i < payLoad.Settings.Dimensions.length; i++) {
                                var dimension = payLoad.Settings.Dimensions[i];
                                var groupingDimension = UtilsService.getItemByVal(payLoad.GroupingDimensions, dimension.DimensionName, 'DimensionName');
                                if (dimension.IsRootDimension || (payLoad.Settings.RootDimensionsFromSearchSection && groupingDimension != undefined)) {
                                    ctrl.groupingDimensions.push(dimension);

                                }
                                else {
                                    ctrl.drillDownDimensions.push(dimension);
                                }

                            }
                        }
                        if (payLoad.Settings.Measures != undefined) {
                            for (var i = 0; i < payLoad.Settings.Measures.length; i++) {
                                ctrl.measures.push(payLoad.Settings.Measures[i]);
                            }
                        }
                    }
                    else {
                        for (var i = 0; i < payLoad.Measures.length; i++) {
                            ctrl.measures.push(payLoad.Measures[i]);
                        }
                        if (payLoad.GroupingDimensions != undefined) {
                            for (var i = 0; i < payLoad.GroupingDimensions.length; i++) {
                                ctrl.groupingDimensions.push(UtilsService.getItemByVal(ctrl.dimensions, payLoad.GroupingDimensions[i].DimensionName, 'DimensionName'));
                            }
                        }
                    }

                    if (ctrl.groupingDimensions.length > 0)
                        ctrl.sortField = 'DimensionValues[0].Name';
                    else
                        ctrl.sortField = 'MeasureValues.' + ctrl.measures[0];

                    var queryFinalized = {
                        Filters: payLoad.DimensionFilters,
                        DimensionFields: UtilsService.getPropValuesFromArray(ctrl.groupingDimensions, 'DimensionName'),
                        MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                        FromTime: fromTime,
                        ToTime: toTime,
                        Currency: payLoad.Currency,
                        WithSummary: payLoad.Settings == undefined ? false : payLoad.Settings.WithSummary,
                        TableId: payLoad.TableId
                    }
                    return queryFinalized;
                }

                function applyDimensionRules() {
                    for (var i = 0; i < ctrl.dimensions.length; i++) {
                        var dimension = ctrl.dimensions[i];
                        if (dimension != undefined) {
                         
                            var dimensionConfig = UtilsService.getItemByVal(ctrl.dimensionsConfig, dimension.DimensionName, 'Name');
                            if (dimensionConfig != undefined && dimensionConfig.RequiredParentDimension  != undefined) {
                                var groupingDimension = UtilsService.getItemByVal(ctrl.groupingDimensions, dimensionConfig.RequiredParentDimension, 'DimensionName');

                                if (groupingDimension == undefined) {
                                    var groupingDimensionIndex = UtilsService.getItemIndexByVal(ctrl.drillDownDimensions, dimensionConfig.Name, 'DimensionName');
                                    ctrl.drillDownDimensions.splice(groupingDimensionIndex, 1);
                                } else if (ctrl.drillDownDimensions.indexOf(dimension) == -1) {
                                    ctrl.drillDownDimensions.push(dimension);
                                }
                            } else if (dimensionConfig.RequiredParentDimension != undefined) {
                                var groupingDimensionIndex = UtilsService.getItemIndexByVal(ctrl.drillDownDimensions, dimensionConfig.RequiredParentDimension, 'DimensionName');
                                ctrl.drillDownDimensions.splice(groupingDimensionIndex, 1);
                            }
                        }
                    }
                }

                function loadDimensionsConfig()
                {
                    var dimensionsFilter = {
                        TableIds: [tableId]
                    }
                  return  VR_Analytic_AnalyticItemConfigAPIService.GetDimensionsInfo(UtilsService.serializetoJson(dimensionsFilter)).then(function (response) {
                      if (response) {
                          for (var i = 0; i < response.length; i++) {
                              ctrl.dimensionsConfig.push(response[i]);
                          }
                      };
                    });
                }

                function loadMeasuresConfig()
                {
                    var measuresFilter = {
                        TableIds: [tableId]
                    }
                    return VR_Analytic_AnalyticItemConfigAPIService.GetMeasuresInfo(UtilsService.serializetoJson(measuresFilter)).then(function (response) {
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.measuresConfig.push(response[i]);
                            }
                        };
                    });
                }
            }
        }
        return directiveDefinitionObject;
    }
]);