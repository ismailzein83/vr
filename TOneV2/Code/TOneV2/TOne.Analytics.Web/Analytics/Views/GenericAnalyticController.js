(function (appControllers) {

    "use strict";

    GenericAnalyticController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticMeasureEnum',  'AnalyticsService'];
    function GenericAnalyticController($scope, GenericAnalyticAPIService, GenericAnalyticMeasureEnum,  analyticsService) {

        var gridApi, measureFields;
        load();

        function defineScope() {

            var now = new Date();
            $scope.fromDate = new Date(2013, 1, 1);
            $scope.toDate = now;

            measureFields = analyticsService.getGenericAnalyticMeasureValues();
            $scope.measures = analyticsService.getGenericAnalyticMeasures();
            $scope.groupKeys = analyticsService.getGenericAnalyticGroupKeys();

            $scope.selectedGroupKeys = [];
            $scope.currentSearchCriteria = {
                groupKeys: []
            };
            $scope.gridReady = function (api) {
                gridApi = api;
            };
            $scope.searchClicked = function () { 
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                .then(function (response) {
                    $scope.currentSearchCriteria.groupKeys.length = 0;
                    $scope.selectedGroupKeys.forEach(function (group) {
                        $scope.currentSearchCriteria.groupKeys.push(group);
                    });
                    //gridApi.setSummary(response.Summary);

                    onResponseReady(response);
                });
            };
        }

        function retrieveData() {
            if (gridApi == undefined)
                return;
            $scope.datasource = [];

            var groupKeys = [];
            var filters = [];

            $scope.selectedGroupKeys.forEach(function (group) {
                groupKeys.push(group.value);
            });


            var query = {
                Filters: filters,
                DimensionFields: groupKeys,
                MeasureFields: measureFields,
                FromTime: $scope.fromDate,
                ToTime: $scope.toDate
            };
            return gridApi.retrieveData(query);
        }

        function load() {
            defineScope();
        }
    }
    appControllers.controller('Generic_GenericAnalyticController', GenericAnalyticController);

})(appControllers);