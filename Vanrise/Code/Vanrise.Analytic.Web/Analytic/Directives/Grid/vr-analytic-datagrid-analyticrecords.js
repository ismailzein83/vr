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

            ctrl.datasource = [];
            ctrl.fromTime;
            ctrl.toTime;
            ctrl.dimensions = [];
            ctrl.measures = [];
            ctrl.parameters;
            ctrl.sortField = "";
            var gridApi;
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
                            ctrl.dimensions.length = 0;
                            var queryFinalized = loadGridQuery(query);
                            var drillDownDefinitions = [];

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

                    ctrl.fromTime = query.FromTime;
                    ctrl.toTime = query.ToTime;
                    if (query.MeasureThreshold != undefined) {
                        ctrl.parameters = query.MeasureThreshold;
                        ctrl.mainGrid = false;
                    }

                    if (query.DimensionFields != undefined) {

                        ctrl.dimensions = query.DimensionFields;
                    }

                    for (var i = 0; i < query.MeasureFields.length; i++)
                        ctrl.measures.push(query.MeasureFields[i]);

                    //for (var i = 0; i < query.Dimensions.length; i++)
                    //    ctrl.dimensions.push(query.Dimensions[i]);
                    console.log(ctrl.dimensions);
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
                        ctrl.sortField = 'MeasureValues.' + ctrl.measures[0];
                    return queryFinalized;
                }
            }
        }
        return directiveDefinitionObject;
    }
]);