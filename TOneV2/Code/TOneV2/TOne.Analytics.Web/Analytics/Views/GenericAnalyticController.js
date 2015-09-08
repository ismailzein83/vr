(function (appControllers) {

    "use strict";

    GenericAnalyticController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticMeasureEnum', 'AnalyticsService', 'BusinessEntityAPIService_temp'];
    function GenericAnalyticController($scope, GenericAnalyticAPIService, GenericAnalyticMeasureEnum, analyticsService, BusinessEntityAPIService) {

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

            $scope.switches = [];
            $scope.selectedSwitches = [];
            $scope.codeGroups = [];
            $scope.selectedCodeGroups = [];

            loadSwitches();
            loadCodeGroups();
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

            $scope.checkExpandablerow = function (groupKeys) {
                return groupKeys.length !== $scope.groupKeys.length;
            };
        }

        function retrieveData() {
            if (gridApi == undefined)
                return;
            $scope.datasource = [];


            //var filters = [[3, ['93', '376', '684', '1684', '213', '355']]];
            var filters = [];
    

            var groupKeys = [];
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
            console.log(query);
            return gridApi.retrieveData(query);
        }

        function load() {
            defineScope();
        }

        function loadSwitches() {
            return BusinessEntityAPIService.GetSwitches().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.switches.push(itm);
                });
            });
        }

        function loadCodeGroups() {
            return BusinessEntityAPIService.GetCodeGroups().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.codeGroups.push(itm);
                });
            });
        }


    }
    appControllers.controller('Generic_GenericAnalyticController', GenericAnalyticController);

})(appControllers);