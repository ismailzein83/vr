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
            var gridDrillDownTabsObj;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.sortByField = 'Average';
                $scope.scopeModel.dataSource = [];
                $scope.scopeModel.dimensionTitle;
                $scope.scopeModel.timePeriodDefinitions = [];
                $scope.scopeModel.drillDownDimensionValues = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_Analytics_VariationReportAPIService.GetFilteredVariationReportRecords(dataRetrievalInput).then(function (response) {
                        if (response != null) {

                            if (gridDrillDownTabsObj == undefined) {
                                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(getDrillDownDefinitions(response.DrillDownDimensions), gridAPI, undefined);
                            }

                            $scope.scopeModel.dimensionTitle = response.DimensionTitle;

                            for (var i = 0; i < response.Data.length; i++) {
                                if (response.Data[i].Dimension != null) {
                                    gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                                }
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
                    console.log('bla');
                    return (dataItem.Dimension != null);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    setSortByField(query.ReportType);
                    gridQuery = cloneGridQuery(query);
                    gridDrillDownTabsObj = undefined;
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function cloneGridQuery(gridQueryObj) {
                    return {
                        ParentReportType: gridQueryObj.ParentReportType,
                        ReportType: gridQueryObj.ReportType,
                        ToDate: gridQueryObj.ToDate,
                        TimePeriod: gridQueryObj.TimePeriod,
                        NumberOfPeriods: gridQueryObj.NumberOfPeriods,
                        DimensionFilters: gridQueryObj.DimensionFilters
                    };
                }
            }

            function setSortByField(reportType) {
                switch (reportType) {
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

                return drillDownTabDefinitions
            }
            function getDrillDownDefinition(drillDownDimensionValue) {
                return {
                    title: getDimensionPluralDescription(drillDownDimensionValue),
                    directive: 'vr-whs-analytics-variationreport-grid',
                    loadDirective: function (directiveAPI, dataItem) {

                        var dimensionFilters = (gridQuery.DimensionFilters != null) ? UtilsService.cloneObject(gridQuery.DimensionFilters, true) : [];

                        dimensionFilters.push({
                            Dimension: dataItem.Dimension,
                            FilterValues: [dataItem.DimensionId]
                        });

                        var directiveQuery = {
                            ParentReportType: gridQuery.ReportType,
                            ReportType: getDrillDownReportTypeValue(drillDownDimensionValue),
                            ToDate: gridQuery.ToDate,
                            TimePeriod: gridQuery.TimePeriod,
                            NumberOfPeriods: gridQuery.NumberOfPeriods,
                            DimensionFilters: dimensionFilters
                        };

                        return directiveAPI.load(directiveQuery);
                    }
                };
            }
            function getDimensionPluralDescription(dimensionValue) {
                return UtilsService.getEnum(WhS_Analytics_VariationReportDimensionEnum, 'value', dimensionValue).pluralDescription;
            }
            function getDrillDownReportTypeValue(drillDownDimensionValue) {

                var currentReportTypeCategory = UtilsService.getEnum(WhS_Analytics_VariationReportTypeEnum, 'value', gridQuery.ReportType).category;

                for (var propertyName in WhS_Analytics_VariationReportTypeEnum) {
                    var reportTypeObj = WhS_Analytics_VariationReportTypeEnum[propertyName];
                    if (reportTypeObj.category == currentReportTypeCategory && reportTypeObj.value != gridQuery.ReportType && reportTypeObj.dimensionValue == drillDownDimensionValue)
                        return reportTypeObj.value;
                }
            }

            function junkCode() {
                function setDrillDownDimensionValues(gridQuery) {

                    $scope.scopeModel.drillDownDimensionValues.length = 0;

                    if (gridQuery.parentReportType != undefined) {
                        switch (gridQuery.parentReportType) {
                            case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                            case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                                return; // => gridQuery.ReportType = Zone
                        }
                    }

                    switch (gridQuery.ReportType) {
                        case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                        case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                            $scope.scopeModel.drillDownDimensionValues.push(WhS_Analytics_VariationReportDimensionEnum.Zone.value);
                            return;
                    }

                    var currentReportDimensionValue = getDimensionValue(gridQuery.ReportType);
                    var filterDimensionValues = (gridQuery.DimensionFilters != null) ? UtilsService.getPropValuesFromArray(gridQuery.DimensionFilters, 'Dimension') : [];

                    for (var propertyName in WhS_Analytics_VariationReportDimensionEnum) {
                        var dimensionValue = WhS_Analytics_VariationReportDimensionEnum[propertyName].value;
                        if (dimensionValue != currentReportDimensionValue && !UtilsService.contains(filterDimensionValues, dimensionValue))
                            $scope.scopeModel.drillDownDimensionValues.push(dimensionValue);
                    }
                }
                function getDimensionValue(reportTypeValue) {
                    return UtilsService.getEnum(WhS_Analytics_VariationReportTypeEnum, 'value', reportTypeValue).dimensionValue;
                }
            }
        }
    }

    app.directive('vrWhsAnalyticsVariationreportGrid', VariationReportGridDirective);

})(app);