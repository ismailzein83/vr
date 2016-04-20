(function (appControllers) {

    'use strict';

    VariationReportController.$inject = ['$scope', 'WhS_Analytics_VariationReportTypeEnum', 'WhS_Analytics_VariationReportTimePeriodEnum', 'UtilsService'];

    function VariationReportController($scope, WhS_Analytics_VariationReportTypeEnum, WhS_Analytics_VariationReportTimePeriodEnum, UtilsService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.reportTypes = UtilsService.getArrayEnum(WhS_Analytics_VariationReportTypeEnum);
            $scope.scopeModel.selectedReportType = UtilsService.getEnum(WhS_Analytics_VariationReportTypeEnum, 'value', WhS_Analytics_VariationReportTypeEnum.InBoundMinutes.value);

            $scope.scopeModel.toDate = new Date('2016-01-05');

            $scope.scopeModel.periodTypes = UtilsService.getArrayEnum(WhS_Analytics_VariationReportTimePeriodEnum);;
            $scope.scopeModel.selectedPeriodType = UtilsService.getEnum(WhS_Analytics_VariationReportTimePeriodEnum, 'value', WhS_Analytics_VariationReportTimePeriodEnum.Daily.value);

            $scope.scopeModel.numberOfPeriods = 3;
            $scope.scopeModel.showGrid = false;

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.search = function () {
                $scope.scopeModel.showGrid = true;
                return gridAPI.load(getGridQuery());
            };
        }

        function load() {
            
        }

        function getGridQuery() {
            return {
                ReportType: $scope.scopeModel.selectedReportType.value,
                ToDate: $scope.scopeModel.toDate,
                TimePeriod: $scope.scopeModel.selectedPeriodType.value,
                NumberOfPeriods: $scope.scopeModel.numberOfPeriods
            };
        }
    }

    appControllers.controller('WhS_Analytics_VariationReportController', VariationReportController);

})(appControllers);