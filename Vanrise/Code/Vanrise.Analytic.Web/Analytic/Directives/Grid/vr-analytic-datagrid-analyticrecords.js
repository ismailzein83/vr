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

            ctrl.selectedMeasures = [];
            ctrl.selectedDimensions = [];
            ctrl.datasource = [];
            ctrl.selectedPeriods = [];
            ctrl.fromTime;
            ctrl.toTime;
            ctrl.dimensions = [];
            ctrl.measures = [];
            ctrl.dimensionFields = [];
            ctrl.parameters;
            ctrl.sortField = "";
            var gridApi;
            var measureValues = [];
            var dimensionValues = [];
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
                            ctrl.selectedPeriods.length = 0;
                            ctrl.selectedMeasures.length = 0;
                            ctrl.dimensions.length = 0;
                            ctrl.dimensionFields.length = 0;

                            var queryFinalized = loadGridQuery(query);

                            var drillDownDefinitions = [];

                            applyDimentionsRules(ctrl.selectedDimensions, ctrl.dimensions);

                            for (var i = 0; i < ctrl.dimensions.length; i++) {
                                var selectedDimensions = [];
                                var dimention = ctrl.dimensions[i];
                                for (var j = 0; j < ctrl.selectedDimensions.length; j++)
                                    if (ctrl.selectedDimensions[j].value != dimention.value)
                                        selectedDimensions.push(ctrl.selectedDimensions[j].value);
                            }

                            return gridApi.retrieveData(queryFinalized);
                        }

                        return directiveAPI;
                    }

                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    console.log(dataRetrievalInput);
                    ctrl.showGrid = true;

                    return VR_Analytic_AnalyticConfigurationAPIService.GetAnalyticRecords(dataRetrievalInput)
                        .then(function (response) {
                            if (isSummary)
                                gridApi.setSummary(response.Summary);
                            onResponseReady(response);
                        });
                };

                ctrl.checkExpandablerow = function (groupKeys) {
                    return groupKeys.length !== ctrl.groupKeys.length;
                };

                function loadGridQuery(query) {
                    ctrl.measures.length = 0;
                    ctrl.dimensions.length = 0;
                    
                    dimensionValues.length = 0;
                    ctrl.fromTime = query.FromTime;
                    ctrl.toTime = query.ToTime;
                    if (query.MeasureThreshold != undefined) {
                        ctrl.parameters = query.MeasureThreshold;
                        ctrl.mainGrid = false;
                    }

                    if (query.DimensionsSelected != undefined) {

                    }

                    if (query.DimensionFields != undefined) {

                        dimensionValues = query.DimensionFields;
                    }

                    if (query.FixedDimensionFields != undefined) {

                    }

                    for (var i = 0; i < query.MeasureFields.length; i++)
                        ctrl.measures.push(query.MeasureFields[i].Name);

                    for (var i = 0; i < query.Dimensions.length; i++)
                        ctrl.dimensions.push(query.Dimensions[i].Name);

                    isSummary = $attrs.withsummary != undefined;
                    var queryFinalized = {
                        Filters: query.Filters,
                        DimensionFields: ctrl.dimensions,
                        MeasureFields: ctrl.measures,
                        FromTime: query.FromTime,
                        ToTime: query.ToTime,
                        Currency: query.Currency,
                        WithSummary: isSummary
                    }
                    if (ctrl.dimensions.length > 0)
                        ctrl.sortField = 'DimensionValues[0].Name';
                    else
                        ctrl.sortField = 'MeasureValues.' + ctrl.measures[0].name;
                    return queryFinalized;
                }

                function applyDimentionsRules(selectedDimensions, dimensions) {

                }

                function getDimentionIndex(dimentionValue, dimensions) {
                    return UtilsService.getItemIndexByVal(dimensions, dimentionValue, "value");
                }

                function eliminateGroupKeysNotInParent(selectedDimensions, dimensions) {

                    for (var i = 0; i < selectedDimensions.length; i++) {
                        for (var j = 0; j < dimensions.length; j++)
                            if (selectedDimensions[i].value == dimensions[j].value)
                                dimensions.splice(j, 1);
                    }
                }


                function editSettings() {
                    var settings = {
                    };

                    settings.onScopeReady = function (modalScope) {
                        modalScope.title = UtilsService.buildTitleForUpdateEditor("Measure Threshold");
                        modalScope.onSaveSettings = function (parameters) {
                            ctrl.parameters = parameters
                        };
                    };
                    var measureThreshold = [];

                    var parameters = {
                        measureThresholds: measureThreshold
                    };

                    VRModalService.showModal('/Client/Modules/Analytic/Directives/Grid/Templates/AnalyticRecordsDataGrid.html', parameters, settings);
                }
            }

        }
        return directiveDefinitionObject;
    }
]);