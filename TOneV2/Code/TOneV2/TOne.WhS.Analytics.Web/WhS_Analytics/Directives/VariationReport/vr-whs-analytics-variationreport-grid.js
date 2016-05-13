(function (app) {

    'use strict';

    VariationReportGridDirective.$inject = ['WhS_Analytics_VariationReportAPIService', 'WhS_Analytics_VariationReportDimensionEnum', 'WhS_Analytics_VariationReportTypeEnum', 'WhS_Analytics_VariationReportTimePeriodEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VariationReportGridDirective(WhS_Analytics_VariationReportAPIService, WhS_Analytics_VariationReportDimensionEnum, WhS_Analytics_VariationReportTypeEnum, WhS_Analytics_VariationReportTimePeriodEnum, UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var variationReportGrid = new VariationReportGrid($scope, ctrl, $attrs);
                variationReportGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Analytics/Directives/VariationReport/Templates/VariationReportGridTemplate.html'
        };

        function VariationReportGrid($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;
            var gridQuery;
            var firstResponse;
            var gridDrillDownTabsObj;
            var reportTypeObjs;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.sortByField = 'Average';
                $scope.scopeModel.dataSource = [];
                $scope.scopeModel.dimensionTitle;
                $scope.scopeModel.timePeriodDefinitions = [];
                $scope.scopeModel.drillDownDimensionValues = [];

                reportTypeObjs = UtilsService.getArrayEnum(WhS_Analytics_VariationReportTypeEnum);

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Analytics_VariationReportAPIService.GetFilteredVariationReportRecords(dataRetrievalInput).then(function (response)
                    {
                        if (response != null && response.Data != null) {

                            if (firstResponse == undefined) {
                                firstResponse = response;
                            }

                            if (gridDrillDownTabsObj == undefined) {
                                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(getDrillDownDefinitions(response.DrillDownDimensions), gridAPI, undefined);
                            }

                            $scope.scopeModel.dimensionTitle = response.DimensionTitle;

                            for (var i = 0; i < response.Data.length; i++) {
                                if (response.Data[i].Dimension != null) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }

                            if (response.Summary != null) {
                                gridAPI.setSummary(response.Summary);
                            }

                            if (response.TimePeriods != null) {
                                $scope.scopeModel.timePeriodDefinitions.length = 0;
                                for (var i = 0; i < response.TimePeriods.length; i++) {
                                    $scope.scopeModel.timePeriodDefinitions.push(response.TimePeriods[i]);
                                }
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                $scope.scopeModel.showExpandIcon = function (dataItem) {
                    return (dataItem.Dimension != null);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (query)
                {
                    setSortByField(query.ReportType);

                    gridQuery = cloneGridQuery(query);
                    gridDrillDownTabsObj = undefined;
                    firstResponse = undefined;

                    return gridAPI.retrieveData(query);
                };

                api.getData = function () {
                    return (firstResponse != undefined) ? {
                        dimensionTitle: firstResponse.DimensionTitle,
                        records: firstResponse.Data,
                        timePeriods: firstResponse.TimePeriods,
                        summary: firstResponse.Summary
                    } : null;
                };

                api.getPageSize = function () {
                    return gridAPI.getPageSize();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function cloneGridQuery(gridQueryObj) {
                    return {
                        ReportType: gridQueryObj.ReportType,
                        ParentDimensions: gridQueryObj.ParentDimensions,
                        ToDate: gridQueryObj.ToDate,
                        TimePeriod: gridQueryObj.TimePeriod,
                        NumberOfPeriods: gridQueryObj.NumberOfPeriods,
                        GroupByProfile: gridQueryObj.GroupByProfile
                    };
                }
            }

            function setSortByField(reportTypeValue) {
                switch (reportTypeValue) {
                    case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                    case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                        $scope.scopeModel.sortByField = 'DimensionName';
                        break;
                    default:
                        $scope.scopeModel.sortByField = 'Average';
                        break;
                }
            }

            function getDrillDownDefinitions(drillDownDimensions) {

                $scope.scopeModel.drillDownDimensionValues.length = 0;

                if (drillDownDimensions != undefined) {
                    for (var i = 0; i < drillDownDimensions.length; i++) {
                        $scope.scopeModel.drillDownDimensionValues.push(drillDownDimensions[i]);
                    }
                }

                var drillDownTabDefinitions = [];

                for (var i = 0; i < $scope.scopeModel.drillDownDimensionValues.length; i++) {
                    drillDownTabDefinitions.push(getDrillDownDefinition($scope.scopeModel.drillDownDimensionValues[i]));
                }

                return drillDownTabDefinitions;
            }
            function getDrillDownDefinition(drillDownDimensionValue) {
                return {
                    title: getDimensionPluralDescription(drillDownDimensionValue),
                    directive: 'vr-whs-analytics-variationreport-grid',
                    loadDirective: function (directiveAPI, dataItem) {

                        var parentDimensions = (gridQuery.ParentDimensions != null) ? UtilsService.cloneObject(gridQuery.ParentDimensions, true) : [];

                        parentDimensions.push({
                            Dimension: dataItem.Dimension,
                            Value: dataItem.DimensionId
                        });

                        var directiveQuery = {
                            ReportType: getDrillDownReportTypeValue(drillDownDimensionValue),
                            ParentDimensions: parentDimensions,
                            ToDate: gridQuery.ToDate,
                            TimePeriod: gridQuery.TimePeriod,
                            NumberOfPeriods: gridQuery.NumberOfPeriods,
                            GroupByProfile: gridQuery.GroupByProfile
                        };

                        return directiveAPI.load(directiveQuery);
                    }
                };
            }
            function getDimensionPluralDescription(dimensionValue) {
                return UtilsService.getEnum(WhS_Analytics_VariationReportDimensionEnum, 'value', dimensionValue).pluralDescription;
            }
            function getDrillDownReportTypeValue(drillDownDimensionValue) {

                var currentReportTypeCategory = UtilsService.getItemByVal(reportTypeObjs, gridQuery.ReportType, 'value').category;

                for (var i = 0; i < reportTypeObjs.length; i++) {
                    var item = reportTypeObjs[i];
                    if (item.category == currentReportTypeCategory && item.value != gridQuery.ReportType && item.dimensionValue == drillDownDimensionValue)
                        return item.value;
                }
            }
        }
    }

    app.directive('vrWhsAnalyticsVariationreportGrid', VariationReportGridDirective);

})(app);