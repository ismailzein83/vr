(function (appControllers) {

    'use strict';

    VariationReportController.$inject = ['$scope', 'WhS_Analytics_VariationReportTypeEnum', 'WhS_Analytics_VariationReportTimePeriodEnum', 'UtilsService'];

    function VariationReportController($scope, WhS_Analytics_VariationReportTypeEnum, WhS_Analytics_VariationReportTimePeriodEnum, UtilsService) {

        var gridAPI;
        var pageSize;
        var chartAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            var reportTypes = UtilsService.getArrayEnum(WhS_Analytics_VariationReportTypeEnum);
            $scope.scopeModel.reportTypes = UtilsService.getFilteredArrayFromArray(reportTypes, true, 'isVisible');
            $scope.scopeModel.selectedReportType = UtilsService.getEnum(WhS_Analytics_VariationReportTypeEnum, 'value', WhS_Analytics_VariationReportTypeEnum.InBoundMinutes.value);

            $scope.scopeModel.toDate = new Date('2016-01-04');

            $scope.scopeModel.periodTypes = UtilsService.getArrayEnum(WhS_Analytics_VariationReportTimePeriodEnum);
            $scope.scopeModel.selectedPeriodType = UtilsService.getEnum(WhS_Analytics_VariationReportTimePeriodEnum, 'value', WhS_Analytics_VariationReportTimePeriodEnum.Daily.value);

            $scope.scopeModel.numberOfPeriods = 3;
            $scope.scopeModel.showGrid = false;
            $scope.scopeModel.showChart = false;

            $scope.scopeModel.top = 5;

            $scope.scopeModel.onReportTypeSelectionChanged = function (selectedReportType) {
                $scope.scopeModel.showGrid = false;
                $scope.scopeModel.showChart = false;
            };

            $scope.scopeModel.onGridReady = function (api)
            {
                gridAPI = api;
                pageSize = api.getPageSize();
            };

            $scope.scopeModel.onChartReady = function (api) {
                chartAPI = api;
            };

            $scope.scopeModel.validateTopFilter = function () {
                if (pageSize == undefined) // Handle the case when/if this function executes before onGridReady in which case pageSize = undefined
                    return null;
                return ($scope.scopeModel.top > pageSize) ? ('Top <= ' + pageSize) : null;
            };

            $scope.scopeModel.search = function () {
                $scope.scopeModel.showGrid = true;
                return loadAllControls();
            };
        }

        function load() {
            
        }

        function loadAllControls() {
            var promises = [];

            var loadGridPromise = loadGrid();
            promises.push(loadGridPromise);

            var loadChartDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadChartDeferred.promise);

            loadGridPromise.then(function () {
                loadChart();
                loadChartDeferred.resolve();
            });

            return UtilsService.waitMultiplePromises(promises);
        }

        function loadGrid() {
            return gridAPI.load(getGridQuery());
        }

        function getGridQuery() {
            return {
                ReportType: $scope.scopeModel.selectedReportType.value,
                ToDate: $scope.scopeModel.toDate,
                TimePeriod: $scope.scopeModel.selectedPeriodType.value,
                NumberOfPeriods: $scope.scopeModel.numberOfPeriods,
                GroupByProfile: $scope.scopeModel.groupByProfile
            };
        }

        function loadChart()
        {
            switch ($scope.scopeModel.selectedReportType.value)
            {
                case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                    return;
            }

            $scope.scopeModel.showChart = true;

            var reportData = gridAPI.getData();
            
            var chartConfig =
            {
                type: 'column',
                yAxisTitle: getReportResultTypeDescription()
            };

            var seriesConfig = { titlePath: 'seriesTitle' };

            var seriesList = [];

            seriesList.push
            ({
                seriesTitle: 'AVG',
                seriesData: []
            });

            for (var i = 0; i < reportData.timePeriods.length; i++)
            {
                var seriesItem =
                {
                    seriesTitle: reportData.timePeriods[i].PeriodDescription,
                    seriesData: []
                };
                seriesList.push(seriesItem);
            }

            var seriesDefinition = [];
            for (var i = 0; i < reportData.records.length && i < $scope.scopeModel.top; i++)
            {
                var record = reportData.records[i];

                seriesDefinition.push
                ({
                    title: record.DimensionName,
                    valuePath: 'seriesData[' + i + ']',
                    type: 'column'
                });

                seriesList[0].seriesData[i] = record.Average;

                for (var j = 1; j <= reportData.timePeriods.length; j++)
                {
                    seriesList[j].seriesData[i] = record.TimePeriodValues[j - 1];
                }
            }

            

            chartAPI.renderChart(seriesList, chartConfig, seriesDefinition, seriesConfig);
        }

        function getReportResultTypeDescription()
        {
            switch ($scope.scopeModel.selectedReportType.value)
            {
                case WhS_Analytics_VariationReportTypeEnum.InBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.OutBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.TopDestinationMinutes.value:
                    return 'Duration';

                case WhS_Analytics_VariationReportTypeEnum.InBoundAmount.value:
                case WhS_Analytics_VariationReportTypeEnum.OutBoundAmount.value:
                case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                case WhS_Analytics_VariationReportTypeEnum.TopDestinationAmount.value:
                    return 'Amount';

                case WhS_Analytics_VariationReportTypeEnum.Profit.value:
                case WhS_Analytics_VariationReportTypeEnum.OutBoundProfit.value:
                case WhS_Analytics_VariationReportTypeEnum.TopDestinationProfit.value:
                    return 'Profit';
            }
        }
    }

    appControllers.controller('WhS_Analytics_VariationReportController', VariationReportController);

})(appControllers);