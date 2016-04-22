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

                        directiveAPI.loadGrid = function (payLoad) {
                            isSummary = $attrs.withsummary != undefined;
                            ctrl.drillDownDimensions.length = 0;
                            if (payLoad.DrillDownDimensions != undefined) {
                                for (var i = 0; i < payLoad.DrillDownDimensions.length; i++) {
                                    ctrl.drillDownDimensions.push(payLoad.DrillDownDimensions[i]);
                                }
                            }
                            var filters = payLoad.DimensionFilters;
                            var queryFinalized = loadGridQuery(payLoad);

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
                                        groupingDimensions: [dimension],
                                        dimensionFilters: newFilters,
                                        measures: ctrl.measures,
                                        FromTime: fromTime,
                                        ToTime: toTime,
                                        DrillDownDimensions: drillDownDimensions
                                    }
                                    return dataItem.gridAPI.loadGrid(drillDownPayLoad);
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
                    console.log(dataRetrievalInput);
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
                    ctrl.groupingDimensions.length = 0;
                    ctrl.measures.length = 0;

                    if (payLoad.Settings != undefined) {
                        if (payLoad.Settings.Dimensions != undefined) {
                            for (var i = 0; i < payLoad.Settings.Dimensions.length; i++) {
                                var dimension = payLoad.Settings.Dimensions[i];
                                var groupingDimension = UtilsService.getItemByVal(payLoad.GroupingDimensions, dimension.DimensionName, 'DimensionName');
                                if (groupingDimension == undefined) {
                                    ctrl.drillDownDimensions.push(dimension);
                                }
                            }
                        }
                    }

                    if (payLoad.Measures != undefined) {
                        for (var i = 0; i < payLoad.Measures.length; i++) {
                            ctrl.measures.push(payLoad.Measures[i]);
                        }
                    }

                    if (payLoad.GroupingDimensions != undefined) {
                        for (var i = 0; i < payLoad.GroupingDimensions.length; i++) {
                            ctrl.groupingDimensions.push(payLoad.GroupingDimensions[i]);
                        }
                    }

                    var queryFinalized = {
                        Filters: payLoad.DimensionFilters,
                        DimensionFields: UtilsService.getPropValuesFromArray(ctrl.groupingDimensions, 'DimensionName'),
                        MeasureFields: UtilsService.getPropValuesFromArray(ctrl.measures, 'MeasureName'),
                        FromTime: fromTime,
                        ToTime: toTime,
                        Currency: payLoad.Currency,
                        WithSummary: isSummary
                    }

                    if (payLoad.GroupingDimensions.length > 0)
                        ctrl.sortField = 'DimensionValues[0].Name';
                    else
                        ctrl.sortField = 'MeasureValues.' + ctrl.measures[0];
                    return queryFinalized;
                }
            }
        }
        return directiveDefinitionObject;
    }
]);