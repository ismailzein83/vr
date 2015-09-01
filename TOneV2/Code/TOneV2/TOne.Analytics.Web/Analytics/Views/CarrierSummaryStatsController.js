(function (appControllers) {

    "use strict";

    CarrierSummaryStatsController.$inject = ['$scope', 'CarrierSummaryStatsAPIService', 'CarrierAccountAPIService', 'CarrierSummaryMeasureEnum', 'ZoneAPIService', 'CurrencyAPIService', 'CarrierTypeEnum', 'VRModalService', 'AnalyticsService'];
    function CarrierSummaryStatsController($scope, CarrierSummaryStatsAPIService, CarrierAccountAPIService, CarrierSummaryMeasureEnum, ZoneAPIService, CurrencyAPIService, CarrierTypeEnum, VRModalService, analyticsService) {

        var gridApi, measureFields = [];


        var groupKeys = analyticsService.getCarrierZoneSummaryGroupKeys();
        var measures = [];
        var currentData;
        defineScope();
        load();

        function defineScope() {

            var now = new Date();
            $scope.fromDate = new Date(2013, 1, 1);
            $scope.toDate = now;


            $scope.groupKeys = groupKeys;
            $scope.selectedGroupKeys = [];
            $scope.measures = measures;

            $scope.currentSearchCriteria = {
                groupKeys: []
            };

            $scope.gridReady = function (api) {
                gridApi = api;
            }

            $scope.searchClicked = function () {
                $scope.currentSearchCriteria.groupKeys.length = 0;
                angular.forEach($scope.selectedGroupKeys, function (group) {
                    $scope.currentSearchCriteria.groupKeys.push(group);
                });
                
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CarrierSummaryStatsAPIService.GetFiltered(dataRetrievalInput)
                .then(function (response) {

                    //gridApi.setSummary(response.Summary);

                    var index = 1;
                    angular.forEach(currentData, function (itm) {

                        if (index > 15)
                            return;
                        index++;

                        var dataItem = {
                            groupKeyValues: itm.GroupFieldValues,
                            entityName: ''
                            //value: itm.Data[measure.propertyName]
                        };

                        for (var i = 0; i < $scope.currentSearchCriteria.groupKeys.length; i++) {
                            if (dataItem.entityName.length > 0)
                                dataItem.entityName += ' - ';
                            dataItem.entityName += itm.GroupFieldValues[i].Name;
                        };
                    });


                    onResponseReady(response);
                });
            };


        }


        function retrieveData() {

            $scope.datasource = [];

            var groupKeys = [];

            angular.forEach($scope.selectedGroupKeys, function (group) {
                groupKeys.push(group.value);
            });


            var query = {
                GroupFields: groupKeys,
                MeasureFields: measureFields,
                FromTime: $scope.fromDate,
                ToTime: $scope.toDate,
            };
            return gridApi.retrieveData(query);
        }

        function load() {
            loadMeasures();
            loadQueryMeasures();
        }


        function loadQueryMeasures() {
            if (measureFields.length == 0)
                for (var prop in CarrierSummaryMeasureEnum) {
                    measureFields.push(CarrierSummaryMeasureEnum[prop].value);
                }
        }

        function loadMeasures() {
            for (var prop in CarrierSummaryMeasureEnum) {
                measures.push(CarrierSummaryMeasureEnum[prop]);
            }
        }


    }
    appControllers.controller('Carrier_CarrierSummaryStatsController', CarrierSummaryStatsController);

})(appControllers);