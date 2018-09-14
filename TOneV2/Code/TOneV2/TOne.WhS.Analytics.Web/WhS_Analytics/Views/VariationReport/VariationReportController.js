(function (appControllers) {

    'use strict';

    VariationReportController.$inject = ['$scope', 'WhS_Analytics_VariationReportTypeEnum', 'WhS_Analytics_VariationReportTimePeriodEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRDateTimeService'];

    function VariationReportController($scope, WhS_Analytics_VariationReportTypeEnum, WhS_Analytics_VariationReportTimePeriodEnum, UtilsService, VRUIUtilsService, VRNotificationService, VRDateTimeService) {

        var reportTypeSelectorAPI;
        var reportTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        var chartAPI;
        var chartReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.toDate = VRDateTimeService.getNowDateTime();

            $scope.scopeModel.periodTypes = UtilsService.getArrayEnum(WhS_Analytics_VariationReportTimePeriodEnum);
            $scope.scopeModel.selectedPeriodType = UtilsService.getEnum(WhS_Analytics_VariationReportTimePeriodEnum, 'value', WhS_Analytics_VariationReportTimePeriodEnum.Daily.value);

            $scope.scopeModel.numberOfPeriods = 7;
            $scope.scopeModel.top = 5;

            $scope.scopeModel.onReportTypeSelectionChanged = function () {
                if ($scope.scopeModel.selectedPeriodType == undefined)
                    return;

                $scope.scopeModel.showGrid = false;
                $scope.scopeModel.showChart = false;

                showTopFilter();
                showCurrencyFilter();

                function showTopFilter() {
                    switch (reportTypeSelectorAPI.getSelectedIds()) {
                        case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                        case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                            $scope.scopeModel.showTopFilter = false;
                            return;
                    }
                    $scope.scopeModel.showTopFilter = true;
                }
                function showCurrencyFilter() {
                    switch (reportTypeSelectorAPI.getSelectedIds()) {
                        case WhS_Analytics_VariationReportTypeEnum.InBoundAmount.value:
                        case WhS_Analytics_VariationReportTypeEnum.OutBoundAmount.value:
                        case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                        case WhS_Analytics_VariationReportTypeEnum.TopDestinationAmount.value:
                        case WhS_Analytics_VariationReportTypeEnum.Profit.value:
                            $scope.scopeModel.showCurrencyFilter = true;
                            return;
                    }
                    $scope.scopeModel.showCurrencyFilter = false;
                }
            };
            $scope.scopeModel.onVariationReportTypeReady = function (api) {
                reportTypeSelectorAPI = api;
                reportTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.onChartReady = function (api) {
                chartAPI = api;
                chartReadyDeferred.resolve();
            };

            $scope.scopeModel.validateTopFilter = function () {
                if (gridAPI == undefined) // Handle the case when/if validateTopFilter executes before onGridReady
                    return null;
                var pageSize = gridAPI.getPageSize();
                return ($scope.scopeModel.top > pageSize) ? ('Top <= ' + pageSize) : null;
            };

            $scope.scopeModel.search = function () {
                $scope.scopeModel.showGrid = true;
                return search();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            var loadCurrencySelectorPromise = loadCurrencySelector();
            var loadReportTypeSelectorPromise = loadReportTypeSelector();

            return UtilsService.waitMultiplePromises([loadReportTypeSelectorPromise, loadCurrencySelectorPromise, gridReadyDeferred.promise, chartReadyDeferred.promise]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadCurrencySelector() {
            var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            currencySelectorReadyDeferred.promise.then(function () {
                var currencySelectorPayload = { selectSystemCurrency: true };
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadDeferred);
            });

            return currencySelectorLoadDeferred.promise;
        }

        function loadReportTypeSelector() {
            var reportTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            reportTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(reportTypeSelectorAPI, { selectfirstitem: true }, reportTypeSelectorLoadDeferred);
            });

            return reportTypeSelectorLoadDeferred.promise;
        }

        function search() {
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
                ReportType: reportTypeSelectorAPI.getSelectedIds(),
                ToDate: $scope.scopeModel.toDate,
                TimePeriod: $scope.scopeModel.selectedPeriodType.value,
                NumberOfPeriods: $scope.scopeModel.numberOfPeriods,
                GroupByProfile: $scope.scopeModel.groupByProfile,
                CurrencyId: ($scope.scopeModel.showCurrencyFilter) ? currencySelectorAPI.getSelectedIds() : null
            };
        }

        function loadChart() {
            switch (reportTypeSelectorAPI.getSelectedIds()) {
                case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.InOutBoundAmount.value:
                    return;
            }

            $scope.scopeModel.showChart = true;

            var reportData = gridAPI.getData();

            var chartConfig =
            {
                type: 'line',
                yAxisTitle: getReportResultTypeDescription()
            };

            var seriesConfig = { titlePath: 'seriesTitle' };

            var seriesList = [];

            seriesList.push
            ({
                seriesTitle: 'AVG',
                seriesData: []
            });

            for (var i = 0; i < reportData.timePeriods.length; i++) {
                var seriesItem =
                {
                    seriesTitle: reportData.timePeriods[i].PeriodDescription,
                    seriesData: []
                };
                seriesList.push(seriesItem);
            }

            var seriesDefinition = [];
            for (var i = 0; i < reportData.records.length && i < $scope.scopeModel.top; i++) {
                var record = reportData.records[i];

                seriesDefinition.push
                ({
                    title: record.DimensionName,
                    valuePath: 'seriesData[' + i + ']',
                        type: 'line'
                });

                seriesList[0].seriesData[i] = record.Average;

                for (var j = 1; j <= reportData.timePeriods.length; j++) {
                    seriesList[j].seriesData[i] = record.TimePeriodValues[j - 1];
                }
            }

            chartAPI.renderChart(seriesList, chartConfig, seriesDefinition, seriesConfig);
        }

        function getReportResultTypeDescription() {
            switch (reportTypeSelectorAPI.getSelectedIds()) {
                case WhS_Analytics_VariationReportTypeEnum.InBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.OutBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.InOutBoundMinutes.value:
                case WhS_Analytics_VariationReportTypeEnum.TopDestinationMinutes.value:
                    return 'Duration (Minutes)';

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