(function (appControllers) {

    "use strict";

    CarrierSummaryController.$inject = ['$scope', 'GenericAnalyticDimensionEnum', 'AnalyticsService', 'BusinessEntityAPIService_temp', 'ZonesService', 'CurrencyAPIService', 'GenericAnalyticMeasureEnum'];
    function CarrierSummaryController($scope, GenericAnalyticDimensionEnum, analyticsService, BusinessEntityAPIService, ZonesService, CurrencyAPIService, GenericAnalyticMeasureEnum) {

        var measureFieldsValues = [];
        load();

        function defineScope() {

            $scope.subViewConnector = {};

            $scope.groupKeys = [];
            $scope.filterKeys = [];
            $scope.measures = [];
            $scope.selectedobject = {};

            loadGroupKeys();
            loadFilterKeys();
            loadMeasures();

            $scope.searchClicked = function () {
                return retrieveData();
            };
        }

        function retrieveData() {
            var query = {
                Filters: $scope.selectedobject.selectedfilters,
                DimensionFields: $scope.selectedobject.selecteddimensions,
                MeasureFields: measureFieldsValues,
                FromTime: $scope.selectedobject.fromdate,
                ToTime: $scope.selectedobject.todate,
                Currency: $scope.selectedobject.currency
            };
            $scope.subViewConnector.retrieveData(query);
        }

        function load() {
            defineScope();
        }

        function loadGroupKeys() {
            for (var item in GenericAnalyticDimensionEnum) {
                if ((GenericAnalyticDimensionEnum[item].name == GenericAnalyticDimensionEnum.Customer.name) ||
                    (GenericAnalyticDimensionEnum[item].name == GenericAnalyticDimensionEnum.Supplier.name))
                    $scope.groupKeys.push(GenericAnalyticDimensionEnum[item]);
            }
        }

        function loadFilterKeys() {
            for (var item in GenericAnalyticDimensionEnum) {
                if ((GenericAnalyticDimensionEnum[item].name == GenericAnalyticDimensionEnum.Customer.name) ||
                    (GenericAnalyticDimensionEnum[item].name == GenericAnalyticDimensionEnum.Supplier.name))
                    $scope.filterKeys.push(GenericAnalyticDimensionEnum[item]);
            }
        }

        function loadMeasures() {
            for (var item in GenericAnalyticMeasureEnum) {
                if ((GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_FirstCDRAttempt.value) ||
                    (GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_ABR.value) ||
                    (GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_ASR.value) ||
                    (GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_NER.value) ||
                    (GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_Attempts.value) ||
                    (GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_SuccessfulAttempts.value) ||
                    (GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_FailedAttempts.value) ||
                    (GenericAnalyticMeasureEnum[item].value == GenericAnalyticMeasureEnum.Measure_DeliveredAttempts.value))
                {
                    measureFieldsValues.push(GenericAnalyticMeasureEnum[item].value);
                    $scope.measures.push(GenericAnalyticMeasureEnum[item]);
                }
                
            }
        }

    }
    appControllers.controller('Carrier_CarrierSummaryController', CarrierSummaryController);

})(appControllers);