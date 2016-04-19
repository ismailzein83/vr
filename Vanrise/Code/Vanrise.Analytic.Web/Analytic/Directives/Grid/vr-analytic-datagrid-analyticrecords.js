"use strict";

app.directive("vrAnalyticDatagridAnalyticrecords", ['UtilsService', 'VRNotificationService', 'Analytic_AnalyticService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VRModalService',
    function (UtilsService, VRNotificationService, Analytic_AnalyticService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService, VRModalService) {

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
            var dimensionValues = [];
            ctrl.datasource = [];
            ctrl.fromTime;
            ctrl.toTime;
            ctrl.dimensions = [];
            ctrl.measures = [];
            ctrl.selectedDimensions = [];
            ctrl.dimensionFields = [];
            ctrl.parameters;
            ctrl.sortField = "";
            var gridApi;
            var drillDown;
            var isSummary = false;

            function initializeController() {

                ctrl.mainGrid = (ctrl.parameters == undefined);

                ctrl.gridReady = function (api) {
                    gridApi = api;
                    if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.loadGrid = function (query) {
                            ctrl.filters = query.Filters;
                            var filters = query.Filters;
                            ctrl.Currency = query.Currency;

                            dimensionValues.length = 0;
                            ctrl.selectedDimensions.length = 0;
                            ctrl.dimensions.length = 0;
                            ctrl.dimensionFields.length = 0;
                            ctrl.measures.length = 0;
                            var queryFinalized = loadGridQuery(query);
                            var drillDownDefinitions = [];
                            for (var i = 0; i < query.Dimensions.length; i++) {
                                var selectedDimensions = [];
                                var dimension = query.Dimensions[i];
                                for (var j = 0; j < ctrl.selectedDimensions.length; j++)
                                    if (ctrl.selectedDimensions[j] != dimension.value) {
                                        selectedDimensions.push(ctrl.selectedDimensions[j]);
                                        setDrillDownData(query.Dimensions[i], selectedDimensions);
                                    }
                            }



                            function setDrillDownData(dimension, selectedDimensions) {
                                var objData = {};

                                objData.title = dimension.description;

                                objData.directive = "vr-analytic-datagrid-analyticrecords";

                                objData.loadDirective = function (directiveAPI, dataItem) {

                                    var selectedfilters = [];
                                    for (var j = 0; j < ctrl.dimensionFields.length; j++) {
                                        selectedfilters.push({
                                            Dimension: ctrl.dimensionFields[j],
                                            FilterValues: [dataItem.DimensionValues[j].Value]
                                        });
                                    }

                                    for (var i = 0; i < filters.length; i++)
                                        selectedfilters.push(filters[i]);

                                    dataItem.gridAPI = directiveAPI;

                                    var query = {
                                        Filters: selectedfilters,
                                        Dimensions: [dimension],
                                        DimensionFields: [dimension.value],
                                        MeasureFields: ctrl.measures,
                                        DimensionsSelected: selectedDimensions,
                                        MeasureThreshold: ctrl.parameters,
                                        FromTime: ctrl.fromTime,
                                        ToTime: ctrl.toTime,
                                        Currency: ctrl.Currency,
                                        WithSummary: isSummary
                                    }
                                    return dataItem.gridAPI.loadGrid(query);
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

                    return VR_Analytic_AnalyticConfigurationAPIService.GetFilteredRecords(dataRetrievalInput)
                        .then(function (response) {
                            for (var i = 0; i < response.Data.length; i++) {
                                drillDown.setDrillDownExtensionObject(response.Data[i]);
                            }
                            if (isSummary)
                                gridApi.setSummary(response.Summary);
                            onResponseReady(response);
                        });
                };

                ctrl.checkExpandablerow = function (groupKeys) {
                    return groupKeys.length !== ctrl.groupKeys.length;
                };

                function loadGridQuery(query) {
                    dimensionValues.length = 0;

                    ctrl.fromTime = query.FromTime;
                    ctrl.toTime = query.ToTime;
                    if (query.MeasureThreshold != undefined) {
                        ctrl.parameters = query.MeasureThreshold;
                        ctrl.mainGrid = false;
                    }

                    if (query.DimensionsSelected != undefined) {
                        for (var i = 0; i < query.DimensionsSelected.length; i++) {
                            ctrl.selectedDimensions.push(query.DimensionsSelected[i]);
                        }
                    }
                    if (query.FixedDimensionFields != undefined) {
                        for (var i = 0; i < query.FixedDimensionFields.length; i++) {
                            ctrl.dimensionFields.push(query.FixedDimensionFields[i]);
                        }
                    }
                    if (query.Dimensions != undefined) {
                        for (var i = 0; i < query.Dimensions.length; i++) {
                            if (UtilsService.contains(query.DimensionFields, query.Dimensions[i].value))
                            ctrl.dimensions.push(query.Dimensions[i]);
                    }
                }

                    if (query.DimensionFields != undefined) {
                        for (var i = 0; i < query.DimensionFields.length; i++) {
                            ctrl.dimensionFields.push(query.DimensionFields[i]);
                            ctrl.selectedDimensions.push(query.DimensionFields[i]);
                    }
                        dimensionValues = query.DimensionFields;
                }

                    for (var i = 0; i < query.MeasureFields.length; i++)
                        ctrl.measures.push(query.MeasureFields[i]);

                    console.log(ctrl.dimensions);
                    isSummary = $attrs.withsummary != undefined;

                    var queryFinalized = {
                            Filters: query.Filters,
                            DimensionFields: dimensionValues,
                            Dimensions: query.Dimensions,
                            MeasureFields: ctrl.measures,
                            FromTime: query.FromTime,
                            ToTime: query.ToTime,
                            Currency: query.Currency,
                            WithSummary: isSummary
                }

                    function GetSelectedDimensions() {
                        var result = [];
                        for (var i = 0; i < ctrl.dimensions.length; i++) {
                            if (!UtilsService.contains(dimensionValues, ctrl.dimensions[i].value))
                                result.push(ctrl.dimensions[i]);
                    }
                        return result;
                }

                    if (ctrl.dimensions.length > 0)
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