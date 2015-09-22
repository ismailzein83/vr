(function (appControllers) {

    "use strict";

    genericAnalyticGridController.$inject = ['$scope', 'GenericAnalyticAPIService'];
    function genericAnalyticGridController($scope, GenericAnalyticAPIService) {
        var gridApi;
        var measureFieldsValues = [];

        defineScope();
        function defineScope() {
            
            $scope.datasource = [];

            $scope.gridReady = function (api) {
                gridApi = api;
            };

            $scope.currentSearchCriteria = {
                groupKeys:[]
            }

            var selectedGroupKeys = [];

            $scope.subViewConnector.getValue = function () {
                return "GetValue";
            };
            $scope.subViewConnector.retrieveData = function (value) {
                $scope.subViewConnector.value = value;

                if (gridApi == undefined)
                    return;

                var groupKeys = [];
                value.DimensionFields.forEach(function (group) {
                    groupKeys.push(group.value);
                });

                selectedGroupKeys = value.DimensionFields;

                for (var i = 0, len = value.MeasureFields.length; i < len; i++) {
                    measureFieldsValues.push(value.MeasureFields[i].value);
                }

                var query = {
                    Filters: value.Filters,
                    DimensionFields: groupKeys,
                    MeasureFields: measureFieldsValues,
                    FromTime: value.FromTime,
                    ToTime: value.ToTime,
                    Currency: value.Currency
                }

                $scope.selectedMeasures = value.MeasureFields;
                $scope.fromDate = value.FromTime;
                $scope.toDate = value.ToTime;
                $scope.Currency = value.Currency;
                $scope.selectedfilters = value.Filters;
                return gridApi.retrieveData(query);
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return GenericAnalyticAPIService.GetFiltered(dataRetrievalInput)
                .then(function (response) {
                    $scope.currentSearchCriteria.groupKeys.length = 0;
                    selectedGroupKeys.forEach(function (group) {
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
    }
    appControllers.controller('GenericAnalyticGridController', genericAnalyticGridController);

})(appControllers);