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

            function initializeController() {

                ctrl.mainGrid = (ctrl.parameters == undefined);

                ctrl.gridReady = function (api) {
                    gridApi = api;
                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.loadGrid = function (payLoad) {

                            var filters = payLoad.Filters;
                            var queryFinalized = loadGridQuery(payLoad);
                            fromTime = payLoad.FromTime;
                            toTime = payLoad.ToTime;

                            var drillDownDefinitions = [];

                            for (var i = 0; i < payLoad.groupingDimensions.length; i++) {
                                var selectedDimensions = [];
                                var dimension = payLoad.groupingDimensions[i];
                                for (var j = 0; j < ctrl.groupingDimensions.length; j++)
                                    if (ctrl.groupingDimensions[j].DimensionName != dimension.DimensionName)
                                        selectedDimensions.push(ctrl.groupingDimensions[j].DimensionName);
                                setDrillDownData(payLoad.groupingDimensions[i], selectedDimensions)
                            }

                            function setDrillDownData(dimension, selectedDimensions) {
                                var objData = {};

                                objData.title = dimension.Title;

                                objData.directive = "vr-analytic-datagrid-analyticrecords";

                                objData.loadDirective = function (directiveAPI, dataItem) {

                                    dataItem.gridAPI = directiveAPI;

                                    //UpdateFilters
                                    var newFilters = [];
                                    for (var j = 0; j < ctrl.groupingDimensions.length; j++) {
                                        newFilters.push({
                                            Dimension: ctrl.groupingDimensions[j].DimensionName,
                                            FilterValues: [dataItem.DimensionValues[j].Id]
                                        });
                                    }
                                    for (var i = 0; i < filters.length; i++)
                                        newFilters.push(filters[i]);

                                    var drillDownPayLoad = {
                                        groupingDimensions: [dimension],
                                        dimensionsFilter: newFilters,
                                        measures: ctrl.measures,
                                        fromTime: fromTime,
                                        toTime: toTime
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
                            for (var i = 0; i < response.Data.length; i++) {
                                drillDown.setDrillDownExtensionObject(response.Data[i]);
                            }
                            onResponseReady(response);
                        });
                };

                function loadGridQuery(payLoad) {
                    var dimensionFilters = [];
                    var dimensions = [];
                    ctrl.groupingDimensions = payLoad.groupingDimensions;
                    ctrl.measures = payLoad.measures;

                    if (payLoad.Settings != undefined) {

                    }

                    if (payLoad.groupingDimensions != undefined) {

                    }

                    var queryFinalized = {
                        Filters: payLoad.Filters,
                        DimensionFields: UtilsService.getPropValuesFromArray(ctrl.groupingDimensions, 'DimensionName'),
                        MeasureFields: ctrl.measures,
                        FromTime: payLoad.FromTime,
                        ToTime: payLoad.ToTime,
                        Currency: payLoad.Currency,
                        WithSummary: $attrs.withsummary != undefined
                    }

                    if (payLoad.groupingDimensions.length > 0)
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