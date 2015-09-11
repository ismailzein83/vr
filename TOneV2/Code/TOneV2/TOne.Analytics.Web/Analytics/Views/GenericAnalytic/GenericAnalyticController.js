(function (appControllers) {

    "use strict";

    GenericAnalyticController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticMeasureEnum', 'AnalyticsService', 'BusinessEntityAPIService_temp'];
    function GenericAnalyticController($scope, GenericAnalyticAPIService, GenericAnalyticMeasureEnum, analyticsService, BusinessEntityAPIService) {

        var measureFields;
        load();

        function defineScope() {

            var now = new Date();
            $scope.fromDate = new Date(2013, 1, 1);
            $scope.toDate = now;
            $scope.subViewConnector = {};

            $scope.click = function () {
                console.log($scope.subViewConnector.getValue());
                $scope.subViewConnector.setValue("setValue");
            }
            measureFields = analyticsService.getGenericAnalyticMeasureValues();
            //$scope.measures = analyticsService.getGenericAnalyticMeasures();
            $scope.groupKeys = analyticsService.getGenericAnalyticGroupKeys();

            $scope.selectedGroupKeys = [];
            $scope.currentSearchCriteria = {
                groupKeys: []
            };

            $scope.switches = [];
            $scope.selectedSwitches = [];
            $scope.codeGroups = [];
            $scope.selectedCodeGroups = [];

            $scope.measures = [];
            $scope.selectedMeasures = [];

            loadSwitches();
            loadCodeGroups();
            loadMeasures();
       

            $scope.searchClicked = function () { 
                return retrieveData();
            };

            

            $scope.checkExpandablerow = function (groupKeys) {
                return groupKeys.length !== $scope.groupKeys.length;
            };

            $scope.groupSelectionChanged = function () {
                $scope.currentSearchCriteria.groupKeys.length = 0;

            };
        }

        function retrieveData() {
           
            

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
                ToTime: $scope.toDate,
                GroupKeys:$scope.selectedGroupKeys
            };
            console.log(query);

            $scope.subViewConnector.retrieveData(query);
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


        function loadMeasures() {
            return $scope.measures = $scope.selectedMeasures = analyticsService.getGenericAnalyticMeasures();
        }

    }
    appControllers.controller('Generic_GenericAnalyticController', GenericAnalyticController);

})(appControllers);