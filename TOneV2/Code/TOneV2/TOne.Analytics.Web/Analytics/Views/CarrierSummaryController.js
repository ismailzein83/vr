(function (appControllers) {

    "use strict";

    CarrierSummaryController.$inject = ['$scope', 'GenericAnalyticDimensionEnum', 'GenericAnalyticMeasureEnum'];
    function CarrierSummaryController($scope, GenericAnalyticDimensionEnum, GenericAnalyticMeasureEnum) {

        load();

        function defineScope() {

            $scope.subViewConnector = {};

            $scope.groupKeys = [];
            $scope.filterKeys = [];
            $scope.measures = [];
            $scope.selectedobject = {};
            $scope.showResult = false;
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
                    MeasureFields: $scope.measures,
                    FromTime: $scope.selectedobject.fromdate,
                    ToTime: $scope.selectedobject.todate,
                    Currency: $scope.selectedobject.currency
                };
                $scope.showResult = true;
                return $scope.subViewConnector.retrieveData(query);
        }

        function load() {
            defineScope();
        }

        function loadGroupKeys() {
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Customer);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Zone);
        }

        function loadFilterKeys() {
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Customer);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Zone);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Currency);
        }

        function loadMeasures() {
            $scope.measures.push(GenericAnalyticMeasureEnum.FirstCDRAttempt);
            $scope.measures.push(GenericAnalyticMeasureEnum.ABR);
            $scope.measures.push(GenericAnalyticMeasureEnum.ASR);
            $scope.measures.push(GenericAnalyticMeasureEnum.NER);
            $scope.measures.push(GenericAnalyticMeasureEnum.Attempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.SuccessfulAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.FailedAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.DeliveredAttempts);
        }
    }
    appControllers.controller('Carrier_CarrierSummaryController', CarrierSummaryController);

})(appControllers);