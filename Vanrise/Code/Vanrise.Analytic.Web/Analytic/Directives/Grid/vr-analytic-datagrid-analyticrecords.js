"use strict";

app.directive("vrAnalyticDatagridAnalyticrecords", ['UtilsService', 'VRNotificationService', 'Analytic_AnalyticService', 'VRUIUtilsService', 'VR_Analytic_AnalyticAPIService', 'VRModalService',
    function (UtilsService, VRNotificationService, Analytic_AnalyticService, VRUIUtilsService, VR_Analytic_AnalyticAPIService, VRModalService) {

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
            templateUrl: "/Client/Modules/Analytic/Directives/Grid/Templates/AnalyticRecordsDataGrid.html"

        };

        function GenericGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            ctrl.datasource = [];
            ctrl.dimensions = [];
            ctrl.groupingDimensions = [];
            ctrl.measures = [];
            ctrl.drillDownDimensions = [];
            ctrl.sortField = "";
            var gridApi;
            var drillDown;
            var fromTime;
            var toTime;
            var isSummary;

            function initializeController() {

                ctrl.mainGrid = (ctrl.parameters == undefined);

                ctrl.gridReady = function (api) {
                    gridApi = api;
                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.load = function (payLoad) {

                            var filters = payLoad.DimensionFilters;
                            var queryFinalized = loadGridQuery(payLoad);

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
                                        Dimensions:ctrl.dimensions,
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
                                if (isSummary)
                                    gridApi.setSummary(response.Summary);
                            }
                            onResponseReady(response);
                        });
                };

                function loadGridQuery(payLoad) {

                    fromTime = payLoad.FromTime;
                    toTime = payLoad.ToTime;
                    isSummary = $attrs.withsummary != undefined;
                    ctrl.groupingDimensions.length = 0;
                    ctrl.measures.length = 0;
                    ctrl.drillDownDimensions.length = 0;

                    if (payLoad.Dimensions != undefined)
                    {
                        ctrl.dimensions = payLoad.Dimensions;
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
                                if (dimension.IsRootDimension || (payLoad.Settings.RootDimensionsFromSearchSection && groupingDimension != undefined))
                                {
                                    ctrl.groupingDimensions.push(dimension);

                                }
                                else
                                {
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
                                ctrl.groupingDimensions.push( UtilsService.getItemByVal(ctrl.dimensions,payLoad.GroupingDimensions[i].DimensionName, 'DimensionName'));
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
                        WithSummary: isSummary,
                        TableId: payLoad.TableId
                    }
                    return queryFinalized;
                }

                function applyDimensionRules() {
                    for (var i = 0; i < ctrl.groupingDimensions; i++) {
                        var groupingDimension = ctrl.groupingDimension[i];
                        if (groupingDimension.ParentDimension)
                        {
                           //var parentDimension = UtilsService.getItemIndexByVal()
                        }
                    }
                }
            }
        }
        return directiveDefinitionObject;
    }
]);