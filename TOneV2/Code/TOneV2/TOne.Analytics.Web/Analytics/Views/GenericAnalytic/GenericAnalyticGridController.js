(function (appControllers) {

    "use strict";

    GenericAnalyticGridController.$inject = ['$scope', 'GenericAnalyticAPIService'];
    function GenericAnalyticGridController($scope, GenericAnalyticAPIService) {
        var gridApi;
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
                var query = {
                    Filters: value.Filters,
                    DimensionFields: groupKeys,
                    MeasureFields: value.MeasureFields,
                    FromTime: value.FromTime,
                    ToTime: value.ToTime,
                    Currency: value.Currency
                }
                gridApi.retrieveData(query);
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
        }
    }
    appControllers.controller('GenericAnalyticGridController', GenericAnalyticGridController);

})(appControllers);