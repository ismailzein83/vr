(function (appControllers) {

    "use strict";

    GTrafficMonitorController.$inject = ['$scope', 'GenericAnalyticDimensionEnum', 'GenericAnalyticMeasureEnum'];
    function GTrafficMonitorController($scope, GenericAnalyticDimensionEnum, GenericAnalyticMeasureEnum) {

        load();

        function defineScope() {

            $scope.subViewConnector = {};

            $scope.groupKeys = [];
            $scope.fixedgroupKeys = [];
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
                FixedDimensionFields: $scope.fixedgroupKeys,
                MeasureFields: $scope.measures,
                FromTime: $scope.selectedobject.fromdate,
                ToTime: $scope.selectedobject.todate,
                Currency: $scope.selectedobject.currency
            };
            return $scope.subViewConnector.retrieveData(query);
        }

        function load() {
            defineScope();
        }

        function loadGroupKeys() {
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Customer);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Zone);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.CodeGroup);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.Switch);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.GateWayIn);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.GateWayOut);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.PortIn);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.PortOut);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.CodeSales);
            $scope.groupKeys.push(GenericAnalyticDimensionEnum.CodeBuy);
        }

        function loadFilterKeys() {
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Customer);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Supplier);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.CodeGroup);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Switch);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.ASR);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.ACD);
            $scope.filterKeys.push(GenericAnalyticDimensionEnum.Attempts);
        }

        function loadMeasures() {
            $scope.measures.push(GenericAnalyticMeasureEnum.DeliveredAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.SuccessfulAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.FailedAttempts);
            $scope.measures.push(GenericAnalyticMeasureEnum.DurationsInSeconds);
            $scope.measures.push(GenericAnalyticMeasureEnum.CeiledDuration);
            $scope.measures.push(GenericAnalyticMeasureEnum.ASR);
            $scope.measures.push(GenericAnalyticMeasureEnum.ABR);
            $scope.measures.push(GenericAnalyticMeasureEnum.NER);
            $scope.measures.push(GenericAnalyticMeasureEnum.ACD);
            $scope.measures.push(GenericAnalyticMeasureEnum.PDDInSeconds);
            $scope.measures.push(GenericAnalyticMeasureEnum.PGAD);
            $scope.measures.push(GenericAnalyticMeasureEnum.MaxDurationInSeconds);
            $scope.measures.push(GenericAnalyticMeasureEnum.LastCDRAttempt);
        }
    }
    appControllers.controller('Analytics_GTrafficMonitorController', GTrafficMonitorController);

})(appControllers);