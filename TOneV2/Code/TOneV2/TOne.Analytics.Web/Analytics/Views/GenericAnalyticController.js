(function (appControllers) {

    "use strict";

    GenericAnalyticController.$inject = ['$scope', 'GenericAnalyticAPIService', 'GenericAnalyticMeasureEnum', 'VRModalService', 'AnalyticsService'];
    function GenericAnalyticController($scope, GenericAnalyticAPIService, GenericAnalyticMeasureEnum, VRModalService, analyticsService) {

        var gridApi, measureFields = [];


        var groupKeys = analyticsService.getGenericAnalyticGroupKeys();
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
            };

            $scope.searchClicked = function () {
                $scope.currentSearchCriteria.groupKeys.length = 0;
                angular.forEach($scope.selectedGroupKeys, function (group) {
                    $scope.currentSearchCriteria.groupKeys.push(group);
                });
                
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                .then(function (response) {

                    //gridApi.setSummary(response.Summary);

                    var index = 1;
                    angular.forEach(currentData, function (itm) {

                        if (index > 15)
                            return;
                        index++;

                        var dataItem = {
                            groupKeyValues: itm.DimensionValues,
                            entityName: ''
                            //value: itm.Data[measure.propertyName]
                        };

                        for (var i = 0; i < $scope.currentSearchCriteria.groupKeys.length; i++) {
                            if (dataItem.entityName.length > 0)
                                dataItem.entityName += ' - ';
                            dataItem.entityName += itm.DimensionValues[i].Name;
                        };
                    });


                    onResponseReady(response);
                });
            };


        }


        function retrieveData() {

            $scope.datasource = [];

            var groupKeys = [];
            var filters = [];

            angular.forEach($scope.selectedGroupKeys, function (group) {
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
            loadMeasures();
            loadQueryMeasures();
        }


        function loadQueryMeasures() {
            if (measureFields.length == 0)
                for (var prop in GenericAnalyticMeasureEnum) {
                    measureFields.push(GenericAnalyticMeasureEnum[prop].value);
                }
        }

        function loadMeasures() {
            for (var prop in GenericAnalyticMeasureEnum) {
                measures.push(GenericAnalyticMeasureEnum[prop]);
            }
        }


    }
    appControllers.controller('Generic_GenericAnalyticController', GenericAnalyticController);

})(appControllers);